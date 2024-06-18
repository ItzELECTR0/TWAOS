using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Child Index")]
    [Category("Transforms/Game Object Child Index")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayDot))]
    [Description("The N-th child of a game object")]

    [Serializable]
    public class GetGameObjectChildByIndex : PropertyTypeGetGameObject
    {
        [SerializeField] 
        private PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();
        
        [SerializeField] 
        private PropertyGetInteger m_Index = GetDecimalInteger.Create(0);

        public override GameObject Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return null;

            int index = (int) Math.Clamp(
                this.m_Index.Get(args), 
                0, gameObject.transform.childCount - 1
            );
            
            Transform child = gameObject.transform.GetChild(index);
            return child != null ? child.gameObject : null;
        }
        
        public static PropertyGetGameObject Create()
        {
            GetGameObjectChildByIndex instance = new GetGameObjectChildByIndex();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_Transform}/{this.m_Index}";

        public override GameObject EditorValue
        {
            get
            {
                GameObject parent = this.m_Transform.EditorValue;
                if (parent == null) return null;
                if (!int.TryParse(this.m_Index.ToString(), out int index)) return null;
                
                index = Math.Clamp(index, 0, parent.transform.childCount - 1);
                return index < parent.transform.childCount 
                    ? parent.transform.GetChild(index).gameObject 
                    : null;
            }
        }
    }
}