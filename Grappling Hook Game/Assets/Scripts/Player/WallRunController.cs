using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCam;

    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Wall Run")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float maxWallRunTime;
    private float wallRunTimer;
    private bool canWallRun;

    [SerializeField] private float maxWallRunCamTilt;
    private float wallRunCamTilt;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerMovement.isOnGround) canWallRun = true;

        CheckForWall();
        WallRunInput();

        playerCam.localRotation = Quaternion.Euler(playerCam.localRotation.eulerAngles.x, playerCam.localRotation.eulerAngles.y, wallRunCamTilt);

        if (Mathf.Abs(wallRunCamTilt) < maxWallRunCamTilt && playerMovement.wallRunning)
        {
            if (playerMovement.wallRight) wallRunCamTilt += Time.deltaTime * maxWallRunCamTilt * 2;
            else if (playerMovement.wallLeft) wallRunCamTilt -= Time.deltaTime * maxWallRunCamTilt * 2;
        }

        if (!playerMovement.wallRunning)
        {
            if (Mathf.Abs(wallRunCamTilt) > 0.1f)
            {
                if (wallRunCamTilt > 0) wallRunCamTilt -= Time.deltaTime * maxWallRunCamTilt * 2;
                else if (wallRunCamTilt < 0) wallRunCamTilt += Time.deltaTime * maxWallRunCamTilt * 2;
            }
            else wallRunCamTilt = 0;
        }
    }

    private void FixedUpdate()
    {
        if (playerMovement.wallRunning) WallRun();
    }

    private void WallRunInput()
    {
        if (canWallRun && !playerMovement.wallRunning && !playerMovement.isOnGround)
        {
            if (Input.GetKey(KeyCode.D) && playerMovement.wallRight) StartWallRun();
            if (Input.GetKey(KeyCode.A) && playerMovement.wallLeft) StartWallRun();
        }
    }

    private void StartWallRun()
    {
        wallRunTimer = maxWallRunTime;

        rb.useGravity = false;
        playerMovement.wallRunning = true;
    }

    private void WallRun()
    {
        rb.AddForce(wallRunForce * orientation.forward, ForceMode.Force);

        if (playerMovement.wallRight) rb.AddForce(wallRunForce / 5 * orientation.right, ForceMode.Force);
        else if (playerMovement.wallLeft) rb.AddForce(wallRunForce / 5 * -orientation.right, ForceMode.Force);

        wallRunTimer -= Time.fixedDeltaTime;
        if (wallRunTimer <= 0)
        {
            canWallRun = false;
            StopWallRun();
        }
    }

    private void StopWallRun()
    {
        rb.useGravity = true;
        playerMovement.wallRunning = false;
    }

    private void CheckForWall()
    {
        playerMovement.wallRight = Physics.Raycast(transform.position, orientation.right, 1, wallLayer);
        playerMovement.wallLeft = Physics.Raycast(transform.position, -orientation.right, 1, wallLayer);

        if (!playerMovement.wallRight && !playerMovement.wallLeft) StopWallRun();
    }
}
