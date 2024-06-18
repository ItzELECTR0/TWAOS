using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Trigger Exit")]
    [Category("Physics/Last Trigger Exit")]
    
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Description("Reference to the last object that exited a Trigger collider with isTrigger")]

    [Serializable]
    public class GetGameObjectLastTriggerExit : PropertyTypeGetGameObject
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
            GetGameObjectLastTriggerExit instance = new GetGameObjectLastTriggerExit();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Last Trigger Exit";
    }
}