using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public sealed class InfoMessage : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/InfoMessage";
        
        private static readonly IIcon ICON = new IconInfoOutline(ColorTheme.Type.TextLight);
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|

        private readonly Label m_Label;
        private readonly Image m_Image;

        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public string Text
        {
            set => m_Label.text = value;
        }
        
        public Texture2D Icon
        {
            set => m_Image.image = value;
        }

        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public InfoMessage(string message)
        {
            var sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (var sheet in sheets) styleSheets.Add(sheet);

            m_Image = new Image { image = ICON.Texture };
            m_Label = new Label { text = message };
            
            Add(m_Image);
            Add(m_Label);
        }
        
        //============================================================================================================||
    }
}