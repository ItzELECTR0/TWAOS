using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIManagerSplashScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        [SerializeField] private bool mobileMode;

        [Header("Resources")]
        [SerializeField] private TextMeshProUGUI PAKStart;
        [SerializeField] private TextMeshProUGUI PAKKey;
        [SerializeField] private TextMeshProUGUI PAKEnd;

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("Heat UI Manager"); }
            if (UIManagerAsset.enableDynamicUpdate == false) { UpdatePAK(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdatePAK(); }
            if (Application.isPlaying == true) { this.enabled = false; }
        }

        void AnalyzePAKText()
        {
            if (mobileMode == true)
                return;

            // Fetch text and process formatting
            string tempText = UIManagerAsset.pakText;
            string[] outMain = tempText.Split(char.Parse("{"));
            string outStart = outMain[0];

            // Apply the first part if available
            if (!string.IsNullOrEmpty(outStart) && PAKStart != null) { PAKStart.gameObject.SetActive(true); PAKStart.text = outStart.Substring(0, outStart.Length - 1); }
            else if (PAKStart != null) { PAKStart.gameObject.SetActive(false); }

            // If there is no key text available, return
            if (outMain.Length <= 1)
            {
                if (PAKKey != null) { PAKKey.transform.parent.gameObject.SetActive(false); }
                if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
                return;
            }

            // Check for PAK text
            string[] outPak = outMain[1].Split(new string[] { "}" }, System.StringSplitOptions.None);

            // Apply PAK Text part if available
            if (!string.IsNullOrEmpty(outPak[0].ToString()) && PAKKey != null) { PAKKey.transform.parent.gameObject.SetActive(true); PAKKey.text = outPak[0].ToString(); }
            else if (PAKKey != null) { PAKKey.transform.parent.gameObject.SetActive(false); }

            // If there is no end text available, return
            if (outPak.Length <= 1)
            {
                if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
                return;
            }

            // Apply the last part if available
            if (!string.IsNullOrEmpty(outPak[1].ToString()) && PAKEnd != null) { PAKEnd.gameObject.SetActive(true); PAKEnd.text = outPak[1].Substring(1, outPak[1].ToString().Length - 1).ToString(); }
            else if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
        }

        void AnalyzePAKLocalizationText()
        {
            if (Application.isPlaying == false || mobileMode == true)
                return;

            LocalizedObject localStart = PAKStart.GetComponent<LocalizedObject>();
            LocalizedObject localKey = PAKKey.GetComponent<LocalizedObject>();
            LocalizedObject localEnd = PAKEnd.GetComponent<LocalizedObject>();

            if (localStart == null || localKey == null || localEnd == null)
                return;

            // Fetch text and process formatting
            string tempText = UIManagerAsset.pakLocalizationText;
            string[] outMain = tempText.Split(char.Parse("{"));
            string outStart = outMain[0];

            // Apply the first part if available
            if (!string.IsNullOrEmpty(outStart) && PAKStart != null) 
            {
                outStart = outStart.Substring(0, outStart.Length - 1);
                outStart = localStart.GetKeyOutput(outStart);

                PAKStart.gameObject.SetActive(true); 
                PAKStart.text = outStart;

                LayoutRebuilder.ForceRebuildLayoutImmediate(PAKStart.transform.parent.GetComponent<RectTransform>());
            }
            else if (PAKStart != null) { PAKStart.gameObject.SetActive(false); }

            // If there is no key text available, return
            if (outMain.Length <= 1)
            {
                if (PAKKey != null) { PAKKey.transform.parent.gameObject.SetActive(false); }
                if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
                return;
            }

            // Check for PAK text
            string[] outPak = outMain[1].Split(new string[] { "}" }, System.StringSplitOptions.None);
            string outPakParsed = outPak[0].ToString();

            // Apply PAK Text part if available
            if (!string.IsNullOrEmpty(outPak[0].ToString()) && PAKKey != null) 
            {
                outPakParsed = localKey.GetKeyOutput(outPakParsed);

                PAKKey.transform.parent.gameObject.SetActive(true);
                PAKKey.text = outPakParsed;

                LayoutRebuilder.ForceRebuildLayoutImmediate(PAKKey.transform.parent.GetComponent<RectTransform>());
            }
            else if (PAKKey != null) { PAKKey.transform.parent.gameObject.SetActive(false); }

            // If there is no end text available, return
            if (outPak.Length <= 1)
            {
                if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
                return;
            }

            // Apply the last part if available
            if (!string.IsNullOrEmpty(outPak[1].ToString()) && PAKEnd != null) 
            {
                string outEndParsed = outPak[1].Substring(1, outPak[1].ToString().Length - 1).ToString();
                outEndParsed = localEnd.GetKeyOutput(outEndParsed);

                PAKEnd.gameObject.SetActive(true); 
                PAKEnd.text = outEndParsed;

                LayoutRebuilder.ForceRebuildLayoutImmediate(PAKEnd.transform.parent.GetComponent<RectTransform>());
            }
            else if (PAKEnd != null) { PAKEnd.gameObject.SetActive(false); }
        }

        void UpdatePAK()
        {
            if (UIManagerAsset.pakType == UIManager.PressAnyKeyTextType.Custom)
                return;

            if (UIManagerAsset.pakType == UIManager.PressAnyKeyTextType.Default && UIManagerAsset.enableLocalization == false) { AnalyzePAKText(); }
            else if (UIManagerAsset.pakType == UIManager.PressAnyKeyTextType.Default && UIManagerAsset.enableLocalization == true) { AnalyzePAKLocalizationText(); }
        }
    }
}