using System;
using System.Collections;
using ELECTRIS;
using UnityEngine;
using Rewired;
using Unity.Mathematics;

// ELECTRO - 03/08/2024 21:32 - Dear future contributors, this code is hot garbage, please either bear with it until it's imroved, or improve it.
// ELECTRO - 06/o8/2024 21:31 - IT WORKS LET'S GOOOOOO (If you change it make sure it STILL works, or it is NOT getting merged)
// ELECTRO - 07/08/2024 19:11 - Adding sprinting and slope movement raaahhh
// ELECTRO - 07/08/2024 22:17 - You must NEVER disallow holdingBack. It's the law.
// ELECTRO - 08/08/2024 04:18 - Slope stuff kinda works but it's janky as fuck

namespace ELECTRIS
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Script Control")]
        [SerializeField] private bool allowHoldingBack = true;
        [SerializeField] private bool allowMovement = true;
        [SerializeField] private bool allowPlayerRotation = true;
        [SerializeField] private bool allowSpeedControl = true;
        [SerializeField] private bool allowSprint = true;
        [SerializeField] private bool allowJump = true;
        [SerializeField] private bool allowDebugging = true;
        [SerializeField] private bool reInput = true;

        [Header("Script Connectors")]
        public PauseManager pausectl;
        public PausedGame paused;

        [Header("Player")]
        public Transform playerObject;
        public Transform playerGraphics;
        private float turnSmoothVelocity;
        [SerializeField] private float rotationSmoothess = 0.1f;
        public Vector3 mDirection;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private bool readyToJump;
        [SerializeField] private bool grounded;
        [SerializeField] private bool combatMode = false;
        public bool isInside;
        public bool isOutside;

        // Detect if the player is on a slope
        [SerializeField] private bool isOnSlope()
        {
            if (Physics.Raycast(playerObject.position, Vector3.down, out slopeHit, playerObject.localScale.y * 0.7f + 0.3f))
            {
                angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle >= minimumSlopeAngle;
            }

            return false;
        }

        // Get the angle of the slope and it's direction
        [SerializeField] private Vector3 slopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(mDirection, slopeHit.normal).normalized;
        }

        [Header("Rewired")]
        public int playerId;
        public Player player;
        private Player systemPlayer;

        [Header("Input")]
        [SerializeField] private float horizontal;
        [SerializeField] private float vertical;

        [Header("Legacy Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Movement")]
        [HideInInspector] public float moveSpeed;
        public float walkSpeedSip;
        public float sprintSpeedSip;
        public float walkSpeedZip;
        public float sprintSpeedZip;
        public float walkSpeedNeon;
        public float sprintSpeedNeon;
        private float superSpeed;
        public float superMultiplier;
        public float speedMultiplier;
        public float gravityMultiplier;
        public float airMultiplier;
        [SerializeField] private float groundDrag;
        public float jumpForce;
        public float jumpCooldown;

        [Header("Slope Handling")]
        public float angle;
        public float minimumSlopeAngle;
        public float maxSlopeAngle;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        [Header("Physics Checking")]
        public Transform Checker;
        public float checkDistance = 0.2f;
        public LayerMask whatIsGround;
        public LayerMask whatIsInside;
        public LayerMask whatIsOutside;

        [Header("Player Camera Connectors")]
        [SerializeField] private Camera cam;
        [SerializeField] private Transform camTransform;

        public enum CurrentPlayer
        {
            Sip,
            Zip,
            Neon
        }

        public enum CurrentMoveState
        {
            Walking,
            Sprinting,
            Air
        }

        public CurrentPlayer currentPlayer;
        public CurrentMoveState moveState;

        void Awake()
        {
            // Initialize rewired by selecting a player
            player = ReInput.players.GetPlayer(playerId);
            systemPlayer = ReInput.players.GetSystemPlayer();

            // Subscribe to the OnGameStateChanged event to initialize pause system
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

            // Get the camera's transform
            camTransform = cam.transform;
            
            // Lock the cursor the center of the screen and make it invisible
            Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        }

        void OnDestroy()
        {
            // Unsusbscribe from the event in the case of the object being destroyed
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newGameState)
        {
            // Log the game state change to console
            Debug.Log(newGameState.ToString());
        }

        private void Start()
        {
            // State which character is selected as the current player.
            // This is a very simple implementation for future CO-OP plans.
            if (playerId == 0)
            {
                currentPlayer = CurrentPlayer.Sip;
            }else if (playerId == 1)
            {
                currentPlayer = CurrentPlayer.Zip;
            }else if (playerId == 2)
            {
                currentPlayer = CurrentPlayer.Neon;
            }

            // Set Walking state on start
            moveState = CurrentMoveState.Walking;

            // Ready to jump on game start
            readyToJump = true;

            // Freeze rotation of the Rigidbody component
            rb.freezeRotation = true;

            // Debug mode
            if (allowDebugging)
            {
                Debugging();
            }
        }

        // Get input using the Unity legacy method
        private void UnityInput()
        {
            // WASD Input
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            // Sprinting
            if (allowSprint && grounded && Input.GetKey(sprintKey))
            {
                moveState = CurrentMoveState.Sprinting;
            }else if (grounded)
            {
                moveState = CurrentMoveState.Walking;
            }else
            {
                moveState = CurrentMoveState.Air;
            }

            //Jump
            if (allowJump && Input.GetKeyDown(jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();

                // Apply jump cooldown
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        //Get input using Rewired
        private void RewiredInput()
        {
            // WASD Input
            horizontal = player.GetAxisRaw("Horizontal" + playerId.ToString());
            vertical = player.GetAxisRaw("Vertical" + playerId.ToString());

            // Sprinting
            if (allowSprint && grounded && player.GetButton("Sprint" + playerId.ToString()))
            {
                moveState = CurrentMoveState.Sprinting;
            }else if (grounded)
            {
                moveState = CurrentMoveState.Walking;
            }else
            {
                moveState = CurrentMoveState.Air;
            }

            //Jump
            if (allowJump && player.GetButtonDown("Jump" + playerId.ToString()) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();

                // Apply jump cooldown
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void Update()
        {
            // Decide which ability to enable based on the playing character
            if (currentPlayer == CurrentPlayer.Sip)
            {
                EnergyManipulation(allowHoldingBack);
            }else if (currentPlayer == CurrentPlayer.Zip)
            {
                HyperThinking();
            }else if (currentPlayer == CurrentPlayer.Neon)
            {
                ElectricUsage();
            }

            // Determine the player's current ground state
            grounded = Physics.CheckSphere(Checker.position, checkDistance, whatIsGround);
            isInside = Physics.CheckSphere(Checker.position, checkDistance, whatIsInside);
            isOutside = Physics.CheckSphere(Checker.position, checkDistance, whatIsOutside);

            // Decide which Input Method to use
            if (reInput)
            {
                RewiredInput();
            }else if (!reInput)
            {
                UnityInput();
            }

            // Control the player's speed gain
            if (allowSpeedControl)
            {
                SpeedControl();
            }

            // Apply drag to the player
            if (grounded)
            {
                rb.linearDamping = groundDrag;
            }else
            {
                rb.linearDamping = 0;
            }
        }

        private void FixedUpdate()
        {
            // Rotate the player based on movement input
            if (allowPlayerRotation)
            {
                PlayerRotation();
            }

            // Move the player
            if (allowMovement)
            {
                Movement();
            }
        }

        // Logic for rotating the player
        public void PlayerRotation()
        {
            // Calculate rotation based on input
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

            if (inputDirection.magnitude >= 0.1f)
            {
                // Calculate rotation based on input angle using Atan2
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;

                // Adjust targetAngle to account for texture displacement
                float targetAngleAdjusted = targetAngle - 90f;

                //Smooth out the calculated rotation
                float playerAngle = Mathf.SmoothDampAngle(playerGraphics.eulerAngles.y, targetAngleAdjusted, ref turnSmoothVelocity, rotationSmoothess);

                // Rotate the player based on calculated rotation
                playerGraphics.rotation = Quaternion.Euler(0f, playerAngle, 0f);

                // Calculating the movement direction based on target angle
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                mDirection = moveDirection.normalized;
            }else
            {
                // If no input, set mDirection to zero
                mDirection = Vector3.zero;
            }

            // Apply rotation to match Slope Angle | EXPERIMENTAL
            //if (isOnSlope())
            //{
                //float slopeAngleX = Mathf.SmoothDampAngle(playerGraphics.eulerAngles.x, angle, ref turnSmoothVelocity, rotationSmoothess);
                //float slopeAngleZ = Mathf.SmoothDampAngle(playerGraphics.eulerAngles.z, angle, ref turnSmoothVelocity, rotationSmoothess);
                //playerGraphics.rotation = Quaternion.Euler(slopeAngleX, playerGraphics.rotation.y, slopeAngleZ);
            //}
        }

        // Logic for player movement
        private void Movement()
        {
            // Slope handling
            if (isOnSlope() && !exitingSlope)
            {
                // Push the player up the slope
                rb.AddForce(slopeMoveDirection() * moveSpeed * speedMultiplier * 2, ForceMode.Force);

                if (rb.linearVelocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }

            // Ground movement
            if (grounded)
            {
                // Move the player
                rb.AddForce(mDirection.normalized * moveSpeed * speedMultiplier, ForceMode.Force);

            // Air movement
            }else if (!grounded)
            {
                // Move the player with air multiplier
                rb.AddForce(mDirection.normalized * moveSpeed * speedMultiplier * airMultiplier, ForceMode.Force);
            }

            rb.useGravity = !isOnSlope();
        }

        // Logic for controlling the player's speed
        private void SpeedControl()
        {
            // Limit Velocity on Slopes or Ground and in air
            if (isOnSlope() && !exitingSlope)
            {
                if (rb.linearVelocity.magnitude > moveSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
                }
            }else
            {
                // Calculate the flat velocity of the player
                Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

                // Limit Movement Velocity of the player
                if (flatVelocity.magnitude > moveSpeed)
                {
                    Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                    rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
                }
            }

            // Handle move speed based on movement state
            if (currentPlayer == CurrentPlayer.Sip && moveState == CurrentMoveState.Sprinting) // If Sip is sprinting
            {
                moveSpeed = sprintSpeedSip;
            }else if (currentPlayer == CurrentPlayer.Zip && moveState == CurrentMoveState.Sprinting) // If Zip is sprinting
            {
                moveSpeed = sprintSpeedZip;
            }else if (currentPlayer == CurrentPlayer.Neon && moveState == CurrentMoveState.Sprinting) // If Neon is sprinting
            {
                moveSpeed = sprintSpeedNeon;
            }else if (currentPlayer == CurrentPlayer.Sip && moveState == CurrentMoveState.Walking) // If Sip is walking
            {
                moveSpeed = walkSpeedSip;
            }else if (currentPlayer == CurrentPlayer.Zip && moveState == CurrentMoveState.Walking) // If Zip is walking
            {
                moveSpeed = walkSpeedZip;
            }else if (currentPlayer == CurrentPlayer.Neon && moveState == CurrentMoveState.Walking) // If Neon is walking
            {
                moveSpeed = walkSpeedNeon;
            }
        }

        // Logic for making the player jump
        private void Jump()
        {
            // Exit slopes on jump
            exitingSlope = true;

            // Reset Y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Make the player jump
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        // Make the player able to jump again
        private void ResetJump()
        {
            readyToJump = true;
            exitingSlope = false;
        }

        // Logic for Debug Mode
        private void Debugging()
        {
            // Display the current player ID
            Debug.Log("PlayerCtl Debug ID:" + playerId.ToString());
        }

        // Logic For Sip's powers
        private void EnergyManipulation(bool holdingBack)
        {
            // Establish Super Speed value
            if (holdingBack)
            {
                superSpeed = sprintSpeedSip * 2;
            }else if (!holdingBack)
            {
                superSpeed = sprintSpeedSip * 2 * superMultiplier;
            }
        }

        // Logic for Zip's powers
        private void HyperThinking()
        {

        }

        // Logic for Neon's powers
        private void ElectricUsage()
        {
            // Establish Super Speed value
            superSpeed = sprintSpeedNeon * 1.5f;
        }
    }
}