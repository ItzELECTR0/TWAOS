using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Collided Enter")]
    [Category("Physics/Last Collided Enter")]
    
    [Image(typeof(IconPhysics), ColorTheme.Type.Red, typeof(OverlayArrowLeft))]
    [Description("Reference to the last object that collided with a Trigger")]

    [Serializable]
    public class GetGameObjectLastCollidedEnter : PropertyTypeGetGameObject
    {
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode() => Instance = null;
        
        #endif
        
        public static GameObject Instance;
        
        public override GameObject Get(Args args) => Instance;
        public override GameObject Get(GameObject gameObject) => Instance;

        public static PropertyGetGameObject Create()
        {
            GetGameObjectLastCollidedEnter instance = new GetGameObjectLastCollidedEnter();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Last Collided";
    }
}