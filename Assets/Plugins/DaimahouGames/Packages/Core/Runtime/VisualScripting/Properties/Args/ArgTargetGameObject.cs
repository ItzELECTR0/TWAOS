using System;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Core.Runtime.VisualScripting
{
    [Title("Target Game Object")]
    [Category("Target Game Object")]
    
    [Image(typeof(IconSphereSolid), ColorTheme.Type.Green)]
    [Description("A target Game Object")]
    
    [Serializable]
    public class ArgTargetGameObject : PropertyTypeArg
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetGameObject m_GameObject;
        
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public override string String => $"{m_GameObject}";
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        
        protected override void SetArgs(ref Args args)
        {
            var gameObject = m_GameObject.Get(args);
            ExtendedArgs.Set(ref args, new Target(gameObject));
        }
        
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}