using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Material value of a Local List Variable")]

    [Serializable]
    public class GetMaterialLocalList : PropertyTypeGetMaterial
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueMaterial.TYPE_ID);

        public override Material Get(Args args) => this.m_Variable.Get<Material>(args);

        public override string String => this.m_Variable.ToString();
    }
}
