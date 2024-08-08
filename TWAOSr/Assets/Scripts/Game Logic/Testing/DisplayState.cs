using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ELECTRO - 07/08/2024 21:10 - Just a testing script for debugging. Ignore.

namespace ELECTRIS
{
    public class DisplayState : MonoBehaviour
    {
        public PlayerController playerCtl;
        public TMP_Text overlayPlayerText;
        public TMP_Text overlayMoveText;
        public TMP_Text overlaySpeedText;
        public TMP_Text overlaySlopeText;

        void Update()
        {
            overlayPlayerText.text = "Current Player: " + playerCtl.currentPlayer.ToString();
            overlayMoveText.text = "Current Movement State: " + playerCtl.moveState.ToString();
            overlaySpeedText.text = "Current Speed: " + playerCtl.mDirection.magnitude.ToString();
            overlaySlopeText.text = "Current Slope Angle: " + playerCtl.angle.ToString();
        } 
    }
}