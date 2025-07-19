using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider normalColl;
    private Collider slidingColl;

    [SerializeField] private Camera cam;
    private float pitch = 0;
    private float roll;

    [SerializeField] private float speed = 5;
    private const float maxSpeed = 15;

    private const float jumpPower = 7.5f;
    [SerializeField] private Transform groundCheckPoint;

    [SerializeField] private Transform leftWallCheckPoint;
    [SerializeField] private Transform rightWallCheckPoint;

    private bool isSliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalColl = GetComponent<CapsuleCollider>();
        slidingColl = GetComponent<SphereCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        bool isOnGround = IsOnGround();
        bool isOnWall = IsOnWall();

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"));

        Vector2 input = Vector2.zero;
        if (!isSliding)
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input.magnitude > 1) input = input.normalized;
            Vector3 velocity = (input.x * transform.right + input.y * transform.forward) * speed;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            if (input.y > 0) speed += 2.5f * Time.deltaTime;
            else speed -= 5 * Time.deltaTime;
            speed = Mathf.Clamp(speed, 5, maxSpeed);

            if (Input.GetKeyDown(KeyCode.Space) && (isOnGround || isOnWall)) rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }

        pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -60, 60);
        roll = -7.5f * input.x;
        if (isOnWall) roll *= -1;
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, roll);
        cam.fieldOfView = 60 + 3 * speed;

        if (Input.GetKeyDown(KeyCode.LeftControl) && speed > 10)
        {
            speed += 10;
            isSliding = true;
            normalColl.enabled = false;
            slidingColl.enabled = true;
        }
        if (isSliding)
        {
            speed -= 10 * Time.deltaTime;
            if (speed < 7.5f)
            {
                isSliding = false;
                normalColl.enabled = true;
                slidingColl.enabled = false;
            }
        }
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.05f);
    }

    private bool IsOnWall()
    {
        return ((Physics.Raycast(leftWallCheckPoint.position, -transform.right, 0.05f) && Input.GetKey(KeyCode.A))
            || (Physics.Raycast(rightWallCheckPoint.position, transform.right, 0.05f) && Input.GetKey(KeyCode.D))) && !IsOnGround();
    }
}
