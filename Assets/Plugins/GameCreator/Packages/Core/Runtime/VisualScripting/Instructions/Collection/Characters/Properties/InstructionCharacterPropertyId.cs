using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change ID")]
    [Description("Changes the Character's ID")]

    [Category("Characters/Properties/Change ID")]
    [Parameter("ID", "The new ID of the Character")]

    [Keywords("Unique", "Guid")]
    [Image(typeof(IconID), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyId : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetString m_ID = GetStringGuid.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"ID of {this.m_Character} = {this.m_ID}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            string id = this.m_ID.Get(args);
            if (string.IsNullOrEmpty(id)) return DefaultResult;

            IdString idString = new IdString(id);
            character.ChangeId(idString);

            return DefaultResult;
        }
    }
}