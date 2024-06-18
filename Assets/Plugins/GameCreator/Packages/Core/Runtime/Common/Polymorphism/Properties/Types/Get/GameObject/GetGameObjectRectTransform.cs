using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Rect Transform")]
    [Category("Transforms/Rect Transform")]
    
    [Image(typeof(IconRectTransform), ColorTheme.Type.Green)]
    [Description("A Rect Transform scene reference or prefab")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectRectTransform : PropertyTypeGetGameObject
    {
        [SerializeField] protected RectTransform m_RectTransform;

        public override GameObject Get(Args args) => this.m_RectTransform != null 
            ? this.m_RectTransform.gameObject 
            : null;
        public override GameObject Get(GameObject gameObject) => this.m_RectTransform != null 
            ? this.m_RectTransform.gameObject 
            : null;

        public GetGameObjectRectTransform() : base()
        { }

        public GetGameObjectRectTransform(RectTransform rectTransform) : this()
        {
            this.m_RectTransform = rectTransform;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectRectTransform instance = new GetGameObjectRectTransform();
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(RectTransform rectTransform)
        {
            GetGameObjectRectTransform instance = new GetGameObjectRectTransform
            {
                m_RectTransform = rectTransform
            };
            
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_RectTransform != null
            ? this.m_RectTransform.name
            : "(none)";

        public override GameObject EditorValue => this.m_RectTransform != null
            ? this.m_RectTransform.gameObject
            : null;
    }
}