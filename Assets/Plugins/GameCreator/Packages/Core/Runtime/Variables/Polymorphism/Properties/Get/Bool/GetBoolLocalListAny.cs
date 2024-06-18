using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Any Local List Variable")]
    [Category("Variables/Any Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns true if the Local List Variable has at least one element")]

    [Serializable]
    public class GetBoolLocalListAny : PropertyTypeGetBool
    {
        [SerializeField] private PropertyGetGameObject m_List = new PropertyGetGameObject();

        public override bool Get(Args args)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(args);
            return (list != null ? list.Count : 0) > 0;
        }

        public override bool Get(GameObject gameObject)
        {
            LocalListVariables list = this.m_List.Get<LocalListVariables>(gameObject);
            return (list != null ? list.Count : 0) > 0;
        }

        public override string String => $"Any in {this.m_List}";
    }
}