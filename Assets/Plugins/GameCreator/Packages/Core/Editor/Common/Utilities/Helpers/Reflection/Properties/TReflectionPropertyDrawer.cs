using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    public abstract class TReflectionPropertyDrawer : TReflectionMemberDrawer
    {
        protected override bool DisableInPlaymode => true;

        protected override List<string> GetList(Component component)
        {
            List<string> names = new List<string>();
            if (component == null) return names;

            PropertyInfo[] properties = component.GetType().GetProperties(BINDINGS);
            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.IsAssignableFrom(this.AcceptType)) continue;
                names.Add(property.Name);
            }

            return names;
        }
    }
}