using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Camera cam;
    private float pitch = 0;
    private float roll;

    [SerializeField] private float speed = 5;
    private const float maxSpeed = 15;

    private const float jumpPower = 7.5f;
    [SerializeField] private Transform groundCheckPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"));

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.magnitude > 1) input = input.normalized;
        Vector3 velocity = (input.x * transform.right + input.y * transform.forward) * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        if (input.y > 0) speed += 2.5f * Time.deltaTime;
        else speed -= 5 * Time.deltaTime;
        speed = Mathf.Clamp(speed, 5, 15);

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround()) rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -60, 60);
        roll = -5 * input.x;
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, roll);
        cam.fieldOfView = 60 + 3 * speed;
    }

    private bool isOnGround()
    {
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.05f);
    }
}
