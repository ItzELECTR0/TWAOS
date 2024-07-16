using System;
using System.Collections;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class PlayerConroller : MonoBehaviour
    {
        [Header("Script Control")]
        [SerializeField] private bool allowMovement = true;
        [SerializeField] private bool allowSprint = false;
        [SerializeField] private bool allowJump = false;
        [SerializeField] private bool allowSlide = false;
        [SerializeField] private bool combatMode = false;
        [SerializeField] private bool reInput = false;

        [Header("Script Connecors")]
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
        [SerializeField] private int playerId;
        private Player player;
        private Player systemPlayer;

        [Header("Legacy Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Movement")]
        public float mSpeed;
        public float speedMultiplier;
        [SerializeField] private float groundDrag;
        private float horizontal;
        private float vertical;

        [Header("Physics Checking")]
        public Transform Checker;
        public float checkDistance = 0.2f;
        public LayerMask whatIsGround;
        public LayerMask whatIsInside;
        public LayerMask whatIsOutside;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        }

        private void Update()
        {
            grounded = Physics.CheckSphere(Checker.position, checkDistance, whatIsGround);
            isInside = Physics.CheckSphere(Checker.position, checkDistance, whatIsInside);
            isOutside = Physics.CheckSphere(Checker.position, checkDistance, whatIsOutside);

            // Input Method
            if (reInput)
            {
                RewiredInput();
            }else
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
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        private void RewiredInput()
        {
            horizontal = player.GetAxisRaw("Horizontal");
            vertical = player.GetAxisRaw("Vertical");
        }

        private void Movement()
        {
            // Calculating the movement direction
            mDirection = orientation.forward * vertical + orientation.right * horizontal;

            // Move the player
            rb.AddForce(mDirection.normalized * mSpeed * speedMultiplier, ForceMode.Force);
        }

        private void SpeedControl()
        {
            // Calculate the flat velocity of the player
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Limit Movement Velocity of the player
            if (flatVelocity.magnitude > mSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * mSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
    }
}