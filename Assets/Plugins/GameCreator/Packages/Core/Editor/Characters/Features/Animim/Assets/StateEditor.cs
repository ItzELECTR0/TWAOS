using System.IO;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public abstract class StateEditor : UnityEditor.Editor
    {
        protected const BindingFlags MEMBER_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        
        private const string ASSETS = "Assets/";
        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/State";

        // MEMBERS: -------------------------------------------------------------------------------

        protected VisualElement m_Root;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected virtual bool HasEntry => true;
        protected virtual bool HasExit => true;

        // PAINT METHODS: -------------------------------------------------------------------------

        public sealed override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.m_Root.styleSheets.Add(sheet);

            this.CreateState();

            if (this.HasEntry)
            {
                SerializedProperty entry = this.serializedObject.FindProperty("m_Entry");
                PropertyField fieldEntry = new PropertyField(entry);
                this.m_Root.Add(fieldEntry);
            }
            
            if (this.HasExit)
            {
                SerializedProperty exit = this.serializedObject.FindProperty("m_Exit");
                PropertyField fieldExit = new PropertyField(exit);
                this.m_Root.Add(fieldExit);
            }
            
            SerializedProperty motion = this.serializedObject.FindProperty("m_Properties");
            PropertyField fieldMotion = new PropertyField(motion);
            this.m_Root.Add(fieldMotion);
            
            SerializedProperty onChange = this.serializedObject.FindProperty("m_OnChange");
            PropertyField fieldOnChange = new PropertyField(onChange);

            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(new LabelTitle("On Refresh:"));
            this.m_Root.Add(new SpaceSmaller());
            this.m_Root.Add(fieldOnChange);

            return this.m_Root;
        }

        private void CreateState()
        {
            this.CreateContent();
            
            SerializedProperty avatarMask = this.serializedObject.FindProperty("m_StateMask");
            PropertyField fieldAvatarMask = new PropertyField(avatarMask);

            VisualElement fieldSpacer = new VisualElement();
            fieldSpacer.AddToClassList("gc-space-smaller");
            
            this.m_Root.Add(fieldAvatarMask);
            this.m_Root.Add(fieldSpacer);
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void CreateContent();

        // CREATE STATE: --------------------------------------------------------------------------

        protected static T CreateState<T>(string name) where T : StateOverrideAnimator
        {
            T state = CreateInstance<T>();

            string selection = Selection.activeObject != null
                ? AssetDatabase.GetAssetPath(Selection.activeObject)
                : ASSETS;

            string directory = File.Exists(PathUtils.PathForOS(selection)) 
                ? PathUtils.PathToUnix(Path.GetDirectoryName(selection)) 
                : selection;

            string path = AssetDatabase.GenerateUniqueAssetPath(
                PathUtils.Combine(directory ?? ASSETS, $"{name}.asset")
            );
            
            DirectoryUtils.RequireFilepath(path);
            AssetDatabase.CreateAsset(state, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = state;

            return state;
        }
    }
}