using UnityEditor;

namespace NullSave
{

    [CustomEditor(typeof(ReIconedTMPActionPlus))]
    public class ReIconedTMPActionPlusEditor : CogEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("playerId");
            SimpleProperty("updateType");
            SimpleProperty("formatText");
            SimpleProperty("notFoundText");
            SimpleProperty("tint", "Sprite Tint");

            MainContainerEnd();
        }

        #endregion

    }
}