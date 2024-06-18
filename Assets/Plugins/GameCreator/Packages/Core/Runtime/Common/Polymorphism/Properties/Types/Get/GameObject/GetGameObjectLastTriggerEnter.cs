using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Trigger Enter")]
    [Category("Physics/Last Trigger Enter")]
    
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowLeft))]
    [Description("Reference to the last object that entered a Trigger collider with isTrigger")]

    [Serializable]
    public class GetGameObjectLastTriggerEnter : PropertyTypeGetGameObject
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
            GetGameObjectLastTriggerEnter instance = new GetGameObjectLastTriggerEnter();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Last Trigger Enter";
    }
}