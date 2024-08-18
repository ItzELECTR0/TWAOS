using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Editor.Installs
{
    [Icon(RuntimePaths.GIZMOS + "GizmoInstaller.png")]
    public class Installer : ScriptableObject
    {
        // +--------------------------------------------------------------------------------------+
        // | An Asset is uniquely identified by its ID value, which is the concatenation of its   |
        // | module name, the category and name: [module].[category].[name]                       |
        // |                                                                                      |
        // | For example:                                                                         |
        // | Core.Examples.BasicScene                                                             | 
        // | Core.Tools.Character                                                                 |
        // |                                                                                      |
        // | The .unitypackage must be named after the same ID value and be next to this asset    |
        // +--------------------------------------------------------------------------------------+

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Install m_Install = new Install();

        // PROPERTIES: ----------------------------------------------------------------------------

        public Install Data => this.m_Install;
        
        // CONTEXT METHODS: -----------------------------------------------------------------------
        
        [ContextMenu("Install Package")]
        public void Install()
        {
            InstallManager.Install(this);
        }
        
        [ContextMenu("Delete Package")]
        public void Delete()
        {
            InstallManager.Delete(this.Data.ID);
        }
        
        [ContextMenu("Build Package")]
        public void Build()
        {
            InstallManager.Build(this);
        }
        
        [ContextMenu("Create Package")]
        public void Create()
        {
            InstallManager.Create(this);
        }
    }
}
