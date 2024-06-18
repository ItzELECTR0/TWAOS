using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Conditions")]
    [Category("Visual Scripting/Conditions")]
    
    [Image(typeof(IconConditions), ColorTheme.Type.Green)]
    [Description("A Conditions component reference")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectConditions : PropertyTypeGetGameObject
    {
        [SerializeField] protected Conditions m_Conditions;

        public override GameObject Get(Args args) => this.m_Conditions != null 
            ? this.m_Conditions.gameObject 
            : null;
        
        public override GameObject Get(GameObject gameObject) => this.m_Conditions != null 
            ? this.m_Conditions.gameObject 
            : null;
        
        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Conditions)) return this.m_Conditions as T;
            return base.Get<T>(args);
        }

        public GetGameObjectConditions() : base()
        { }

        public GetGameObjectConditions(GameObject gameObject) : this()
        {
            this.m_Conditions = gameObject.Get<Conditions>();
        }
        
        public GetGameObjectConditions(Conditions Conditions) : this()
        {
            this.m_Conditions = Conditions;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectConditions instance = new GetGameObjectConditions();
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(GameObject gameObject)
        {
            GetGameObjectConditions instance = new GetGameObjectConditions
            {
                m_Conditions = gameObject != null ? gameObject.Get<Conditions>() : null
            };
            
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(Conditions Conditions)
        {
            GetGameObjectConditions instance = new GetGameObjectConditions
            {
                m_Conditions = Conditions
            };
            
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_Conditions != null
            ? this.m_Conditions.gameObject.name
            : "(none)";
        
        public override GameObject EditorValue => this.m_Conditions != null 
            ? this.m_Conditions.gameObject
            : null;
    }
}