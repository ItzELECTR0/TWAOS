using System;
using System.Collections;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class PlayerConroller : MonoBehaviour
    {
        [Header("Script Options")]
        public bool UseRewired;

        [Header("Player")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Vector3 mDirection;
        [SerializeField] private Rigidbody rb;

        [Header("Rewired")]
        [SerializeField] private int playerId;
        private Player player;
        private Player systemPlayer;

        [Header("Movement")]
        public float mSpeed;
        public float speedMultiplier;
        private float horizontal;
        private float vertical;

        [Header("Variables")]
        private bool readyToJump;

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

        private void Start()
        {
            readyToJump = true;
            rb.freezeRotation = true;
        }

        private void Update()
        {
            if (UseRewired)
            {
                RewiredInput();
            }else
            {
                UnityInput();
            }
        }

        private void FixedUpdate()
        {
            Movement();
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
    }
}