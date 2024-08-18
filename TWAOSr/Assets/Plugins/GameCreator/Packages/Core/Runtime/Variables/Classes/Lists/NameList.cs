using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class NameList : TList<NameVariable>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public string[] Names
        {
            get
            {
                string[] names = new string[this.Length];
                for (int i = 0; i < this.Length; ++i)
                {
                    names[i] = this.Get(i).Name;
                }

                return names;
            }
        }

        public IdString[] TypeIds
        {
            get
            {
                IdString[] typeIds = new IdString[this.Length];
                for (int i = 0; i < this.Length; ++i)
                {
                    typeIds[i] = this.Get(i).TypeID;
                }

                return typeIds;
            }
        }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public NameList() : base()
        { }
        
        public NameList(params NameVariable[] variables) : base(variables)
        { }
    }
}