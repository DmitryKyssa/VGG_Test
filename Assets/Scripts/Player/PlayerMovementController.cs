using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : Singleton<PlayerMovementController>
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 2.5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float upperLookLimit = 30f;
    [SerializeField] private float lowerLookLimit = 30f;

    private CharacterController characterController;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction inventoryAction;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 appliedMovement;
    private Vector2 currentLookInput;
    private float cameraRotationX = 0f;
    private bool isJumping = false;
    private bool isRunning = false;
    private float verticalVelocity = 0f;

    private readonly float gravity = Physics.gravity.y;

    public PlayerInput PlayerInput => playerInput;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
        inventoryAction = playerInput.actions["Inventory"];

        gameObject.SetTag(Tag.Player);
    }

    private void OnEnable()
    {
        moveAction.performed += OnMoveInput;
        moveAction.canceled += OnMoveInput;

        lookAction.performed += OnLookInput;
        lookAction.canceled += OnLookInput;

        jumpAction.performed += OnJumpInput;

        runAction.performed += OnRunInput;
        runAction.canceled += OnRunInput;

        inventoryAction.performed += ctx => InventorySystem.Instance.ToggleInventoryUI();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMoveInput;
        moveAction.canceled -= OnMoveInput;

        lookAction.performed -= OnLookInput;
        lookAction.canceled -= OnLookInput;

        jumpAction.performed -= OnJumpInput;

        runAction.performed -= OnRunInput;
        runAction.canceled -= OnRunInput;

        inventoryAction.performed -= ctx => InventorySystem.Instance.ToggleInventoryUI();
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        currentLookInput = context.ReadValue<Vector2>();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            isJumping = true;
        }
    }

    private void OnRunInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    private void HandleMovement()
    {
        float moveSpeed;

        if (isRunning)
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        Vector3 forward = transform.forward * currentMovementInput.y;
        Vector3 right = transform.right * currentMovementInput.x;
        currentMovement = (forward + right).normalized * moveSpeed;

        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f;

            if (isJumping)
            {
                verticalVelocity = jumpForce;
                isJumping = false;
            }
        }
        else
        {
            verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        appliedMovement = new Vector3(currentMovement.x, verticalVelocity, currentMovement.z);

        characterController.Move(appliedMovement * Time.deltaTime);
    }

    private void HandleLook()
    {
        transform.Rotate(Vector3.up, currentLookInput.x * mouseSensitivity);

        cameraRotationX -= currentLookInput.y * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -upperLookLimit, lowerLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }
}