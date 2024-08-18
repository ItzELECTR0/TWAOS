using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("String ID")]
    [Category("Constants/String ID")]
    
    [Image(typeof(IconID), ColorTheme.Type.Yellow)]
    [Description("Returns an alphanumeric string without any spaces")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringId : PropertyTypeGetString
    {
        [SerializeField] private IdString m_Id;
        
        public override string Get(Args args) => this.m_Id.String;
        
        public override string Get(GameObject gameObject) => this.m_Id.String;

        public GetStringId()
        { }

        public GetStringId(string name)
        {
            this.m_Id = new IdString(name);
        }
        
        public static PropertyGetString Create(string name) => new PropertyGetString(
            new GetStringId(name)
        );

        public override string String => string.IsNullOrEmpty(this.m_Id.String)
            ? "<empty>"
            : this.m_Id.String;

        public override string EditorValue => this.m_Id.String;
    }
}