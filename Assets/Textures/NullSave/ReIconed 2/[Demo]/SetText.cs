using TMPro;
using UnityEngine;

namespace NullSave
{
    public class SetText : MonoBehaviour
    {

        public TMP_InputField textSource;
        public ReIconedTMPActionPlus destination;

        public void DoSetText()
        {
            destination.SetFormatText(textSource.text);
        }

    }
}