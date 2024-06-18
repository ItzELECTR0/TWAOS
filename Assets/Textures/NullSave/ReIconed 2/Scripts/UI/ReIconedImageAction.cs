using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave
{
    [RequireComponent(typeof(Image))]
    public class ReIconedImageAction : MonoBehaviour
    {

        #region Variables

        [Tooltip("Rewired Player Id")] public int playerId;
        [Tooltip("How should we lookup this action?")] public ReIconedActionLookup actionLookup;
        [Tooltip("Which controller do we prefer?")] public ReIconedUpdateType updateType;
        [Tooltip("Which Action should we monitor?")] public string actionName;
        [Tooltip("Which Action Id should we monitor?")] public int actionId;
        [Tooltip("What sprite should we display if we can't find the action?")] public Sprite notFoundSprite;

        private Image image;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Start()
        {
            ReInput.InputSourceUpdateEvent += UpdateUI;
            UpdateUI();
        }

        private void Reset()
        {
            playerId = 0;
            actionName = "Horizontal";
            notFoundSprite = null;
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            InputMap map = actionLookup == ReIconedActionLookup.Name ? ReIconed.GetActionHardwareInput(actionName, playerId, updateType) : ReIconed.GetActionHardwareInput(actionId, playerId, updateType);
            image.sprite = map == null ? notFoundSprite : map.unitySprite;
        }

        #endregion

    }
}