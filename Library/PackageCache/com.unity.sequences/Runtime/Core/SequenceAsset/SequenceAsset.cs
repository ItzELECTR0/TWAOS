namespace UnityEngine.Sequences
{
    /// <summary>
    /// The Sequence package uses the SequenceAsset Component on Regular Prefab and Prefab Variant to distinguish them
    /// from other Prefabs. Once a Prefab has this Component, it appears in the Sequences window, in the Asset
    /// Collections section. You can then use this Prefab in the Sequence Assembly window, and add it to any Sequences
    /// of the Project.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [ComponentHelpURL("sequence-asset")]
    public class SequenceAsset : MonoBehaviour
    {
        [SerializeField] string m_Type = "Character";

        /// <summary>
        /// Indicates the name of the SequenceAsset Collection type.
        /// </summary>
        public string type
        {
            get => m_Type;
            set => m_Type = value;
        }
    }
}
