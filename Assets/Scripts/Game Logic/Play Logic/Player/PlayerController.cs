using System;
using System.Collections;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class PlayerConroller : MonoBehaviour
    {
        [Header("Player")]
        private Transform orientation;
        private Vector3 mDirection;
        private Rigidbody rb;

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

        private void Awake()
        {
            player = ReInput.players.GetPlayer(playerId);
            systemPlayer = ReInput.players.GetSystemPlayer();
        }

        private void Start()
        {
            readyToJump = true;
            rb.freezeRotation = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            RewiredInput();
        }

        private void FixedUpdate()
        {
            Movement();
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