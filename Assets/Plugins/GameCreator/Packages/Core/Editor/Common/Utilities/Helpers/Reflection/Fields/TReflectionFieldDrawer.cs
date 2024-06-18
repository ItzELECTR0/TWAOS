using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    public abstract class TReflectionFieldDrawer : TReflectionMemberDrawer
    {
        protected override bool DisableInPlaymode => false;
        
        protected override List<string> GetList(Component component)
        {
            List<string> names = new List<string>();
            if (component == null) return names;

            FieldInfo[] fields = component.GetType().GetFields(BINDINGS);
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsAssignableFrom(this.AcceptType)) continue;
                names.Add(field.Name);
            }

            return names;
        }
    }
}