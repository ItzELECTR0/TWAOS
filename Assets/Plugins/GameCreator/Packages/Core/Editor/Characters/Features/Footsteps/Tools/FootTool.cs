using System;
using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class FootTool : TPolymorphicItemTool
    {
        private static readonly IIcon ICON_BONE = new IconBoneOutline(ColorTheme.Type.Green);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.CHARACTERS + "StyleSheets/Foot-Head",
            EditorPaths.CHARACTERS + "StyleSheets/Foot-Body"
        };

        protected override object Value => null;

        public override string Title
        {
            get
            {
                const string nameType = BoneDrawer.PROP_TYPE;
                SerializedProperty bone = this.m_Property.FindPropertyRelative("m_Bone");
                SerializedProperty type = bone.FindPropertyRelative(nameType);
                
                switch (type.enumValueIndex)
                {
                    case 0: // None
                        return "(none)";
                    
                    case 1: // Human
                        const string nameHuman = BoneDrawer.PROP_HUMAN;
                        SerializedProperty human = bone.FindPropertyRelative(nameHuman);
                        return $"Human {human.enumDisplayNames[human.enumValueIndex]}";
                        
                    case 2: // Path
                        const string namePath = BoneDrawer.PROP_PATH;
                        SerializedProperty path = bone.FindPropertyRelative(namePath);
                        return $"Path {path.stringValue}";
                    
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public FootTool(IPolymorphicListTool parentTool, int index)
            : base(parentTool, index)
        { }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------

        protected override Texture2D GetIcon()
        {
            return ICON_BONE.Texture;
        }

        protected override void SetupBody()
        {
            VisualElement fieldBody = BoneDrawer.CreatePropertyGUI(
                this.m_Property.FindPropertyRelative("m_Bone"),
                "Bone"
            );

            fieldBody.Bind(this.m_Property.serializedObject);
            fieldBody.RegisterCallback<SerializedPropertyChangeEvent>(change =>
            {
                this.UpdateHead();
            });
            
            this.m_Body.Add(fieldBody);
            this.UpdateBody(false);
        }
    }
}