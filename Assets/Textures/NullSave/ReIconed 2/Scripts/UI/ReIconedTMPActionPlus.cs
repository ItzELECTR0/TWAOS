using Rewired;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NullSave
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ReIconedTMPActionPlus : MonoBehaviour
    {

        #region Variables

        [Tooltip("Rewired Player Id")] public int playerId = 0;
        [Tooltip("Which controller do we prefer?")] public ReIconedUpdateType updateType;
        [Tooltip("What text do we want to update with actions?"), TextArea(2, 5)] public string formatText = "Horizontal: {action:Horizontal}";
        [Tooltip("What text do we want to use if we can't find the action?"), TextArea(2, 5)] public string notFoundText = "<sprite=0>";
        [Tooltip("Allow sprite tinting?")] public bool tint = true;

        private TextMeshProUGUI tmpText;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            ReInput.InputSourceUpdateEvent += UpdateUI;
            UpdateUI();
        }

        private void Reset()
        {
            formatText = "Horizontal: {action:Horizontal}";
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

        private Dictionary<string, InputMap> GetMapping()
        {
            Dictionary<string, InputMap> result = new Dictionary<string, InputMap>();
            string outputText = formatText;
            string actionName;
            int i, e;

            while (true)
            {
                i = outputText.IndexOf("{action:");
                if (i < 0) break;
                e = outputText.IndexOf("}", i + 1);
                if (e < 0) e = outputText.Length;

                actionName = outputText.Substring(i + 8, e - i - 8);
                if (!result.ContainsKey(actionName))
                {
                    result.Add(actionName, ReIconed.GetActionHardwareInput(actionName, playerId, updateType));
                }

                outputText = outputText.Substring(0, i) + outputText.Substring(e + 1);
            }

            return result;
        }

        private void UpdateUI()
        {
            Dictionary<string, InputMap> map = GetMapping();

            string newText = formatText;
            InputMap entryMap;

            foreach (var entry in map)
            {
                entryMap = entry.Value;
                if (entryMap != null) tmpText.spriteAsset = entryMap.TMPSpriteAsset;
                newText = newText.Replace("{action:" + entry.Key + "}", entryMap == null ? notFoundText : "<sprite index=" + entryMap.tmpSpriteIndex + " tint=" + (tint ? "1" : "0") + ">");
            }

            tmpText.text = newText;
        }


        #endregion

    }
}