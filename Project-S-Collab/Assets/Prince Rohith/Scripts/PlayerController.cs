using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float depthSpeed = 4f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody rb;
    private InputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        float speed = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
        animator.SetFloat("Speed", speed);

        //TODO: Animation not added need to be added
        //animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
    }

    void Move()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveInput.x * moveSpeed;
        velocity.z = moveInput.y * depthSpeed;

        rb.linearVelocity = velocity;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        //TODO: Animation not added need to be added
        //animator.SetTrigger("Jump");
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }
}