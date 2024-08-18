using System;
using UnityEngine;

namespace GameCreator.Editor.Installs
{
    [Serializable]
    public class Install
    {
        // ENUMS: ---------------------------------------------------------------------------------
        
        public enum ComplexityType
        {
            StartHere    = 0,
            Beginner     = 1,
            Intermediate = 2,
            Advanced     = 3,
            None         = 100,
            Skin         = 200
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private string m_Name;
        [SerializeField] private string m_Module;
        
        [TextArea(5, 50)]
        [SerializeField] private string m_Description;
        [SerializeField] private string m_Author;
        
        [SerializeField] private Version m_Version;
        [SerializeField] private ComplexityType m_Complexity;
        
        [SerializeField] private Dependency[] m_Dependencies;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public string Name => m_Name;
        public string Module => m_Module;
        
        public string ID => string.Format(
            "{0}.{1}", 
            this.Module.Replace(" ", string.Empty),
            this.Name.Replace(" ", string.Empty)
        );
        
        public string Description => m_Description;
        public string Author => m_Author;

        public Version Version => m_Version;
        public ComplexityType Complexity => m_Complexity;

        public Dependency[] Dependencies => m_Dependencies;
    }
}