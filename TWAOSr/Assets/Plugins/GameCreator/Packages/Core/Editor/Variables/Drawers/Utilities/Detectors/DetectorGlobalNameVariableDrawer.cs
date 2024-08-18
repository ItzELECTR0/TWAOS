using System;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(DetectorGlobalNameVariable))]
    public class DetectorGlobalNameVariableDrawer : TDetectorNameVariableDrawer
    {
        protected override Type AssetType => typeof(GlobalNameVariables);
        protected override bool AllowSceneReferences => false;
        
        protected override TNamePickTool Tool(ObjectField field, SerializedProperty property)
        {
            GlobalNamePickTool namePickTool = new GlobalNamePickTool(property);
            field.RegisterValueChangedCallback(_ => namePickTool.OnChangeAsset());
            
            return namePickTool;
        }
    }
}