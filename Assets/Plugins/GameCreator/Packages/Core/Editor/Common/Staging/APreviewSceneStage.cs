using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameCreator.Editor.Core
{
    public abstract class APreviewSceneStage : PreviewSceneStage
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string Title { get; }
        protected abstract string Icon { get; }

        public sealed override string assetPath => this.AssetPath;

        [field: NonSerialized] protected internal string AssetPath { get; set; }
        
        public ScriptableObject Asset => AssetDatabase.LoadAssetAtPath<ScriptableObject>(
            this.AssetPath
        );
        
        // TITLE: ---------------------------------------------------------------------------------
        
        protected override GUIContent CreateHeaderContent()
        {
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(this.Icon);
            return new GUIContent(this.Title, texture);
        }
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        public virtual void BeforeStageSetup()
        { }
        
        public virtual void AfterStageSetup()
        { }
    }
}