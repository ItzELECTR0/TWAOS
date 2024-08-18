using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Material value of a Local Name Variable")]

    [Serializable]
    public class GetMaterialLocalName : PropertyTypeGetMaterial
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueMaterial.TYPE_ID);

        public override Material Get(Args args) => this.m_Variable.Get<Material>(args);

        public override string String => this.m_Variable.ToString();
    }
}
