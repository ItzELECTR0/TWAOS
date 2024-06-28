using GameCreator.Runtime.Common;
using UnityEngine;

namespace Plugins.DaimahouGames.Packages.Core.Runtime.VisualScripting.Properties.Do
{
    [Image(typeof(IconCircleSolid), ColorTheme.Type.Green)]

    public abstract class TPropertyTypeDo
    {
        public abstract string String { get; }
    }
}