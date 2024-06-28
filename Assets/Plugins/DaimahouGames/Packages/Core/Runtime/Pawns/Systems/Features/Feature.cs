using System;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine;

namespace DaimahouGames.Runtime.Pawns
{
    [Serializable]
    public abstract class Feature : IFeature, IGenericItem
    {
        //============================================================================================================||
        // -----------------------------------------------------------------------------------------------------------|
        
        #region EditorInfo
        [SerializeField] private bool m_IsExpanded;
        public virtual string Title { get; } = "feature - (no name)";
        public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
        public bool IsExpanded { get => m_IsExpanded; set => m_IsExpanded = value; }
        public virtual string[] Info { get; } = Array.Empty<string>();
        #endregion
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State ------------------------------------------------------------------------------------->|

        [SerializeField] private Pawn m_Pawn;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public Pawn Pawn => m_Pawn;
        public GameObject GameObject => m_Pawn == null ? null : m_Pawn.gameObject;
        public Transform Transform => Pawn.Transform;
        
        public string Name => GameObject.name;
        public Vector3 Position => Transform.position;
        
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        
        protected virtual void Enable() {}
        protected virtual void Disable() {}
        
        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        
        public T Get<T>() where T : Feature => Pawn.Get<T>();
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        
        protected virtual void Awake() {}
        protected virtual void Start() {}
        protected virtual void Update() {}
        
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※

        void IFeature.Awake() => Awake();
        void IFeature.Start() => Start();
        void IFeature.Update() => Update();
        void IFeature.Enable() => Enable();
        void IFeature.Disable() => Disable();
        
        //============================================================================================================||
    }
}