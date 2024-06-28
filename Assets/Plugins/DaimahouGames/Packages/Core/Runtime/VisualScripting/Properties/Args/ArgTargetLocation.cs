using System;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Core.Runtime.VisualScripting
{
    [Title("Target Location")]
    [Category("Target Location")]
    
    [Image(typeof(IconLocation), ColorTheme.Type.Green)]
    [Description("A target location")]
    
    [Serializable]
    public class ArgTargetLocation : PropertyTypeArg
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetLocation m_Location;
        
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public override string String => $"{m_Location}";
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        
        protected override void SetArgs(ref Args args)
        {
            var location = m_Location.Get(args);
            ExtendedArgs.Set(ref args, new Target(location.GetPosition(args.Self)));
        }
        
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}