using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change ID")]
    [Description("Changes the Local Name or List Variable's ID. It only works on non-Savable variables")]

    [Category("Variables/Change ID")]
    [Parameter("ID", "The new ID of the Local Variable")]

    [Keywords("Unique", "Guid")]
    [Image(typeof(IconID), ColorTheme.Type.Purple)]

    [Serializable]
    public class InstructionVariablesChangeId : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_LocalVariables = GetGameObjectInstance.Create();
        
        [SerializeField]
        private PropertyGetString m_ID = GetStringGuid.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"ID of {this.m_LocalVariables} = {this.m_ID}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            TLocalVariables variables = this.m_LocalVariables.Get<TLocalVariables>(args);
            if (variables == null) return DefaultResult;

            string id = this.m_ID.Get(args);
            if (string.IsNullOrEmpty(id)) return DefaultResult;

            IdString idString = new IdString(id);
            variables.ChangeId(idString);

            return DefaultResult;
        }
    }
}