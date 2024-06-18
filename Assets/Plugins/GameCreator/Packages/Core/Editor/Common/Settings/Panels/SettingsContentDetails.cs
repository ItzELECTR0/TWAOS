using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    internal class SettingsContentDetails : VisualElement
    {
        private const string NAME_TOOLBAR = "GC-Settings-Details-Toolbar";
        private const string NAME_SCROLLVIEW = "GC-Settings-Details-ScrollView";
        private const string NAME_SCROLLVIEW_CONTENT = "GC-Settings-Details-ScrollView-Content";

        private const string CLASS_FULLSCREEN = "gc-settings-details-content-fullscreen";
        
        private const string USS_PATH = EditorPaths.COMMON + "Settings/Stylesheets/SettingsDetails";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SettingsWindow m_Window;

        private Toolbar m_Toolbar;
        private ScrollView m_ScrollView;
        private VisualElement m_Content;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public SettingsContentDetails(SettingsWindow window)
        {
            this.m_Window = window;
            this.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            this.AddToClassList(AlignLabel.CLASS_UNITY_MAIN_CONTAINER);
        }

        public void OnEnable()
        {
            this.m_Window.EventChangeSelection += this.OnChangeSelection;
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.styleSheets.Add(styleSheet);
            }
        }

        public void OnDisable()
        {
            this.m_Window.EventChangeSelection -= this.OnChangeSelection;
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeSelection(int index)
        {
            this.Clear();

            TAssetRepository asset = this.m_Window.ContentList.Asset;
            if (asset == null) return;

            this.CreateToolbar(asset);
            this.CreateContent(asset);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void CreateToolbar(TAssetRepository asset)
        {
            this.m_Toolbar = new Toolbar { name = NAME_TOOLBAR };
            this.Add(this.m_Toolbar);

            VisualElement spacer = new VisualElement();
            spacer.AddToClassList("gc-spacer");
            
            ToolbarButton buttonSelect = new ToolbarButton(() =>
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            });

            buttonSelect.text = "Select Asset";

            this.m_Toolbar.Add(spacer);
            this.m_Toolbar.Add(buttonSelect);
        }
        
        private void CreateContent(TAssetRepository asset)
        {
            if (asset.IsFullScreen)
            {
                this.m_Content = new VisualElement { name = NAME_SCROLLVIEW_CONTENT };
                this.Add(this.m_Content);
            }
            else
            {
                this.m_ScrollView = new ScrollView(ScrollViewMode.Vertical)
                {
                    name = NAME_SCROLLVIEW,
                    contentContainer =
                    {
                        name = NAME_SCROLLVIEW_CONTENT
                    }
                };

                this.m_Content = this.m_ScrollView.contentContainer;
                this.Add(this.m_ScrollView);
            }

            SerializedObject assetSerialized = new SerializedObject(asset);
            SerializedProperty assetProperty = assetSerialized.FindProperty(
                TAssetRepositoryEditor.NAMEOF_MEMBER
            );

            PropertyField fieldAsset = new PropertyField(assetProperty);
            fieldAsset.Bind(assetSerialized);
            
            if (asset.IsFullScreen) fieldAsset.AddToClassList(CLASS_FULLSCREEN);
            
            this.m_Content.Add(fieldAsset);
        }
    }
}