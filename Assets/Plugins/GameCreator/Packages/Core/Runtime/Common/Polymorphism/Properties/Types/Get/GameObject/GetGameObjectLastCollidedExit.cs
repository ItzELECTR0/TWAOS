using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Collided Exit")]
    [Category("Physics/Last Collided Exit")]
    
    [Image(typeof(IconPhysics), ColorTheme.Type.Red, typeof(OverlayArrowRight))]
    [Description("Reference to the last object that exited the collision with a Trigger")]

    [Serializable]
    public class GetGameObjectLastCollidedExit : PropertyTypeGetGameObject
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
            GetGameObjectLastCollidedExit instance = new GetGameObjectLastCollidedExit();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Last Collided Exit";
    }
}