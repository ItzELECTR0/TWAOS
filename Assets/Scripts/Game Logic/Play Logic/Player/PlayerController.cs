using System;
using System.Collections;
using ELECTRIS;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Script Control")]
        [SerializeField] private bool allowMovement = true;
        [SerializeField] private bool allowSprint = false;
        [SerializeField] private bool allowJump = false;
        [SerializeField] private bool allowSlide = false;
        [SerializeField] private bool allowDebugging = false;
        [SerializeField] private bool combatMode = false;
        [SerializeField] private bool reInput = false;

        [Header("Script Connectors")]
        [SerializeField] private ThirdPerson tpsCtl;
        public PauseController pausectl;
        public PausedGame paused;

        [Header("Player")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Vector3 mDirection;
        [SerializeField] private Rigidbody rb;
        private bool readyToJump;
        private bool grounded;
        public bool isInside;
        public bool isOutside;

        [Header("Rewired")]
        public int playerId;
        public Player player;
        private Player systemPlayer;

        [Header("Input")]
        private float horizontal;
        private float vertical;

        [Header("Legacy Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Movement")]
        public float walkSpeed;
        public float speedMultiplier;
        [SerializeField] private float groundDrag;
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;

        [Header("Physics Checking")]
        public Transform Checker;
        public float checkDistance = 0.2f;
        public LayerMask whatIsGround;
        public LayerMask whatIsInside;
        public LayerMask whatIsOutside;

        void Awake()
        {
            player = ReInput.players.GetPlayer(playerId);
            systemPlayer = ReInput.players.GetSystemPlayer();
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        void OnDestroy()
        {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newGameState)
        {
            Debug.Log(newGameState.ToString());
        }

        private void Start()
        {
            readyToJump = true;
            rb.freezeRotation = true;

            if (allowDebugging)
            {
                Debugging();
            }
        }

        private void Update()
        {
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

            SpeedControl();

            // Drag
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
            if (allowMovement)
            {
                Movement();
            }
        }

        private void UnityInput()
        {
            // WASD Input
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            //Jump
            if (allowJump && Input.GetKey(jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void RewiredInput()
        {
            // WASD Input
            horizontal = player.GetAxisRaw("Horizontal" + playerId.ToString());
            vertical = player.GetAxisRaw("Vertical" + playerId.ToString());

            //Jump
            if (allowJump && player.GetButton("Jump" + playerId.ToString()) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void Movement()
        {
            // Calculating the movement direction
            mDirection = orientation.forward * vertical + orientation.right * horizontal;

            // Ground movement
            if (grounded)
            {
                // Move the player
                rb.AddForce(mDirection.normalized * walkSpeed * speedMultiplier, ForceMode.Force);
            }else if (!grounded)
            {
                // Move the player
                rb.AddForce(mDirection.normalized * walkSpeed * speedMultiplier * airMultiplier, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            // Calculate the flat velocity of the player
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Limit Movement Velocity of the player
            if (flatVelocity.magnitude > walkSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * walkSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }

        private void Jump()
        {
            // Reset Y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Make the player jump
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        private void Debugging()
        {
            Debug.Log("PlayerCtl Debug" + playerId.ToString());
        }
    }
}