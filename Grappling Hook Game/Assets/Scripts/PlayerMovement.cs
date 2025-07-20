using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float groundDrag;
    [Tooltip("The max difference between movespeed and desired movespeed before the game decides to lerp")]
    [SerializeField] private float moveSpeedTolerance;
    private float moveSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float slopeIncreaseMultiplier;

    [Header("Jumping")]
    [SerializeField] private float jumpStrength;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airSpeedMultiplier;
    private bool canJump;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    private bool isOnGround;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Orientation")]
    [SerializeField] private Transform orientation;

    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;

    private Rigidbody rb;

    private MovementState state;
    public bool sliding;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        GetInput();
        //SpeedControl();
        StateHandler();

        if (isOnGround) rb.drag = groundDrag;
        else rb.drag = groundDrag * airSpeedMultiplier;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if (sliding)
        {
            state = MovementState.sliding;
            if (IsOnSlope() && rb.velocity.y < 0.1f) desiredMoveSpeed = slideSpeed;
            else desiredMoveSpeed = sprintSpeed;
        }
        else if (isOnGround && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (isOnGround && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (isOnGround)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > moveSpeedTolerance && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(LerpSpeed());
        }
        else moveSpeed = desiredMoveSpeed;

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && isOnGround)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(5 * Vector3.down, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (IsOnSlope()) rb.AddForce(10 * moveSpeed * GetSlopeMoveDirection(moveDirection), ForceMode.Force);
        else if (isOnGround) rb.AddForce(10 * moveSpeed * moveDirection.normalized, ForceMode.Force);
        else rb.AddForce(10 * moveSpeed * airSpeedMultiplier * moveDirection.normalized, ForceMode.Force);

        rb.useGravity = !IsOnSlope();
    }

    private void SpeedControl()
    {
        if (!exitingSlope && IsOnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        } 
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.y);
            }
        }
    }

    private IEnumerator LerpSpeed()
    {
        float time = 0;
        float diff = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float start = moveSpeed;

        while (time < diff)
        {
            moveSpeed = Mathf.Lerp(start, desiredMoveSpeed, time / diff);

            if (IsOnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else time += Time.deltaTime;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(jumpStrength * Vector3.up, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
    }

    public  bool IsOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return 0 < angle && angle < maxSlopeAngle;
        }
        return false;
    }

    public  Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
