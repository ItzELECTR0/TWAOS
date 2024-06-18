using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Serializable]
    public class Slots : TSerializableDictionary<int, Slots.Data>, IGameSave
    {
        [Serializable]
        public struct Data
        {
            public string date;
            public string[] keys;
        }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int LatestSlot
        {
            get
            {
                int lastSlot = -1;
                DateTime lastDateTime = DateTime.MinValue;

                foreach (KeyValuePair<int, Data> entry in this)
                {
                    if (!DateTime.TryParse(entry.Value.date, out DateTime dateTime)) continue;
                    if (DateTime.Compare(lastDateTime, dateTime) > 0) continue;

                    lastSlot = entry.Key;
                    lastDateTime = dateTime;
                }

                return lastSlot;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(int slot, string[] keys)
        {
            this[slot] = new Data
            {
                date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                keys = keys
            };
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => "slots";
        public bool IsShared => true;

        public LoadMode LoadMode => LoadMode.Greedy;

        public Type SaveType => typeof(Slots);
        
        public object GetSaveData(bool includeNonSavable)
        {
            return this;
        }

        public Task OnLoad(object value)
        {
            Slots slots = value as Slots;
            this.m_Dictionary = slots?.m_Dictionary ?? new Dictionary<int, Data>();
            
            return Task.FromResult(true);
        }
    }
}