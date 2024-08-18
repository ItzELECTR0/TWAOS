using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Input Flick")]
    [Category("Input/On Input Flick")]
    [Description("Detects when Input (Vector 2) is flicked")]

    [Image(typeof(IconJoystick), ColorTheme.Type.Yellow)]

    [Serializable]
    public class EventOnInputFlick : TEventValue
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputPropertyValueVector2 m_Input = new InputPropertyValueVector2();
        [SerializeField] private CompareMinDistanceOrNone m_MinDistance = new CompareMinDistanceOrNone();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Value => this.m_Input.Read().magnitude;
        protected override CompareMinDistanceOrNone MinDistance => this.m_MinDistance;

        // METHODS: -------------------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            this.m_Input.OnStartup();
        }

        protected internal override void OnDestroy(Trigger trigger)
        {
            base.OnDestroy(trigger);
            this.m_Input.OnDispose();
        }

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            
            this.m_Input.OnUpdate();
            this.CheckExecute();
        }
    }
}