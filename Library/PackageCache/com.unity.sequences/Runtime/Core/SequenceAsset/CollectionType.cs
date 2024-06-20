using System.Collections.Generic;

namespace UnityEngine.Sequences
{
    class CollectionType
    {
        /// <summary>
        /// Default Sequence Asset Collection Types provided by the package.
        /// </summary>
        static readonly string[] s_DefaultTypes = new string[]
        {
            "Character",
            "Fx",
            "Lighting",
            "Photography",
            "Prop",
            "Set",
            "Audio"
        };

        static CollectionType s_Instance = null;
        readonly List<string> m_Types = new List<string>(s_DefaultTypes);

        public static CollectionType instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new CollectionType();

                return s_Instance;
            }
        }

        public IReadOnlyList<string> types => m_Types;
        public static string undefined => "Undefined";

        public string[] GetTypes()
        {
            return m_Types.ToArray();
        }
    }
}
