using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Slot Save Date")]
    [Category("Storage/Slot Save Date")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Green)]
    [Description("Returns the save date of a slot in the specified format")]
    
    [Example("General: 01/28/1990 00:01")]
    [Example("Short Date: 01/28/1990")]
    [Example("Long Date: Sunday, 28 January 1990")]
    
    [Serializable]
    public class GetStringSaveDate : PropertyTypeGetString
    {
        private enum DateFormat
        {
            General,
            ShortDate,
            LongDate
        }
        
        [SerializeField] private PropertyGetInteger m_Slot = new PropertyGetInteger(1);
        [SerializeField] private DateFormat m_Format = DateFormat.General;

        public override string Get(Args args)
        {
            int slot = (int) this.m_Slot.Get(args);
            string value = SaveLoadManager.Instance.GetSaveDate(slot);

            if (string.IsNullOrEmpty(value)) return string.Empty;
            DateTime date = DateTime.Parse(value);
            
            return this.m_Format switch
            {
                DateFormat.General => date.ToString("g"),
                DateFormat.ShortDate => date.ToString("d"),
                DateFormat.LongDate => date.ToString("D"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringSaveDate()
        );

        public override string String => $"Slot {m_Slot} Date";
    }
}