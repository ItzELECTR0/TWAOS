// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

// This is a template to use to allow you to use exported Rewired constants so you can configure Players and Actions in the
// inspector of the Cinemachine input axis using drop-down menus instead of tying strings.
// Copy this script to a location outside of the Rewired folder, then uncomment the script and modify the 3 items below to use it.
// See the documentation on exporting constants if you don't know how to do it.
//
// Also, you should copy the Editor/EditorScripts.cs file to a location outside the Rewired folder and place it into a
// folder named "Editor". Then follow the instructions on that script as well.

/*

namespace YOURNAMESPACE { // 1. Change this to a namespace for your project
    
    using CustomRewiredActionConstants = YOURNAMESPACE.Consts.Action; // 2. Change the right hand side to your exported Rewired Actions class.
    using CustomRewiredPlayerConstants = YOURNAMESPACE.Consts.Player; // 3. Change the right hand side to your exported Rewired Players class.

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // DO NOT EDIT ANYTHING BELOW THIS LINE UNLESS YOU WANT TO RENAME THE CLASS //////////////////////////////////////////////////////////////
    // IF YOU RENAME THE CLASS, BE SURE TO ALSO RENAME IT IN THE EDITOR SCRIPT AS WELL ///////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    using System;
    using Rewired.Integration.Cinemachine3;

    [UnityEngine.ExecuteAlways]
    [Unity.Cinemachine.SaveDuringPlay]
    public class CustomRewiredCinemachineInputAxisController : RewiredCinemachineInputAxisControllerBase<CustomRewiredCinemachineInputAxisController.Reader> {

        [UnityEngine.Tooltip("Rewired Player id. ")]
        [Rewired.ActionIdProperty(typeof(CustomRewiredPlayerConstants))]
        [UnityEngine.SerializeField]
        private int _playerId = 0;

        /// <summary>
        /// Rewired Player id.
        /// </summary>
        public int playerId { get { return _playerId; } set { _playerId = value; } }

        protected override void Reset() {
            base.Reset();
            _playerId = 0;
        }

        public override int GetPlayerId() {
            return _playerId;
        }

        [Serializable]
        public class Reader : RewiredCinemachineInputAxisControllerReaderBase {

            [UnityEngine.Tooltip("Rewired Action id.")]
            [Rewired.ActionIdProperty(typeof(CustomRewiredActionConstants))]
            [UnityEngine.SerializeField]
            private int _actionId;

            /// <summary>
            /// Rewired Action id.
            /// </summary>
            public int actionId { get { return _actionId; } set { _actionId = value; } }

            public override int GetActionId() {
                return _actionId;
            }
        }
    }
}

*/
