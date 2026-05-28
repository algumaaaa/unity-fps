using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    public InputActionAsset playerControls;

    private CharacterController characterController;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction shootAction;
    private InputAction aimAction;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 currentMovement;
    private Camera mainCamera;
    private Animator mainAnimator;

    public float mouseSensitivity = 1;
    public float sprintMultiplier = 2;
    public float walkSpeed = 1;
    public float jumpForce = 10;

    private const float GRAVITY = 10;
    private const float RAYCAST_RANGE = 100;
    private float verticalRotation = 0;
    private float ammo = 6;
    private bool isMoving = false;
    private bool isAiming = false;

    private void Awake()
    {
      //Cursor.lockState = CursorLockMode.Locked;
      moveAction = playerControls.FindActionMap("Player").FindAction("Move");
      lookAction = playerControls.FindActionMap("Player").FindAction("Look");
      jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");
      sprintAction = playerControls.FindActionMap("Player").FindAction("Sprint");
      shootAction = playerControls.FindActionMap("Player").FindAction("Attack");
      aimAction = playerControls.FindActionMap("Player").FindAction("Aim");

      moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
      moveAction.canceled += context => moveInput = Vector2.zero;
      lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
      lookAction.canceled += context => lookInput = Vector2.zero;
      shootAction.performed += context => HandleShoot();
      shootAction.canceled += context => mainAnimator.SetBool("isShooting", false);
      // TODO: can probably other inputs the same way?
    }

    private void Start()
    {
      characterController = GetComponent<CharacterController>();
      mainAnimator = GetComponentInChildren<Animator>();
      mainCamera = Camera.main;
      OnEnable();
    }

    private void OnEnable()
    {
      moveAction.Enable();
      lookAction.Enable();
      jumpAction.Enable();
      sprintAction.Enable();
      shootAction.Enable();
      aimAction.Enable();
    }

    private void OnDisable()
    {
      moveAction.Disable();
      lookAction.Disable();
      jumpAction.Disable();
      sprintAction.Disable();
    }

    private void ProcessMovement()
    {
      float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;
      float verticalSpeed = moveInput.y * walkSpeed * speedMultiplier;
      float horizontalSpeed = moveInput.x * walkSpeed * speedMultiplier;

      Vector3 horizontalMovement = new Vector3 (horizontalSpeed, 0, verticalSpeed);
      horizontalMovement = transform.rotation * horizontalMovement;

      ProcessGravityAndJumping();

      currentMovement.x = horizontalMovement.x;
      currentMovement.z = horizontalMovement.z;

      characterController.Move(currentMovement * Time.deltaTime);
      isMoving = moveInput.y != 0 || moveInput.x != 0;
    }

    private void ProcessGravityAndJumping()
    {
      if (characterController.isGrounded) {
        currentMovement.y = -.5f;
        
        if (jumpAction.triggered) {
          currentMovement.y = jumpForce;
        }
      }
      else {
        currentMovement.y -= GRAVITY * Time.deltaTime;
      }
    }

    private void HandleRotation()
    {
      float mouseXRotation = lookInput.x * mouseSensitivity;
      transform.Rotate(0, mouseXRotation, 0);

      verticalRotation -= lookInput.y * mouseSensitivity;
      verticalRotation = Mathf.Clamp(verticalRotation, -85, 85);
      mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleShoot()
    {
      if (ammo == 0 || !isAiming) return;
      mainAnimator.SetBool("isShooting", true);
      ammo--;
      RaycastHit hitInfo;
      if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitInfo)) {
        Debug.Log(hitInfo);
        Debug.Log(ammo);
      }
    }

    private void HandleAim()
    {
      if (aimAction.IsPressed()) {
        mainAnimator.SetFloat("isAiming", 1);
        isAiming = true;
      }
      else {
        mainAnimator.SetFloat("isAiming", -1);
        isAiming = false;
      }
    }

    private void Update()
    {
      ProcessMovement();
      HandleRotation();
      //HandleShoot();
      HandleAim();
    }
}
