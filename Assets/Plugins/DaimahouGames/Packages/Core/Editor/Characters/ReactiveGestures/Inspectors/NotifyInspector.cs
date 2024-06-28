using System;
using DaimahouGames.Runtime.Characters;
using GameCreator.Editor.Common;
using DaimahouGames.Editor.Core;
using UnityEngine.UIElements;

namespace PrototypeCreator.Core.Editor.Characters
{
    public sealed class NotifyInspector : GenericInspector
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|

        private Slider m_Slider;
        private MinMaxSlider m_MinMaxSlider;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public override string Title => m_Property.GetValue<TNotifyBase>()?.Title;
        private float AnimValue => m_Slider?.value ?? m_MinMaxSlider.minValue;
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public NotifyInspector(ListInspector list, int index) : base(list, index) {}
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected override void SetupHead()
        {
            base.SetupHead();

            if (IsExpanded)
            {
                ListInspector.CollapseItems();
                IsExpanded = true;
                Refresh();
            }
            
            m_HeadButton.clicked += () =>
            {
                var isExpanded = IsExpanded;
                ListInspector.CollapseItems();
                IsExpanded = isExpanded;
                ListInspector.Refresh();
                
                if(IsEnabled) ((NotifiesInspector) ListInspector).UpdateAnimationProgress(AnimValue);
            };
        }

        protected override void SetupBody()
        {
            if (m_Property.FindPropertyRelative("m_TriggerTime") == null)
            {
                DrawMinMaxSlider(m_Body);
            }
            else
            {
                DrawSlider(m_Body);
            }
            
            m_Body.Add(new SpaceSmall());
            base.SetupBody();
        }

        private void DrawSlider(VisualElement root)
        {
            m_Slider = new Slider
            {
                value = m_Property.FindPropertyRelative("m_TriggerTime").floatValue,
                highValue = 1
            };

            root.Add(m_Slider);

            m_Slider.RegisterValueChangedCallback(evt =>
            {
                ((NotifiesInspector) ListInspector).UpdateAnimationProgress(evt.newValue);
                m_Property.FindPropertyRelative("m_TriggerTime").floatValue = evt.newValue;
                SerializationUtils.ApplyUnregisteredSerialization(m_Property.serializedObject);
                Refresh();
            });
        }
        
        
        private void DrawMinMaxSlider(VisualElement root)
        {
            m_MinMaxSlider = new MinMaxSlider("", 0, 1, 0, 1)
            {
                value = m_Property.FindPropertyRelative("m_ActiveWindow").vector2Value,
                lowLimit = 0,
                highLimit = 1,
            };
    
            root.Add(m_MinMaxSlider);
            
            m_MinMaxSlider.RegisterValueChangedCallback(evt =>
            {
                var animValue = Math.Abs(evt.previousValue.x - evt.newValue.x) > float.Epsilon ? 
                    evt.newValue.x :
                    evt.newValue.y;
                
                ((NotifiesInspector) ListInspector).UpdateAnimationProgress(animValue);
                m_Property.FindPropertyRelative("m_ActiveWindow").vector2Value = evt.newValue;
                SerializationUtils.ApplyUnregisteredSerialization(m_Property.serializedObject);
                Refresh();
            });
        }
        
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}