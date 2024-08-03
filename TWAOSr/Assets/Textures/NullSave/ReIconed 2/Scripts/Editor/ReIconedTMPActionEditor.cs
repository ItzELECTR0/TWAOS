using UnityEditor;

namespace NullSave
{

    [CustomEditor(typeof(ReIconedTMPAction))]
    public class ReIconedTMPActionEditor : CogEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("playerId");
            SimpleProperty("updateType");
            SimpleProperty("actionLookup");
            if ((ReIconedActionLookup)SimpleInt("actionLookup") == ReIconedActionLookup.Name)
            {
                SimpleProperty("actionName");
            }
            else
            {
                SimpleProperty("actionId");
            }
            SimpleProperty("formatText");
            SimpleProperty("notFoundText");
            SimpleProperty("tint", "Sprite Tint");

            MainContainerEnd();
        }

        #endregion

    }
}