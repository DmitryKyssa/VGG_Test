using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : Singleton<PlayerMovementController>
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 2.5f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float upperLookLimit = 30f;
    [SerializeField] private float lowerLookLimit = 30f;

    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private Vector3 standingCameraPosition = new Vector3(0, 0.8f, 0);
    [SerializeField] private Vector3 crouchingCameraPosition = new Vector3(0, 0.4f, 0);

    private CharacterController characterController;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction runAction;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private Vector3 appliedMovement;
    private Vector2 currentLookInput;
    private float cameraRotationX = 0f;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isRunning = false;
    private float verticalVelocity = 0f;
    private float targetHeight;
    private Vector3 targetCameraPosition;

    private readonly float gravity = Physics.gravity.y;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
        runAction = playerInput.actions["Run"];

        targetHeight = standingHeight;
        targetCameraPosition = standingCameraPosition;

        gameObject.SetTag(Tag.Player);
    }

    private void OnEnable()
    {
        moveAction.performed += OnMoveInput;
        moveAction.canceled += OnMoveInput;

        lookAction.performed += OnLookInput;
        lookAction.canceled += OnLookInput;

        jumpAction.performed += OnJumpInput;

        crouchAction.performed += OnCrouchInput;
        crouchAction.canceled += OnCrouchInput;

        runAction.performed += OnRunInput;
        runAction.canceled += OnRunInput;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMoveInput;
        moveAction.canceled -= OnMoveInput;

        lookAction.performed -= OnLookInput;
        lookAction.canceled -= OnLookInput;

        jumpAction.performed -= OnJumpInput;

        crouchAction.performed -= OnCrouchInput;
        crouchAction.canceled -= OnCrouchInput;

        runAction.performed -= OnRunInput;
        runAction.canceled -= OnRunInput;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleCrouching();
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

    private void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            targetHeight = crouchHeight;
            targetCameraPosition = crouchingCameraPosition;
        }
        else if (context.canceled)
        {
            if (!Physics.Raycast(transform.position, Vector3.up, standingHeight))
            {
                isCrouching = false;
                targetHeight = standingHeight;
                targetCameraPosition = standingCameraPosition;
            }
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
        if (isCrouching)
        {
            moveSpeed = crouchSpeed;
        }
        else
        {
            if (isRunning)
            {
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
        }

        Vector3 forward = transform.forward * currentMovementInput.y;
        Vector3 right = transform.right * currentMovementInput.x;
        currentMovement = (forward + right).normalized * moveSpeed;

        if (isRunning && !isCrouching && currentMovementInput != Vector2.zero)
        {
            currentRunMovement = currentMovement;
        }
        else
        {
            currentRunMovement = Vector3.zero;
        }

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

    private void HandleCrouching()
    {
        if (characterController.height != targetHeight)
        {
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

            float heightDifference = standingHeight - characterController.height;
            characterController.center = new Vector3(0, -heightDifference / 2f, 0);
        }

        if (cameraTransform.localPosition != targetCameraPosition)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCameraPosition, crouchTransitionSpeed * Time.deltaTime);
        }
    }
}