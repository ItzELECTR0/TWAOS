using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Sort List by Distance")]
    [Description("Sorts the List Variable elements based on their distance to a given position")]
    
    [Image(typeof(IconSort), ColorTheme.Type.Teal)]
    
    [Category("Variables/Sort List by Distance")]
    
    [Parameter("List Variable", "Local List or Global List which elements are sorted")]
    [Parameter("Position", "The reference position that is used to measure the sorting distance")]
    [Parameter("Order", "From Closest to Farthest puts the closest elements to the Position first")]

    [Keywords("Order", "Organize", "Array", "List", "Variables")]
    [Serializable]
    public class InstructionVariablesSortDistance : Instruction
    {
        private enum Order
        {
            ClosestToFarthest,
            FarthestToClosest
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] 
        private CollectorListVariable m_ListVariable = new CollectorListVariable();

        [SerializeField] 
        private PropertyGetPosition m_Position = GetPositionCharacter.Create;
        
        [SerializeField]
        private Order m_Order = Order.ClosestToFarthest;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Args m_Args;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => 
            $"Sort {this.m_ListVariable} by Distance to {this.m_Position}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            List<object> elements = this.m_ListVariable.Get(args);

            this.m_Args = args;
            elements.Sort(this.SortingMethod);

            this.m_ListVariable.Fill(elements.ToArray(), args);
            return DefaultResult;
        }

        private int SortingMethod(object a, object b)
        {
            IdString type = this.m_ListVariable.GetTypeId(this.m_Args);
            Vector3 position = this.m_Position.Get(this.m_Args);

            if (type.Hash == ValueVector3.TYPE_ID.Hash)
            {
                float vector3A = Vector3.Distance(position, (Vector3) a);
                float vector3B = Vector3.Distance(position, (Vector3) b);
                
                return this.m_Order == Order.ClosestToFarthest
                    ? vector3A.CompareTo(vector3B)
                    : vector3B.CompareTo(vector3A);
            }
            
            if (type.Hash == ValueGameObject.TYPE_ID.Hash)
            {
                GameObject gameObjectA = a as GameObject;
                GameObject gameObjectB = b as GameObject;
                
                if (gameObjectA == null && gameObjectB == null) return 0;
                if (gameObjectA == null) return +1;
                if (gameObjectB == null) return -1;
                
                float distanceA = Vector3.Distance(position, gameObjectA.transform.position);
                float distanceB = Vector3.Distance(position, gameObjectB.transform.position);
                
                return this.m_Order == Order.ClosestToFarthest
                    ? distanceA.CompareTo(distanceB)
                    : distanceB.CompareTo(distanceA);
            }

            return 0;
        }
    }
}