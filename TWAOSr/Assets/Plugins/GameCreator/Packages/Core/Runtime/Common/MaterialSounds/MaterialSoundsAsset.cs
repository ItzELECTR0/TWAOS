using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [CreateAssetMenu(
        fileName = "Material Sounds", 
        menuName = "Game Creator/Common/Material Sounds",
        order = 50
    )]
    [Icon(RuntimePaths.GIZMOS + "GizmoMaterialSounds.png")]
    public class MaterialSoundsAsset : ScriptableObject
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private MaterialSoundsData m_MaterialSounds = new MaterialSoundsData();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public MaterialSoundsData MaterialSounds => m_MaterialSounds;
    }
}
