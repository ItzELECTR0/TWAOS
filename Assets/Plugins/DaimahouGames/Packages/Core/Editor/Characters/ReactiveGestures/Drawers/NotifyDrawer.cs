using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Editor.Core;
using UnityEditor;

namespace DaimahouGames.Editor.Characters
{
    [CustomPropertyDrawer(typeof(TNotifyBase), true)]
    public class NotifyDrawer : GenericItemDrawer
    {
        protected override IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return base.GetIgnoredField(property).Concat(new[] {"m_TriggerTime"});
        }
    }
}