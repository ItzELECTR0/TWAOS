using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Spawn Method")]

    [Serializable]
    public abstract class PropertyTypeGetSpawnMethod : TPropertyTypeGet<Projectile>
    {
        public abstract void Spawn(ExtendedArgs args, Projectile projectile);
    }
}