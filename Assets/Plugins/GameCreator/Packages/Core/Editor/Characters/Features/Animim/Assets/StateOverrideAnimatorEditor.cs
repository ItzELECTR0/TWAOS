using System.IO;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Characters
{
    public abstract class StateOverrideAnimatorEditor : StateEditor
    {
        protected abstract override void CreateContent();
        
        // CREATE STATE: --------------------------------------------------------------------------
        
        protected static T CreateState<T>(string name, string sourcePath)
            where T : StateOverrideAnimator
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
    }
}