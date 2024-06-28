using System.IO;
using System.Reflection;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Characters;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrototypeCreator.Core.Editor.Characters
{
    [CustomEditor(typeof(ReactiveState))]
    public class ReactiveStateEditor : ReactiveGestureEditor
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        
        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            m_Root.Add(new SpaceSmall());
            
            var folder = new FoldoutInspector();
            folder.AddBodyElements(serializedObject, "m_StateClip", "m_StateMask", "m_Speed", "m_Entry", "m_Exit");
            folder.SetTile("Animation Settings");

            m_Root.Add(folder);

            m_Player = new ReactiveGesturePlayer(m_PreviewObject, serializedObject);
            
            CreateNotifiesInspector();
            
            return m_Root;
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※

        private const string ASSETS = "Assets/";
        protected const BindingFlags MEMBER_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        
       [MenuItem("Assets/Create/Game Creator/Characters/Reactive Gesture State", false, -20)]
        internal static void CreateFromMenuItem()
        {
            var state = CreateState<ReactiveState>(
                "Reactive Gesture State",
                RuntimePaths.CHARACTERS + "Assets/Overrides/Animation.overrideController"
            );
            
            state.name = "Reactive Gesture State";
        }

        private static T CreateState<T>(string name, string sourcePath) where T : StateOverrideAnimator
        {
            T state = CreateState<T>(name);

            AnimatorOverrideController controller = Instantiate(
                AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(sourcePath)
            );
            

            controller.name = Path.GetFileNameWithoutExtension(sourcePath); 
            controller.hideFlags = HideFlags.HideInHierarchy;
            
            AssetDatabase.AddObjectToAsset(controller, state);
            typeof(T).GetField("m_Controller", MEMBER_FLAGS)?.SetValue(state, controller);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(state));

            return state;
        }

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
            
            AssetDatabase.CreateAsset(state, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = state;

            return state;
        }
        
        //============================================================================================================||
    }
}