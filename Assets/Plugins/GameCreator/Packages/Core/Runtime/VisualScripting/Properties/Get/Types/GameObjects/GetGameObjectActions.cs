using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Actions")]
    [Category("Visual Scripting/Actions")]
    
    [Image(typeof(IconInstructions), ColorTheme.Type.Blue)]
    [Description("An Actions component reference")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectActions : PropertyTypeGetGameObject
    {
        [SerializeField] protected Actions m_Actions;

        public override GameObject Get(Args args) => this.m_Actions != null 
            ? this.m_Actions.gameObject 
            : null;
        
        public override GameObject Get(GameObject gameObject) => this.m_Actions != null 
            ? this.m_Actions.gameObject 
            : null;

        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Actions)) return this.m_Actions as T;
            return base.Get<T>(args);
        }

        public GetGameObjectActions() : base()
        { }

        public GetGameObjectActions(GameObject gameObject) : this()
        {
            this.m_Actions = gameObject.Get<Actions>();
        }
        
        public GetGameObjectActions(Actions actions) : this()
        {
            this.m_Actions = actions;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectActions instance = new GetGameObjectActions();
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(GameObject gameObject)
        {
            GetGameObjectActions instance = new GetGameObjectActions
            {
                m_Actions = gameObject != null ? gameObject.Get<Actions>() : null
            };
            
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(Actions actions)
        {
            GetGameObjectActions instance = new GetGameObjectActions
            {
                m_Actions = actions
            };
            
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_Actions != null
            ? this.m_Actions.gameObject.name
            : "(none)";

        public override GameObject EditorValue => this.m_Actions != null 
            ? this.m_Actions.gameObject
            : null;
    }
}