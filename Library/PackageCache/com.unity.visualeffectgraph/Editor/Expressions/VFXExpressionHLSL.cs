using System;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace UnityEditor.VFX
{
#pragma warning disable 0659

    class VFXExpressionHLSL : VFXExpression, IHLSLCodeHolder
    {
        string m_FunctionName;
        VFXValueType m_ValueType;
        string m_HlslCode;
        List<int> m_TextureParentExpressionIndex;
        string[] m_Includes;

        public VFXExpressionHLSL() : this(string.Empty, string.Empty, VFXValueType.None, new [] { VFXValue<int>.Default }, Array.Empty<string>())
        {
        }

        public VFXExpressionHLSL(string functionName, string hlslCode, VFXValueType returnType, VFXExpression[] parents, string[] includes) : base(Flags.InvalidOnCPU, parents)
        {
            this.m_TextureParentExpressionIndex = new List<int>();
            this.m_FunctionName = functionName;
            this.m_ValueType = returnType;
            this.m_HlslCode = hlslCode;
            this.m_Includes = includes;
        }

        public VFXExpressionHLSL(string functionName, string hlslCode, System.Type returnType, VFXExpression[] parents, string[] includes) : base(Flags.InvalidOnCPU, parents)
        {
            this.m_TextureParentExpressionIndex = new List<int>();
            this.m_FunctionName = functionName;
            this.m_ValueType = GetVFXValueTypeFromType(returnType);
            this.m_HlslCode = hlslCode;
            this.m_Includes = includes;
        }

        public override VFXValueType valueType => m_ValueType;
        public override VFXExpressionOperation operation => VFXExpressionOperation.None;
        public ShaderInclude shaderFile => null;
        public string sourceCode { get; set; }
        public string customCode => m_HlslCode;
        public IEnumerable<string> includes => m_Includes;

        public bool HasShaderFile() => false;
        public bool Equals(IHLSLCodeHolder other) => false;

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is VFXExpressionHLSL hlslExpression)
            {
                if (hlslExpression.m_Includes.Length == m_Includes.Length)
                    return false;

                for (var index = 0; index < m_Includes.Length; ++index)
                {
                    if (hlslExpression.m_Includes[index] != m_Includes[index])
                        return false;
                }

                return hlslExpression.m_FunctionName == m_FunctionName
                       && hlslExpression.valueType == valueType
                       && hlslExpression.m_HlslCode == m_HlslCode;
            }

            return false;
        }

        protected override int GetInnerHashCode()
        {
            var hash = base.GetInnerHashCode();
            hash = HashCode.Combine(hash, m_FunctionName.GetHashCode(), m_ValueType.GetHashCode(), m_HlslCode.GetHashCode());
            foreach (var include in m_Includes)
                hash = HashCode.Combine(hash, include);
            return hash;
        }

        protected override VFXExpression Reduce(VFXExpression[] reducedParents)
        {
            m_TextureParentExpressionIndex.Clear();
            for (int i = 0; i < reducedParents.Length; i++)
            {
                if (IsTexture(reducedParents[i].valueType))
                {
                    m_TextureParentExpressionIndex.Add(i);
                }
            }

            var newExpression = (VFXExpressionHLSL)base.Reduce(reducedParents);
            newExpression.m_FunctionName = m_FunctionName;
            newExpression.m_ValueType = m_ValueType;
            newExpression.m_HlslCode = m_HlslCode;
            newExpression.m_TextureParentExpressionIndex = new List<int>(m_TextureParentExpressionIndex);
            newExpression.m_Includes = (string[])m_Includes.Clone();
            return newExpression;
        }

        public sealed override string GetCodeString(string[] parentsExpressions)
        {
            foreach (var index in m_TextureParentExpressionIndex)
            {
                var expression = parentsExpressions[index];
                parentsExpressions[index] = $"VFX_SAMPLER({expression})";
            }
            return $"{m_FunctionName}({string.Join(", ", parentsExpressions)})";
        }
    }
}
