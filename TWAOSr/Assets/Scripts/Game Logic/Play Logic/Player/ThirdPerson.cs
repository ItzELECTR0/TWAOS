using System;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;
using Rewired;

// ELECTRO - 03/08/2024 21:34 - Dear future contributors, this code barely functions the way it's intended, please either bear with it until it's imroved, or improve it.

namespace ELECTRIS
{
    public class ThirdPerson : MonoBehaviour
{
    [Header("Script Control")]
    [SerializeField] private bool reInput;

    [Header("Script Connectors")]
    [SerializeField] private PlayerController playerCtl;

    [Header("Variables")]
    public Transform orientation;
    public Transform Player;
    public Transform playerObj;
    [SerializeField] private Rigidbody rb;
    public float roationSpeed;

    [Header("Adjustments")]
    public float xAdjustment;
    public float zAdjustment;

    [Header("Input")]
    private float horizontal;
    private float vertical;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCtl.player = ReInput.players.GetPlayer(playerCtl.playerId);
    }

    private void Update()
    {
        // Rotate Orientation based on Camera Position
        Vector3 viewDirection = Player.position - new Vector3(transform.position.x * xAdjustment, Player.position.y, transform.position.z * zAdjustment);
        orientation.forward = viewDirection.normalized;

        // Decide which Input Method to use
        if (reInput)
        {
            RewiredInput();
        }else if (!reInput)
        {
            UnityInput();
        }

        // Calculate the direction the player is trying to go
        Vector3 inputDirection = orientation.forward * vertical + orientation.right * horizontal;
        
        // Rotate the player object based on desired direction
        if (inputDirection !=  Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirection.normalized, Time.deltaTime * roationSpeed);
        }
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
}
}
