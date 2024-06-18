using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public abstract class TCopyRunner : MonoBehaviour
    {
        /* Must implement a 'm_Runner' member of type T */
        
        public abstract T GetRunner<T>();
    }
}