using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Animation")]
    
    public interface IUnitAnimim : IUnitCommon
    {
        /**
         * IMPORTANT NOTE: It is required that the class implementing this interface has a
         * serializable field called 'm_Animator' that is used to know and change the reference
         * of the model in class ModelTool::ChangeModelEditor() 
        **/
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        Transform Mannequin { get; set; }
        
        Animator  Animator { get; set; }
        Reaction Reaction { get; set; }
        
        float SmoothTime  { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }
        
        Vector3 RootMotionDeltaPosition { get; }
        Quaternion RootMotionDeltaRotation { get; }

        // EVENTS: --------------------------------------------------------------------------------
        
        event Action<int> EventOnAnimatorIK;
        
        // METHODS: -------------------------------------------------------------------------------

        void ApplyMannequinPosition();
        void ApplyMannequinRotation();
        void ApplyMannequinScale();
    }
}