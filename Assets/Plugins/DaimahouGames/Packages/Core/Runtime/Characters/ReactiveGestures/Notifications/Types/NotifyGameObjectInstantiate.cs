using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Title("Instantiate GameObject")]
    [Category("Instantiate GameObject")]
    
    [Parameter("Game Object", "Game Object reference that is instantiated")]
    [Parameter("Position", "The position where the new game object is instantiated")]
    [Parameter("Rotation", "The rotation that the new game object has")]
    [Parameter("Save", "Optional value where the newly instantiated game object is stored")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Teal, typeof(OverlayPlus))]

    [Keywords("Create", "New", "Game Object")]
    [Serializable]
    public class AnimNotifyBaseBaseGameObjectInstantiate : TNotify
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetInstantiate m_GameObject = new();
        [SerializeField] private PropertyGetLocation m_Location = new();
        [SerializeField] private PropertyGetGameObject m_Parent = GetGameObjectNone.Create();
        [SerializeField] private PropertySetGameObject m_Save = SetGameObjectNone.Create;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string SubTitle => $"Instantiate {m_GameObject}";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected override Task Trigger(Character character)
        {
            if (character == null) return Task.CompletedTask;

            var target = character.gameObject;
            var instance = CreateInstance(target);

            if (instance != null)
            {
                var parent = m_Parent.Get<Transform>(target);
                if (parent != null) instance.transform.SetParent(parent);
                
                m_Save.Set(instance, target);
            }
            return Task.CompletedTask;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private GameObject CreateInstance(GameObject target)
        {
            var entityTransform = target.transform;

            var location = m_Location.Get(target);

            var position = location.HasPosition(target)
                ? location.GetPosition(target)
                : entityTransform.position;

            var rotation = location.HasRotation(target)
                ? location.GetRotation(target)
                : entityTransform.rotation;

            var instance = m_GameObject.Get(target, position, rotation);
            return instance;
        }
        
        //============================================================================================================||
    }
}