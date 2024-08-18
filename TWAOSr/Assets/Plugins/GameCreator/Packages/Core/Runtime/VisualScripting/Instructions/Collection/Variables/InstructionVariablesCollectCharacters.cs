using System;
using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Collect Characters")]
    [Description("Collects all Characters that within a certain radius of a position")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Teal, typeof(OverlayListVariable))]
    
    [Category("Variables/Collect Characters")]
    
    [Serializable]
    public class InstructionVariablesCollectCharacters : TInstructionVariablesCollect
    {
        [NonSerialized] private List<ISpatialHash> m_Results = new List<ISpatialHash>();
        
        protected override string TitleTarget => "Characters";
        
        protected override List<GameObject> Collect(Vector3 origin, float maxRadius, float minDistance)
        {
            List<GameObject> result = new List<GameObject>();
            SpatialHashCharacters.Find(origin, maxRadius, this.m_Results);

            foreach (ISpatialHash element in this.m_Results)
            {
                if (Vector3.Distance(element.Position, origin) <= minDistance) continue;
                
                Character character = element as Character;
                if (character == null) continue;
                
                result.Add(character.gameObject);
            }

            return result;
        }
    }
}