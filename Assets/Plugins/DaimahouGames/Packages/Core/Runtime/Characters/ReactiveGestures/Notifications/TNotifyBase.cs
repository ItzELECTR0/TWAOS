using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Serializable]
    public abstract class TNotifyBase : IGenericItem, INotify
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        #region EditorInfo
        [SerializeField] private bool m_IsExpanded;
        public virtual string Title => "Notify Base";
        public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
        public bool IsExpanded { get => m_IsExpanded; set => m_IsExpanded = value; }
        public virtual string[] Info { get; } = Array.Empty<string>();
        #endregion
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public abstract Task Update(Character character, float progressLastFrame, float currentProgress);
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        
        protected abstract Task Trigger(Character character);
        
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}