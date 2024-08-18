using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class GeneralRepository : TRepository<GeneralRepository>
    {
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        
        public override string RepositoryID => "core.general";

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private GeneralSave m_Save = new GeneralSave();
        [SerializeField] private GeneralAudio m_Audio = new GeneralAudio();

        // PROPERTIES: ----------------------------------------------------------------------------

        public GeneralAudio Audio => this.m_Audio;
        public GeneralSave Save => this.m_Save;

        // EDITOR ENTER PLAYMODE: -----------------------------------------------------------------

        #if UNITY_EDITOR
        
        [InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode() => Instance = null;
        
        #endif
    }
}