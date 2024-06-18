using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TBoxDrawer : PropertyDrawer
    {
        private const string USS_PATH = EditorPaths.COMMON + "Utilities/Helpers/Box/TBox";

        private const string CLASS_BODY_ACTIVE = "gc-box-active";
        
        private static readonly IIcon ICON_ARR_D = new IconArrowDropDown(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ARR_R = new IconArrowDropRight(ColorTheme.Type.TextLight);
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected virtual string Name(SerializedProperty property) => property.displayName;

        // IMPLEMENT METHODS: ---------------------------------------------------------------------
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            this.OnEnable();
            
            VisualElement root = new VisualElement { name = "GC-Box-Root" };
            VisualElement head = new VisualElement { name = "GC-Box-Head" };
            VisualElement body = new VisualElement { name = "GC-Box-Body" };

            root.Add(head);
            root.Add(body);
            
            this.SetupStyleSheets(root);

            Image headImage = new Image
            {
                image = property.isExpanded
                    ? ICON_ARR_D.Texture
                    : ICON_ARR_R.Texture
            };
            
            Label headLabel = new Label(this.Name(property));

            head.Add(headImage);
            head.Add(headLabel);
            
            head.RegisterCallback<ClickEvent>(clickEvent =>
            {
                property.serializedObject.Update();
                property.isExpanded = !property.isExpanded;
                
                headImage.image = property.isExpanded
                    ? ICON_ARR_D.Texture
                    : ICON_ARR_R.Texture;
                
                SerializationUtils.ApplyUnregisteredSerialization(property.serializedObject);
                
                body.Clear();
                body.RemoveFromClassList(CLASS_BODY_ACTIVE);
                
                if (property.isExpanded)
                {
                    body.AddToClassList(CLASS_BODY_ACTIVE);
                    this.CreatePropertyContent(body, property);
                }
                
                root.Bind(property.serializedObject);
            });

            if (property.isExpanded)
            {
                body.AddToClassList(CLASS_BODY_ACTIVE);
                this.CreatePropertyContent(body, property);
            }

            return root;
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void CreatePropertyContent(
            VisualElement container, SerializedProperty property)
        {
            SerializationUtils.CreateChildProperties(
                container,
                property, 
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
        }
        
        protected virtual void OnEnable()
        { }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetupStyleSheets(VisualElement root)
        {
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }
        }
    }
}
