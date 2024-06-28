using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Core.Editor.Pawns;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using UnityEditor;

namespace DaimahouGames.Abilities.Editor.Pawns
{
    [CustomPropertyDrawer(typeof(Caster), true)]
    public class CasterDrawer : PawnGenericItemDrawer
    {
        private const string SLOTS = "m_AbilitySlots";
        
        protected override void CreateGUI(SerializedProperty property)
        {
            base.CreateGUI(property);

            var slotsProperty = property.FindPropertyRelative(SLOTS);
            var slotsInspector = new ListInspector<Caster.KnownAbility>(slotsProperty)
            {
                ItemPrefix = "# ",
                PrintItemNumber = true
            };
            slotsInspector.SetAddButtonText("Add new slot");

            slotsInspector.Refresh();
            m_Root.Add(slotsInspector);
        }

        protected override IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return base.GetIgnoredField(property).Concat(new []{SLOTS});
        }
    }
}