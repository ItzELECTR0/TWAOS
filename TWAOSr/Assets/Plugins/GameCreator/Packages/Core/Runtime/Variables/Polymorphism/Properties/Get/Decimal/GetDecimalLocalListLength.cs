using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Count of Local List Variable")]
    [Category("Variables/Count of Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the amount of elements of a Local List Variable")]

    [Serializable]
    public class GetDecimalLocalListLength : PropertyTypeGetDecimal
    {
        [SerializeField] private PropertyGetGameObject m_List = new PropertyGetGameObject();

        public override double Get(Args args)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(args);
            return list != null ? list.Count : 0;
        }

        public override double Get(GameObject gameObject)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(gameObject);
            return list != null ? list.Count : 0;
        }

        public override string String => $"{this.m_List} Length";
    }
}