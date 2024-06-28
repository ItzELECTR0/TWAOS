using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DaimahouGames.Runtime.Abilities
{
    [Serializable]
    public class AbilitiesRepository : TRepository<AbilitiesRepository>
    {
        internal const string REPOSITORY_ID = "abilities.general";
        
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        
        public override string RepositoryID => REPOSITORY_ID;

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private AbilitiesCatalogue m_Abilities = new AbilitiesCatalogue();

        // PROPERTIES: ----------------------------------------------------------------------------

        public AbilitiesCatalogue Abilities => this.m_Abilities;
        
        // EDITOR ENTER PLAYMODE: -----------------------------------------------------------------

        #if UNITY_EDITOR
        
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;
        
        #endif
    }
}