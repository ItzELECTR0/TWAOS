using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Shot Active")]
    [Description("Returns true if the Camera Shot is assigned to the Main Camera")]

    [Category("Cameras/Is Shot Active")]
    
    [Parameter("Shot", "The camera shot")]

    [Keywords("Camera", "Enabled", "Assigned", "Running")]
    [Image(typeof(IconCameraShot), ColorTheme.Type.Green)]
    
    [Serializable]
    public class ConditionCameraShotActive : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Shot = GetGameObjectShot.Create;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is {this.m_Shot} Active";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            ShotCamera shot = this.m_Shot.Get<ShotCamera>(args);
            if (shot == null) return false;

            MainCamera mainCamera = ShortcutMainCamera.Get<MainCamera>();
            if (mainCamera == null) return false;

            return mainCamera.Transition.CurrentShotCamera == shot;
        }
    }
}
