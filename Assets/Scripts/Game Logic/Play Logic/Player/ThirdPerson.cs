using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class ThirdPerson : MonoBehaviour
{
    [Header("Script Control")]
    [SerializeField] private bool reInput;

    [Header("Variables")]
    public Transform orientation;
    public Transform Player;
    public Transform playerObj;
    [SerializeField] private Rigidbody rb;
    public float roationSpeed;

    [Header("Input")]
    private float horizontal;
    private float vertical;

    [Header("Rewired")]
    [SerializeField] private int playerId;
    private Player player;
    [SerializeField] private int playerActionId;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = ReInput.players.GetPlayer(playerId);
    }

    private void Start()
    {
        playerActionId = playerId - 1;
    }

    private void Update()
    {
        // Rotate Orientation based on Camera Position
        Vector3 viewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        orientation.forward = viewDirection.normalized;

        if (reInput)
        {
            RewiredInput();
        }else
        {
            UnityInput();
        }

        Vector3 inputDirection = orientation.forward * vertical + orientation.right * horizontal;
        
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
        horizontal = player.GetAxisRaw("Horizontal" + playerActionId.ToString());
        vertical = player.GetAxisRaw("Vertical" + playerActionId.ToString());
    }
}
}
