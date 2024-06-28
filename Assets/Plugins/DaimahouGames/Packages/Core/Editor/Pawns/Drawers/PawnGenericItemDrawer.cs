using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using PrototypeCreator.Editor.Core;
using UnityEditor;

namespace DaimahouGames.Core.Editor.Pawns
{
    public class PawnGenericItemDrawer : GenericItemDrawer
    {
        //============================================================================================================||
        // -----------------------------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            var ignore = base.GetIgnoredField(property).ToList();
            
            var pawnProperty = property.FindPropertyRelative(PawnEditor.PAWN_FIELD);
            if (pawnProperty.objectReferenceValue != null) ignore.Add(PawnEditor.PAWN_FIELD);

            return ignore;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}