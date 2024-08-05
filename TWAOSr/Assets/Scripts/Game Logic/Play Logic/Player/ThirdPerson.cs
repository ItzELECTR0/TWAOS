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
            player = ReInput.players.GetPlayer(playerCtl.playerId);
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
            horizontal = playerCtl.player.GetAxisRaw("Horizontal" + playerCtl.playerId.ToString());
            vertical = playerCtl.player.GetAxisRaw("Vertical" + playerCtl.playerId.ToString());
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
    }
}
