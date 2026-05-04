#if UNITY_EDITOR
using UnityEditor.Build;
#endif
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerMovement : MonoBehaviour
{
    
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    
    // Player Variables
    // [SerializeField] private Rigidbody rb; //contains the rigidbody of the player
    public CharacterController charController;
    private PlayerControlsInputs _input;
    private GameStateManager.GameState currentGameState;
    
    // ==== Movement ====
    [Title("Movement", "Variables used for the Player's movement mechanic.")]
    [SerializeField] private float moveSpeed = 5.0f; //speed coefficient
    [SerializeField] private float sprintSpeed = 7.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    private Vector3 inputDirection;
    private float speedOffset = 0.1f;
    private float playerTargetYaw;
    
    // Private calculation variables
    // (not set in the Inspector)
    private float _speed;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private const float _threshold = 0.01f;
    [SerializeField] private float turnSpeed = 120f;

    // ========== Jumping ==========
    [Space]
    [Title("Jump", "Variables used for the Player's jumping mechanic.")]
    
    // UNDER CONSTRUCTION
    // Working on jump functionality. -- Nikki
    
    public float jumpPower = 4.0f; // How strong the jump force is
    public int maxJumps = 1;
    public int jumpsRemaining;
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    [Space]
    [Title("Ground Check", "Variables used to perform ground checks for jumping.")]
    [SerializeField] private bool isGrounded;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    
    // Subscribe to events
    private void OnEnable()
    {
        PlayerGroundcast.groundCheck += GroundCheck;
        GameStateManager.transitionedToNewState += UpdateGameState;
        // PlayerGroundcast.groundCheck += JumpAndGravity;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        PlayerGroundcast.groundCheck -= GroundCheck;
        GameStateManager.transitionedToNewState += UpdateGameState;
        // PlayerGroundcast.groundCheck -= JumpAndGravity;
    }
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerControlsInputs>();

#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#endif
    }

    private void Start()
    {
        playerTargetYaw = transform.rotation.eulerAngles.y;
        
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        ConfigurePlayerOnGameState();
    }
    
    /// <summary>
    /// Updates the localized currentGameState variable to the new game state
    /// passed in by the GameStateManager. This allows the PlayerMovement
    /// script to know the current game state and adjust the Player accordingly.
    /// </summary>
    /// <param name="newGameState"></param>
    private void UpdateGameState(GameStateManager.GameState newGameState)
    {
        currentGameState = newGameState;
    }
    
    private void ConfigurePlayerOnGameState()
    {
        if (currentGameState == GameStateManager.GameState.Exploration)
        {
            Player.Instance.ToggleCharacterController(true);
            MoveCharacter();
            RotateCharacter();
            JumpAndGravity();
            
            charController.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            Player.Instance.ToggleCharacterController(false);
        }
    }
    
    private void MoveCharacter()
    {
        // Set the target speed depending on movement type (walking or sprinting)
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;
        
        // If there's no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;
        
        // Get the current horizontal speed (X, Z)
        float currentHorizontalSpeed = new Vector3(charController.velocity.x, 0.0f, charController.velocity.z).magnitude;
        
        // create a float input magnitude
        // float inputMagnitude = Mathf.Clamp01(_input.move.magnitude);
        float inputMagnitude = _input.move.magnitude;
        
        // Accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // Use Lerp() to smoothly interpolate between the current speed and the
            // target speed, based on the input magnitude and the speed change rate.
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            
            // Round speed to reduce jitter
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        
        // Normalize input direction
        inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        
        // If Move input is detected
        if (_input.move != Vector2.zero)
        {
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }
    }

    /// <summary>
    /// Rotates the Player based on the Look input. Specifically, it uses the
    /// Player's yaw values in a way similar to rotating a camera in a scene.
    /// These yaw values are first assigned in Start() via the Player's local
    /// Euler angles.
    /// </summary>
    private void RotateCharacter()
    {
        // If Look input is detected
        if (_input.look.sqrMagnitude >= _threshold)
        {
            playerTargetYaw += _input.look.x;
        }

        // Clamp rotation to limit it to 360 degrees
        playerTargetYaw = ClampAngle(playerTargetYaw, float.MinValue, float.MaxValue);

        // Rotate the Player
        transform.rotation = Quaternion.Euler(0.0f, playerTargetYaw, 0.0f);
    }
    
    /// <summary>
    /// Clamps the angle between a minimum and maximum value. Used to
    /// prevent the Player from rotating infinitely in one direction.
    /// </summary>
    /// <param name="lfAngle"></param>
    /// <param name="lfMin"></param>
    /// <param name="lfMax"></param>
    /// <returns></returns>
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    /// <summary>
    /// Checks if the Player is on the ground. Consistently works
    /// to update the isGrounded and jumpsRemaining variables
    /// Called at the end of every FixedUpdate().
    /// </summary>
    private void GroundCheck(bool grounded)
    {
        // If the Player is grounded
        if (grounded)
        {
            isGrounded = true; // Player is on the ground
            jumpsRemaining = maxJumps; // Reset the jumps
        }
        else
        {
            // Player is not on the ground
            isGrounded = false;
        }
    }
    
    
    // ========================
    // ========================
    // ========================
    

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;
            
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
}
