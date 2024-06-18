using System;
using GameCreator.Runtime.Common;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class VariablesRepository : TRepository<VariablesRepository>
    {
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        
        public override string RepositoryID => "core.variables";

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private GlobalVariables m_Variables = new GlobalVariables();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public GlobalVariables Variables => this.m_Variables;
        
        // EDITOR ENTER PLAYMODE: -----------------------------------------------------------------

        #if UNITY_EDITOR
        
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;

        #endif
    }
}