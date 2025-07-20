using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookController : MonoBehaviour
{
    public static bool isGrappling;

    [SerializeField] private LineRenderer lr;

    [SerializeField] private Transform grappleOutPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask grappleLayer;

    private float maxSwingDistance = 25;
    private Vector3 swingPoint;
    private Vector3 currentGrapplePosition;
    private SpringJoint joint;

    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] float horizontalThrust;
    [SerializeField] float forwardThrust;
    [SerializeField] float cableExtensionSpeed;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) StartSwing();
        if (Input.GetKeyUp(KeyCode.Mouse0)) StopSwing();

        if (joint != null) AirMovement();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, grappleLayer))
        {
            isGrappling = true;

            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distance = Vector3.Distance(swingPoint, transform.position);
            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = grappleOutPoint.position;
        }
    }

    private void StopSwing()
    {
        isGrappling = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        if (joint == null) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8);

        lr.SetPosition(0, grappleOutPoint.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void AirMovement()
    {
        if (Input.GetKey(KeyCode.D)) rb.AddForce(horizontalThrust * Time.deltaTime * orientation.right);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(horizontalThrust * Time.deltaTime * -orientation.right);

        if (Input.GetKey(KeyCode.W)) rb.AddForce(forwardThrust * Time.deltaTime * orientation.forward);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(forwardThrust * Time.deltaTime * -orientation.forward);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 direction = swingPoint - transform.position;
            rb.AddForce(forwardThrust * Time.deltaTime * direction.normalized);

            float distance = Vector3.Distance(transform.position, swingPoint);
            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.25f;
        }
    }
}
