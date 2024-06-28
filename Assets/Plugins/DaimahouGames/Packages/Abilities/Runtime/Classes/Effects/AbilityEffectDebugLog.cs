using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(0, 1, 1)]
    
    [Title("Debug log")]
    [Category("Debug log")]
    
    [Description("Print a debug log for each valid target")]

    [Keywords("Debug", "Log")]

    [Image(typeof(IconBug), ColorTheme.Type.Yellow)]
    [Serializable]
    public class AbilityEffectDebugLog : AbilityEffect
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private PropertyGetString m_Message;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        protected override string Summary => string.Format(
            "Debug Log{0}",
            string.IsNullOrEmpty(m_Message.ToString()) ? "" : $": {m_Message}"
        );

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected override void Apply_Internal(ExtendedArgs args)
        {
            var message = args.Target == null
                ? $"[Position: {args.Get<Vector3>()}]"
                : $"[Target: {args.Target.name}]";
            
            Debug.Log($"{message} {m_Message.Get(args)}", args.Target);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}