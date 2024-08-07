using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ELECTRO - 07/08/2024 21:10 - Just a testing script for debugging. Ignore.

namespace ELECTRIS
{
    public class DisplayState : MonoBehaviour
    {
        public TMP_Text overlayPlayerText;
        public TMP_Text overlayMoveText;
        public PlayerController playerCtl;

        void Update()
        {
            overlayPlayerText.text = "Current Player: " + playerCtl.currentPlayer.ToString();
            overlayMoveText.text = "Current Movement State: " + playerCtl.moveState.ToString();
        } 
    }
}