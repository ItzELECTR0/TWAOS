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
        [SerializeField] private float m_Radius = 0.5f;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Vector3 GetPosition(Marker marker, GameObject user)
        {
            if (user == null) return marker.transform.position;

            Character userCharacter = user.Get<Character>();
            Vector3 userPosition = userCharacter != null 
                ? userCharacter.Feet 
                : user.transform.position;
            
            Vector3 direction = marker.transform.InverseTransformPoint(userPosition).normalized;
            Vector3 point = marker.transform.TransformPoint(direction * this.m_Radius);
            
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
            
            Debug.DrawLine(marker.transform.position, marker.transform.TransformPoint(user.transform.TransformDirection(Vector3.forward)), Color.magenta);
            return user.transform.TransformDirection(Vector3.forward);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        public override void OnDrawGizmos(Marker marker)
        {
            GizmosExtension.Circle(
                marker.transform.position + Vector3.up * 0.01f,
                this.m_Radius
            );
        }
    }
}