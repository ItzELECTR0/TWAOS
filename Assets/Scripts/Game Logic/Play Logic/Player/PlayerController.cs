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
        [SerializeField] private bool combatMode;
        [SerializeField] private bool reInput = false;

        [Header("Script Connecors")]
        public PauseController pausectl;
        public PausedGame paused;

        [Header("Movement")]
        [SerializeField] private float speed = 6;
        public float walkSpeed = 6;
        public float sprintSpeed = 8;
        public float speedMultiplier;
        public float airMultiplier;
        public float groundDrag;
        public float jumpForce;
        public float jumpCooldown;
        public Transform orientation;
        Vector3 moveDir;

        [Header("Legacy Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Player")]
        public bool sprinting = false;
        public bool air = false;
        private bool grounded;
        public bool isInside;
        public bool isOutside;
        bool readyToJump;
        public Rigidbody rb;
        [SerializeField] private Player player;
        [SerializeField] private int playerID = 1;

        [Header("Physics Checking")]
        public Transform Checker;
        public float checkDistance = 0.2f;
        public LayerMask whatIsGround;
        public LayerMask whatIsInside;
        public LayerMask whatIsOutside;

        [Header("Slope Physics")]
        public float maxSlopeAngle;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        [Header("Input")]
        private float horizontal;
        private float vertical;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            if (!combatMode)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }
            else if (combatMode)
            {
                GameStateManager.Instance.SetState(GameState.Combat);
            }

            player = ReInput.players.GetPlayer(playerID);
            rb.freezeRotation = true;
            readyToJump = true;
        }

        void Update ()
        {
            grounded = Physics.CheckSphere(Checker.position, checkDistance, whatIsGround);
            isInside = Physics.CheckSphere(Checker.position, checkDistance, whatIsInside);
            isOutside = Physics.CheckSphere(Checker.position, checkDistance, whatIsOutside);

            if (grounded)
            {
                rb.linearDamping = groundDrag;
            }
            else if (!grounded)
            {
                rb.linearDamping = 0;
            }

            if (reInput)
            {
                RewiredInput();
            }
            else if (!reInput)
            {
                DefaultInput();
            }

            StateControl();
            SpeedControl();
        }

        private void FixedUpdate()
        {
            if (allowMovement)
            {
                MovePlayer();
            }
            else if (!allowMovement)
            {
                return;
            }
        }

        private void DefaultInput()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (allowJump)
            {
                if (Input.GetKey(jumpKey) && readyToJump && grounded)
                {
                    readyToJump = false;
                    Jump();
                    Invoke(nameof(ResetJump), jumpCooldown);
                }
            }
            else if (!allowJump)
            {
                return;
            }
        }

        private void RewiredInput()
        {
            horizontal = player.GetAxisRaw("Horizontal");
            vertical = player.GetAxisRaw("Vertical");

            if (allowJump)
            {
                if (player.GetButton("Jump") && readyToJump && grounded)
                {
                    readyToJump = false;
                    Jump();
                    Invoke(nameof(ResetJump), jumpCooldown);
                }
            }
            else if (!allowJump)
            {
                return;
            }
        }

        private void StateControl()
        {
            if (allowSprint)
            {
                if (reInput)
                {
                    if (grounded && player.GetButton("Sprint"))
                    {
                        sprinting = true;
                        speed = sprintSpeed;
                    }
                    else if (grounded)
                    {
                        sprinting = false;
                        speed = walkSpeed;
                    }
                    else if (!grounded)
                    {
                        sprinting = false;
                        air = true;
                    }
                    else if (!grounded && player.GetButton("Sprint"))
                    {
                        sprinting = true;
                        air = true;
                    }
                }
                else if (!reInput)
                {
                    if (grounded && Input.GetKey(sprintKey))
                    {
                        sprinting = true;
                        speed = sprintSpeed;
                    }
                    else if (grounded)
                    {
                        sprinting = false;
                        speed = walkSpeed;
                    }
                    else if (!grounded)
                    {
                        sprinting = false;
                        air = true;
                        speed = walkSpeed;
                    }
                    else if (!grounded && player.GetButton("Sprint"))
                    {
                        sprinting = true;
                        air = true;
                        speed = sprintSpeed;
                    }

                    rb.useGravity = !OnSlope();
                }
            }
            else if (!allowSprint)
            {
                return;
            }
        }

        private void MovePlayer()
        {
            moveDir = orientation.forward * vertical + orientation.right * horizontal;

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDir() * speed * speedMultiplier, ForceMode.Force);

                if (rb.linearVelocity.y > 0 || rb.angularVelocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }

            if (grounded)
            {
                rb.AddForce(moveDir.normalized * speed * speedMultiplier, ForceMode.Force);
            }
            else if (!grounded)
            {
                rb.AddForce(moveDir.normalized * speed * speedMultiplier * airMultiplier, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            if (!allowSlide)
            {
                if (OnSlope() && !exitingSlope)
                {
                    if (rb.linearVelocity.magnitude > speed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * speed;
                    }
                }
                else
                {
                    Vector3 flatLinearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

                    if (flatLinearVelocity.magnitude > speed)
                    {
                        Vector3 limitedLinearVelocity = flatLinearVelocity.normalized * speed;
                        rb.linearVelocity = new Vector3(limitedLinearVelocity.x, rb.linearVelocity.y, limitedLinearVelocity.z);
                    }
                }
            }
            else if (allowSlide)
            {
                return;
            }
        }

        // WHY THIS SHIT BREAKING THE WHOLE SYSTEM?
        private void Jump()
        {
            exitingSlope = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            readyToJump = true;
            exitingSlope = false;
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1f * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private Vector3 GetSlopeMoveDir()
        {
            return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
        }
    }
}