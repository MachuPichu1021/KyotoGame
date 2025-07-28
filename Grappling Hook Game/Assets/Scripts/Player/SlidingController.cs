using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float slideTimer;

    [SerializeField] private float slideYScale;
    private float startYScale;

    [SerializeField] private AudioClip slideSFX;

    [Header("Keybinds")]
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;

    private float horizontalInput, verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0)) StartSlide();

        if (Input.GetKeyUp(slideKey) && playerMovement.sliding) StopSlide();
    }

    private void FixedUpdate()
    {
        if (playerMovement.sliding) Sliding();
    }

    private void StartSlide()
    {
        playerMovement.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(5 * Vector3.down, ForceMode.Impulse);

        slideTimer = maxSlideTime;
        AudioManager.instance.PlaySound(slideSFX);
    }

    private void Sliding()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!playerMovement.IsOnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(slideForce * inputDirection.normalized, ForceMode.Force);
            slideTimer -= Time.fixedDeltaTime;
        }
        else rb.AddForce(slideForce * playerMovement.GetSlopeMoveDirection(inputDirection), ForceMode.Force);


        if (slideTimer <= 0) StopSlide();
    }

    private void StopSlide()
    {
        playerMovement.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        AudioManager.instance.Stop();
    }
}
