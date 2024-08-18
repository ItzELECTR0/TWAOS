using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Collect Markers")]
    [Description("Collects all Markers that within a certain radius of a position")]
    
    [Image(typeof(IconMarker), ColorTheme.Type.Teal, typeof(OverlayListVariable))]
    
    [Category("Variables/Collect Markers")]
    
    [Serializable]
    public class InstructionVariablesCollectMarkers : TInstructionVariablesCollect
    {
        [NonSerialized] private List<ISpatialHash> m_Results = new List<ISpatialHash>();
        
        protected override string TitleTarget => "Markers";

        protected override List<GameObject> Collect(Vector3 origin, float maxRadius, float minDistance)
        {
            List<GameObject> result = new List<GameObject>();
            SpatialHashMarkers.Find(origin, maxRadius, this.m_Results);

            foreach (ISpatialHash element in this.m_Results)
            {
                if (Vector3.Distance(element.Position, origin) <= minDistance) continue;
                
                Marker marker = element as Marker;
                if (marker == null) continue;
                
                result.Add(marker.gameObject);
            }

            return result;
        }
    }
}