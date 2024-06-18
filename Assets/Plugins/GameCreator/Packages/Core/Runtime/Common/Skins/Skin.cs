using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public abstract class Skin : ScriptableObject
    {
        public virtual string Description => string.Empty;
        public virtual string HasError => string.Empty;
    }
}