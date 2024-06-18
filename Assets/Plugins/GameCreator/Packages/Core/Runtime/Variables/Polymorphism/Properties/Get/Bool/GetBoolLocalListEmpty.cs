using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Empty Local List Variable")]
    [Category("Variables/Empty Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns true if the Local List Variable is empty")]

    [Serializable]
    public class GetBoolLocalListEmpty : PropertyTypeGetBool
    {
        [SerializeField] private PropertyGetGameObject m_List = new PropertyGetGameObject();

        public override bool Get(Args args)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(args);
            return (list != null ? list.Count : 0) == 0;
        }

        public override bool Get(GameObject gameObject)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(gameObject);
            return (list != null ? list.Count : 0) == 0;
        }

        public override string String => $"{this.m_List} is Empty";
    }
}