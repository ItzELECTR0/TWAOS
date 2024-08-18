using System;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(DetectorLocalNameVariable))]
    public class DetectorLocalNameVariableDrawer : TDetectorNameVariableDrawer
    {
        protected override Type AssetType => typeof(LocalNameVariables);
        protected override bool AllowSceneReferences => true;

        protected override TNamePickTool Tool(ObjectField field, SerializedProperty property)
        {
            LocalNamePickTool namePickTool = new LocalNamePickTool(property);
            field.RegisterValueChangedCallback(_ => namePickTool.OnChangeAsset());

            return namePickTool;
        }
    }
}