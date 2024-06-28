using System;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Core
{
    [Serializable]
    
    [Image(typeof(IconBrain), ColorTheme.Type.Blue)]
    
    [Description("Brain")]
    public class Brain : Feature
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeReference] private Controller m_Controller;
        [SerializeField] private bool m_AutoPossess;
        
        public Controller Controller => m_Controller;
        
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public override string Title => string.Format("Brain: {0}{1}",
            m_Controller ? m_Controller.name : "(none)",
            m_AutoPossess ? " [Auto-possess]" : "");

        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|

        protected override void Awake()
        {
            if(m_AutoPossess) m_Controller.Possess(Pawn);
        }

        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public void ChangeAutoPosses(bool autoPossess)
        {
            m_AutoPossess = autoPossess;
        }
        
        public void ChangeController(Controller controller)
        {
            throw new NotImplementedException();
        }
        
        public T GetInputProvider<T>() where T : class, IInputProvider => m_Controller.GetInputProvider<T>();

        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}