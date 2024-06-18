using Rewired;
using TMPro;
using UnityEngine;

namespace NullSave
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ReIconedTMPAction : MonoBehaviour
    {

        #region Variables

        [Tooltip("Rewired Player Id")] public int playerId;
        [Tooltip("How should we lookup this action?")] public ReIconedActionLookup actionLookup;
        [Tooltip("Which controller do we prefer?")] public ReIconedUpdateType updateType;
        [Tooltip("Which Action should we monitor?")] public string actionName;
        [Tooltip("Which Action Id should we monitor?")] public int actionId;
        [Tooltip("What text do we want to update with the action?"), TextArea(2, 5)] public string formatText;
        [Tooltip("What text do we want to use if we can't find the action?"), TextArea(2, 5)] public string notFoundText;
        [Tooltip("Allow sprite tinting?")] public bool tint;

        private TextMeshProUGUI tmpText;
        private Controller lastActive;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            switch (updateType)
            {
                case ReIconedUpdateType.ActiveInput:
                    ReInput.InputSourceUpdateEvent += UpdateUI;
                    break;
                case ReIconedUpdateType.PreferController:
                case ReIconedUpdateType.PreferKeyboard:
                    ReInput.ControllerConnectedEvent += ControllerChanged;
                    ReInput.ControllerDisconnectedEvent += ControllerChanged;
                    break;
            }

            UpdateUI();
        }

        private void Reset()
        {
            playerId = 0;
            actionName = "Horizontal";
            actionId = 0;
            formatText = "Horizontal: {action}";
            notFoundText = "<sprite=0>";
            tint = true;
        }

        #endregion

        #region Public Methods

        public void SetFormatText(string value)
        {
            formatText = value;
            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void ControllerChanged(ControllerStatusChangedEventArgs args)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            Controller activeController = ReIconed.GetActiveController(playerId);
            if (lastActive != null && lastActive == activeController) return;

            lastActive = activeController;
            InputMap map = actionLookup == ReIconedActionLookup.Name ? ReIconed.GetActionHardwareInput(actionName, playerId, updateType) : ReIconed.GetActionHardwareInput(actionId, playerId, updateType);

            if (map == null)
            {
                tmpText.spriteAsset = null;
                tmpText.text = formatText.Replace("{action}", notFoundText);
            }
            else
            {
                tmpText.spriteAsset = map.TMPSpriteAsset;
                tmpText.text = formatText.Replace("{action}", "<sprite=" + map.tmpSpriteIndex + " tint=" + (tint ? "1" : "0") + ">");
            }
        }

        #endregion

    }
}