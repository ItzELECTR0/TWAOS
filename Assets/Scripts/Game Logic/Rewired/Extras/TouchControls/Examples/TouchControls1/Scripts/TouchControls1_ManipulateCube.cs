// Copyright (c) 2017 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace Rewired.Demos {

    [AddComponentMenu("")]
    public class TouchControls1_ManipulateCube: MonoBehaviour {

        public float rotateSpeed = 1f;
        public float moveSpeed = 1f;

        private int currentColorIndex = 0;
        private Color[] colors = new Color[] {
            Color.white,
            Color.red,
            Color.green,
            Color.blue
        };

        private void OnEnable() {
            if(!ReInput.isReady) return;

            Player player = ReInput.players.GetPlayer(0);
            if(player == null) return;

            // Subscribe to input events
            player.AddInputEventDelegate(OnMoveReceivedX, UpdateLoopType.Update, InputActionEventType.AxisActive, "Horizontal");
            player.AddInputEventDelegate(OnMoveReceivedX, UpdateLoopType.Update, InputActionEventType.AxisInactive, "Horizontal");
            player.AddInputEventDelegate(OnMoveReceivedY, UpdateLoopType.Update, InputActionEventType.AxisActive, "Vertical");
            player.AddInputEventDelegate(OnMoveReceivedY, UpdateLoopType.Update, InputActionEventType.AxisInactive, "Vertical");
            player.AddInputEventDelegate(OnCycleColor, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "CycleColor");
            player.AddInputEventDelegate(OnCycleColorReverse, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "CycleColorReverse");
            player.AddInputEventDelegate(OnRotationReceivedX, UpdateLoopType.Update, InputActionEventType.AxisActive, "RotateHorizontal");
            player.AddInputEventDelegate(OnRotationReceivedX, UpdateLoopType.Update, InputActionEventType.AxisInactive, "RotateHorizontal");
            player.AddInputEventDelegate(OnRotationReceivedY, UpdateLoopType.Update, InputActionEventType.AxisActive, "RotateVertical");
            player.AddInputEventDelegate(OnRotationReceivedY, UpdateLoopType.Update, InputActionEventType.AxisInactive, "RotateVertical");
        }

        private void OnDisable() {
            if(!ReInput.isReady) return;

            Player player = ReInput.players.GetPlayer(0);
            if(player == null) return;

            // Unsubscribe from input events
            player.RemoveInputEventDelegate(OnMoveReceivedX);
            player.RemoveInputEventDelegate(OnMoveReceivedY);
            player.RemoveInputEventDelegate(OnCycleColor);
            player.RemoveInputEventDelegate(OnCycleColorReverse);
            player.RemoveInputEventDelegate(OnRotationReceivedX);
            player.RemoveInputEventDelegate(OnRotationReceivedY);
        }

        private void OnMoveReceivedX(InputActionEventData data) {
            OnMoveReceived(new Vector2(data.GetAxis(), 0f));
        }

        private void OnMoveReceivedY(InputActionEventData data) {
            OnMoveReceived(new Vector2(0f, data.GetAxis()));
        }

        private void OnRotationReceivedX(InputActionEventData data) {
            OnRotationReceived(new Vector2(data.GetAxis(), 0f));
        }

        private void OnRotationReceivedY(InputActionEventData data) {
            OnRotationReceived(new Vector2(0f, data.GetAxis()));
        }

        private void OnCycleColor(InputActionEventData data) {
            OnCycleColor();
        }

        private void OnCycleColorReverse(InputActionEventData data) {
            OnCycleColorReverse();
        }

        private void OnMoveReceived(Vector2 move) {
            transform.Translate((Vector3)move * Time.deltaTime * moveSpeed, Space.World);
        }

        private void OnRotationReceived(Vector2 rotate) {
            rotate *= rotateSpeed;
            transform.Rotate(Vector3.up, -rotate.x, Space.World);
            transform.Rotate(Vector3.right, rotate.y, Space.World);
        }

        private void OnCycleColor() {
            if(colors.Length == 0) return;
            currentColorIndex++;
            if(currentColorIndex >= colors.Length) currentColorIndex = 0;
            GetComponent<Renderer>().material.color = colors[currentColorIndex];
        }

        private void OnCycleColorReverse() {
            if(colors.Length == 0) return;
            currentColorIndex--;
            if(currentColorIndex < 0) currentColorIndex = colors.Length - 1;
            GetComponent<Renderer>().material.color = colors[currentColorIndex];
        }
    }
}