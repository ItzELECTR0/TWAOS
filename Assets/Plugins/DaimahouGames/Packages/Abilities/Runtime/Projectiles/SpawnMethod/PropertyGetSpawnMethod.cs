using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Serializable]
    public class PropertyGetSpawnMethod : TPropertyGet<PropertyTypeGetSpawnMethod, Projectile>
    {
        // todo : create dedicated TExecutableProperty to avoid declaration of useless Get methods
        public PropertyGetSpawnMethod(PropertyTypeGetSpawnMethod defaultType) : base(defaultType) {}

        public void Spawn(ExtendedArgs args, Projectile projectile) => m_Property.Spawn(args, projectile);

        public string String => m_Property == null ? "(none)" : m_Property.String;
    }
}