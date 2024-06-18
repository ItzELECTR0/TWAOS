using System;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class AssetDate
    {
        private static readonly string[] MONTHS = {
            "Unknown",
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private int day;
        [SerializeField] private int month;
        [SerializeField] private int year;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Day => this.day;
        public string Month => MONTHS[this.month];
        public int Year => this.year;
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => $"{this.Month} {this.Day}, {this.Year}";
    }
}