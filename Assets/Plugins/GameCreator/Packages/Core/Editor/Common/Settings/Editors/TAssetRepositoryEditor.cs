using System;
using System.IO;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(TAssetRepository), true)]
    public class TAssetRepositoryEditor : UnityEditor.Editor
    {
        private const string USS_PATH = EditorPaths.COMMON + "Settings/Stylesheets/Repository";
        
        public const string NAMEOF_MEMBER = "m_Repository";

        private const string ERR_LOCATION = "This asset location is not valid.";
        private const string NAME_ERROR = "GC-Repository-Error";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Root;
        private VisualElement m_FixLocation;
        
        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void OnEnable()
        {
            EditorApplication.projectChanged += this.RefreshFixLocation;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= this.RefreshFixLocation;
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void CreateContent(VisualElement root)
        {
            SerializedProperty repository = this.serializedObject.FindProperty(NAMEOF_MEMBER);
            PropertyField fieldRepository = new PropertyField(repository);
            
            root.Add(fieldRepository);
        }

        // GUI METHODS: ---------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.m_Root.styleSheets.Add(styleSheet);
            }
            
            this.m_FixLocation = new VisualElement();
            this.m_Root.Add(this.m_FixLocation);
            
            this.RefreshFixLocation();
            this.CreateContent(this.m_Root);

            return this.m_Root;
        }

        private void RefreshFixLocation()
        {
            if (this.m_FixLocation == null) return;
            this.m_FixLocation.Clear();

            TAssetRepository assetRepository = this.target as TAssetRepository;

            string expectedDirectoryPath = assetRepository != null
                ? assetRepository.AssetPath
                : string.Empty;

            string expectedFilePath = assetRepository != null
                ? PathUtils.Combine(expectedDirectoryPath, $"{assetRepository.RepositoryID}.asset")
                : string.Empty;

            if (string.IsNullOrEmpty(expectedFilePath)) return;
            if (AssetDatabase.LoadAssetAtPath<TAssetRepository>(expectedFilePath) != null) return;
            
            VisualElement errorMessage = new Label
            {
                name = NAME_ERROR,
                text = ERR_LOCATION
            };
            
            this.m_FixLocation.Add(errorMessage);
                
            Button buttonFix = new Button(() =>
            {
                UnityEngine.Object reference = this.serializedObject.targetObject;
                string currentPath = AssetDatabase.GetAssetPath(reference);

                string resolution = AssetDatabase.ValidateMoveAsset(currentPath, expectedFilePath);
                if (!string.IsNullOrEmpty(resolution)) return;
                    
                AssetDatabase.MoveAsset(currentPath, expectedFilePath);
                AssetDatabase.Refresh();
            })
            {
                text = "Fix Location"
            };
                
            this.m_FixLocation.Add(buttonFix);
        }
    }
}