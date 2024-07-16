// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    public class VibrationTest : UnityEngine.MonoBehaviour {

        public int playerId;
        public float vibrationIncrement = 0.1f;

        private float[] motors = new float[2];

        private static readonly string[] action_motors = new string[] { "VibrationMotor0", "VibrationMotor1" };
        private static readonly string action_stop = "StopVibration";

        Player player {
            get {
                return ReInput.players.GetPlayer(playerId);
            }
        }

        void Update() {

            for(int i = 0; i < action_motors.Length; i++) {
                if (player.GetButtonDown(action_motors[i])) {
                    SetVibration(i, UnityEngine.Mathf.Clamp01(motors[i] + vibrationIncrement));
                }
                if (player.GetNegativeButtonDown(action_motors[i])) {
                    SetVibration(i, UnityEngine.Mathf.Clamp01(motors[i] - vibrationIncrement));
                }
            }
            
            if (player.GetButtonDown(action_stop)) {
                StopVibration();
            }
        }

        void StopVibration() {
            player.StopVibration();
            System.Array.Clear(motors, 0, motors.Length);
        }

        void SetVibration(int motorIndex, float value) {
            motors[motorIndex] = value;
            player.SetVibration(motorIndex, motors[motorIndex]);
        }
    }
}
