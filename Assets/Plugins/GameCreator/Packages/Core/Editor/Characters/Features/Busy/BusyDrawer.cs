using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(Busy))]
    public class BusyDrawer : PropertyDrawer
    {
        private const string NAME_BUSY = "GC-Character-Busy";
        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/Busy";

        private readonly IIcon ICON_NONE = new IconBusyNone(ColorTheme.Type.TextLight);

        // CREATE PROPERTY: -----------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Character character = property.serializedObject.targetObject as Character;
            if (character == null) return new Label("Unknown Character as SerializedObject target");

            VisualElement container = new VisualElement
            {
                name = NAME_BUSY,
                tooltip = "Displays the current state of the Busy system"
            };

            Image imageBase     = new Image { image = ICON_NONE.Texture };
            Image imageDead     = new Image { image = ICON_NONE.Texture };
            Image imageArmLeft  = new Image { image = ICON_NONE.Texture };
            Image imageArmRight = new Image { image = ICON_NONE.Texture };
            Image imageLegLeft  = new Image { image = ICON_NONE.Texture };
            Image imageLegRight = new Image { image = ICON_NONE.Texture };

            container.Add(imageBase);
            imageBase.Add(imageDead);
            imageDead.Add(imageArmLeft);
            imageArmLeft.Add(imageArmRight);
            imageArmRight.Add(imageLegLeft);
            imageLegLeft.Add(imageLegRight);

            container.userData = new BusyTool(
                character, imageBase, imageDead,
                imageArmLeft, imageArmRight,
                imageLegLeft, imageLegRight
            );
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets) container.styleSheets.Add(styleSheet);

            return container;
        }
    }
}