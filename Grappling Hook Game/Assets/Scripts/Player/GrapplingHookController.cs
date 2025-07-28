using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingHookController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform grappleOutPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask grappleLayer;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [SerializeField] private float maxSwingDistance = 30;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Speeds")]
    [SerializeField] private float horizontalThrust;
    [SerializeField] private float forwardThrust;
    [SerializeField] private float cableExtensionSpeed;

    [Header("Prediction")]
    [SerializeField] private RaycastHit predictionHit;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private Transform predictionPoint;

    [Header("Grapple Visuals")]
    [SerializeField] private Image grappleImage;
    [SerializeField] private Sprite[] grappleExtensionSprites;
    [SerializeField] private Sprite grappleActiveSprite;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite crosshairNoGrapple;
    [SerializeField] private Sprite crosshairCanGrapple;

    [Header("Audio")]
    [SerializeField] private AudioClip grappleShootSFX;
    [SerializeField] private AudioClip grapplePullSFX;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        grappleImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) StartSwing();
        if (Input.GetKeyUp(KeyCode.Mouse0)) StopSwing();

        CheckForSwingPoints();

        if (joint != null) AirMovement();

        crosshairImage.sprite = predictionPoint.gameObject.activeInHierarchy ? crosshairCanGrapple : crosshairNoGrapple;
    }

    private void StartSwing()
    {
        if (predictionHit.point == Vector3.zero) return;

        playerMovement.swinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distance = Vector3.Distance(swingPoint, transform.position);
        joint.maxDistance = distance * 0.8f;
        joint.minDistance = distance * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7;
        joint.massScale = 4.5f;

        AudioManager.instance.PlaySound(grappleShootSFX);
        StartCoroutine(GrappleExtendAnimation());
    }

    private void StopSwing()
    {
        playerMovement.swinging = false;
        grappleImage.gameObject.SetActive(false);
        Destroy(joint);
    }

    private void AirMovement()
    {
        if (Input.GetKey(KeyCode.D)) rb.AddForce(horizontalThrust * Time.deltaTime * orientation.right, ForceMode.Force);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(horizontalThrust * Time.deltaTime * -orientation.right, ForceMode.Force);

        if (Input.GetKey(KeyCode.W)) rb.AddForce(forwardThrust * Time.deltaTime * orientation.forward, ForceMode.Force);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(forwardThrust * Time.deltaTime * -orientation.forward, ForceMode.Force);

        if (Input.GetKeyDown(KeyCode.Mouse1)) AudioManager.instance.PlaySound(grapplePullSFX);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 direction = swingPoint - transform.position;
            rb.AddForce(cableExtensionSpeed * direction.normalized, ForceMode.Force);

            float distance = Vector3.Distance(transform.position, swingPoint);
            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * 0.25f;
        }
    }

    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out RaycastHit sphereCastHit, maxSwingDistance, grappleLayer);
        Physics.Raycast(cam.position, cam.forward, out RaycastHit raycastHit, maxSwingDistance, grappleLayer);

        Vector3 realHitPoint;
        if (raycastHit.point != Vector3.zero) realHitPoint = raycastHit.point;
        else if (sphereCastHit.point != Vector3.zero) realHitPoint = sphereCastHit.point;
        else realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else predictionPoint.gameObject.SetActive(false);

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    private IEnumerator GrappleExtendAnimation()
    {
        int index = 0;
        grappleImage.gameObject.SetActive(true);
        while (index < grappleExtensionSprites.Length)
        {
            grappleImage.sprite = grappleExtensionSprites[index];
            yield return new WaitForSeconds(0.1f);
            index++;
        }
        grappleImage.sprite = grappleActiveSprite;
    }
}
