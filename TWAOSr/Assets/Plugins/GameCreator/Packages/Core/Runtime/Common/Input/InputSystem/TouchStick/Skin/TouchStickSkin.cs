using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [CreateAssetMenu(
        fileName = "Touchstick Skin",
        menuName = "Game Creator/Developer/Touchstick Skin",
        order = 300
    )]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoTouchstick.png")]
    
    public class TouchStickSkin : TSkin<GameObject>
    {
        private const string MSG = "A game object prefab with a Touchstick component";
        
        private const string ERR_NO_VALUE = "Prefab value cannot be empty";
        private const string ERR_TOUCHSTICK = "Prefab does not contain a 'TouchStick' component";

        public override string Description => MSG;

        public override string HasError
        {
            get
            {
                if (this.Value == null) return ERR_NO_VALUE;
                return !this.Value.GetComponentInChildren<TTouchStick>() 
                    ? ERR_TOUCHSTICK 
                    : string.Empty;
            }
        }
    }
}