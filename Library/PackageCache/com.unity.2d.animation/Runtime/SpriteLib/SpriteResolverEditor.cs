#if UNITY_EDITOR
using System;
using UnityEditor;

namespace UnityEngine.U2D.Animation
{
    public partial class SpriteResolver : ISerializationCallbackReceiver
    {
        internal static string spriteHashPropertyName => nameof(m_SpriteHash);
        
        bool m_SpriteLibChanged;

        /// <summary>
        /// Raised when object is deserialized in the Editor.
        /// </summary>
        public event Action onDeserializedCallback = () => { };

        void OnDidApplyAnimationProperties()
        {
            if (IsInGUIUpdateLoop())
                ResolveUpdatedValue();
        }

        internal bool spriteLibChanged
        {
            get => m_SpriteLibChanged;
            set => m_SpriteLibChanged = value;
        }

        /// <summary>
        /// Called before object is serialized.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <summary>
        /// Called after object is deserialized.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            onDeserializedCallback();
        }
    }
}
#endif
