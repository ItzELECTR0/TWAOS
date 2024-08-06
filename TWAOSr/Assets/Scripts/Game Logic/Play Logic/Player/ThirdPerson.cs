using System;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;
using Rewired;

// ELECTRO - 03/08/2024 21:34 - Dear future contributors, this code barely functions the way it's intended, please either bear with it until it's imroved, or improve it.
// ELECTRO - 05/08/2024 23:36 - Fighting the burnout with a mighty will, all I manage to do is delete everything after realizing it makes things worse.

namespace ELECTRIS
{
    public class ThirdPerson : MonoBehaviour
    {
        [Header("Script Control")]
        [SerializeField] private bool reInput;

        [Header("Script Connectors")]
        [SerializeField] private PlayerController playerCtl;

        [Header("Rewired")]
        [SerializeField] private int playerId;
        private Player player;

        [Header("Input")]
        [SerializeField] private float horizontal;
        [SerializeField] private float vertical;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerId = playerCtl.playerId;
            player = ReInput.players.GetPlayer(playerId);
        }

        private void UnityInput()
        {
            // WASD Input
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        private void RewiredInput()
        {
            // WASD Input
            horizontal = playerCtl.player.GetAxisRaw("Horizontal");
            vertical = playerCtl.player.GetAxisRaw("Vertical");
        }

        private void Update()
        {
            // Decide which Input Method to use
            if (reInput)
            {
                RewiredInput();
            }else if (!reInput)
            {
                UnityInput();
            }
        }

        public void PlayerRotation()
        {
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            playerCtl.character.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
}
