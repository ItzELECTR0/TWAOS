using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Inwards")]
    [Category("Inwards")]
    [Description("Moves the target on the outer ring and rotates it towards the center")]

    [Image(typeof(IconCircleOutline), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class MarkerTypeInwards : TMarkerType
    {
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalConstantPointFive.Create;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Args m_Args;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Vector3 GetPosition(Marker marker, GameObject user)
        {
            if (user == null) return marker.transform.position;

            Character userCharacter = user.Get<Character>();
            Vector3 userPosition = userCharacter != null 
                ? userCharacter.Feet 
                : user.transform.position;

            this.m_Args ??= new Args(marker);
            this.m_Args.ChangeTarget(user);
            
            Vector3 direction = marker.transform.InverseTransformPoint(userPosition).normalized;
            float radius = (float) this.m_Radius.Get(this.m_Args);
            
            Vector3 point = marker.transform.TransformPoint(direction * radius);
            
            return point;
        }

        public override Vector3 GetDirection(Marker marker, GameObject user)
        {
            if (user == null) return marker.transform.TransformDirection(Vector3.forward);

            Vector3 point = this.GetPosition(marker, user);
            if (point != marker.transform.position)
            {
                Vector3 direction = marker.transform.position - point;
                direction.y = 0f;
                Debug.DrawLine(marker.transform.position, marker.transform.TransformPoint(direction), Color.magenta);
                return direction.normalized;
            }
            
            return user.transform.TransformDirection(Vector3.forward);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void OnDrawGizmos(Marker marker)
        {
            float scale = MathUtils.Max(
                marker.transform.lossyScale.x,
                marker.transform.lossyScale.y,
                marker.transform.lossyScale.z
            );
            
            GizmosExtension.Circle(
                marker.transform.position + Vector3.up * 0.01f,
                scale * (float) this.m_Radius.EditorValue
            );
        }
    }
}