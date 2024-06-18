using UnityEditor;

namespace NullSave
{

    [CustomEditor(typeof(ReIconedImageAction))]
    public class ReIconedImageActionEditor : CogEditor
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
            SimpleProperty("notFoundSprite");

            MainContainerEnd();
        }

        #endregion

    }
}