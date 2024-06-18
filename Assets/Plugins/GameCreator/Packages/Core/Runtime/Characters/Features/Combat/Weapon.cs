using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class Weapon
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Id => this.Asset?.Id.Hash ?? IdString.EMPTY.Hash;

        [field: NonSerialized] public IWeapon Asset { get; }
        [field: NonSerialized] public GameObject Instance { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Weapon(IWeapon weapon, GameObject instance)
        {
            this.Asset = weapon;
            this.Instance = instance;
        }
    }
}