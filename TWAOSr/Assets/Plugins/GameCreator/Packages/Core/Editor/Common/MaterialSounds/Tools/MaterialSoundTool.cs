using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    public class MaterialSoundTool : TPolymorphicItemTool
    {
        private static readonly IIcon DEFAULT_ICON = new IconTexture(ColorTheme.Type.TextLight);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.COMMON + "MaterialSounds/StyleSheets/MaterialSound-Head",
            EditorPaths.COMMON + "MaterialSounds/StyleSheets/MaterialSound-Body"
        };

        protected override object Value => this.m_Property.GetValue<MaterialSoundTexture>();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MaterialSoundTool(IPolymorphicListTool parentTool, int index) 
            : base(parentTool, index)
        { }

        protected override Texture2D GetIcon()
        {
            SerializedProperty property = this.m_Property.FindPropertyRelative("m_Texture");
            Texture2D texture = property.objectReferenceValue as Texture2D;
            
            return texture != null ? texture : DEFAULT_ICON.Texture;
        }
    }
}