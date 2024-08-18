using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Name")]
    [Category("Game Objects/Game Object Name")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("Returns the name of the game object")]
    
    [Serializable]
    public class GetStringGameObjectsName : PropertyTypeGetString
    {
        [SerializeField] 
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();

        public override string Get(Args args) => this.GetName(args);

        private string GetName(Args args)
        {
            GameObject gameObject = m_GameObject.Get(args);
            return gameObject != null ? gameObject.name : string.Empty;
        }

        public GetStringGameObjectsName() : base()
        { }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringGameObjectsName()
        );

        public override string String => $"{this.m_GameObject}'s Name";

        public override string EditorValue => this.m_GameObject.EditorValue != null
            ? this.m_GameObject.EditorValue.name
            : default;
    }
}