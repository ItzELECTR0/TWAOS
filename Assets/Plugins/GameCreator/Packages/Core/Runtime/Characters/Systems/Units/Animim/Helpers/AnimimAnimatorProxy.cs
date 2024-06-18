using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [AddComponentMenu("")]
    internal class AnimimAnimatorProxy : MonoBehaviour
    {
        [field: NonSerialized]
        public TUnitAnimim Animim { private get; set; }

        // METHODS: -------------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            this.Animim?.OnAnimatorIK(layerIndex);   
        }

        private void OnAnimatorMove()
        {
            this.Animim?.OnAnimatorMove();
        }
    }
}