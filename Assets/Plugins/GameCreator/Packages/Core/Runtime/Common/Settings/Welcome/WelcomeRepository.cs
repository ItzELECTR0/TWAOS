using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class WelcomeRepository : TRepository<WelcomeRepository>
    {
        public const string REPOSITORY_ID = "core.welcome";
        
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        
        public override string RepositoryID => REPOSITORY_ID;

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private bool m_OpenOnStartup = true;

        [SerializeField] private WelcomeData m_WelcomeData;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool OpenOnStartup => this.m_OpenOnStartup;

        public WelcomeData WelcomeData
        {
            get => this.m_WelcomeData;
            set => this.m_WelcomeData = value;
        }
    }
}