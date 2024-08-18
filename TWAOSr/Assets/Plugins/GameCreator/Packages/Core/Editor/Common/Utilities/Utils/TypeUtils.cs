using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    public static class TypeUtils
    {
        public enum Sort
        {
            ByType = 0,
            ByTitle = 1,
            ByCategory = 2
        }
        
        private static readonly char[] ASSEMBLY_SEPARATOR = { ' ' };

        private static Dictionary<int, Type> CACHE_ASSEMBLIES_TYPES;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private static readonly Comparison<Type>[] Comparisons =
        {
            CompareByType,
            CompareByTitle,
            CompareByCategory
        };
        
        private static readonly char[] SEPARATOR = { '.' };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static string GetNiceName(Type type)
        {
            return GetNiceName(type.ToString());
        }

        public static string GetNiceName(string type)
        {
            string[] split = type.Split(SEPARATOR);
            return split.Length > 0 
                ? TextUtils.Humanize(split[^1]) 
                : string.Empty;
        }

        public static Type[] GetTypesDerivedFrom(Type type, Sort sort = Sort.ByTitle)
        {
            Type[] types = GetDerivedTypes(type);
            Array.Sort(types, Comparisons[(int)sort]);

            return types;
        }

        public static Trie<Type> GetTypesTree(Type typeBase)
        {
            Trie<Type> trie = Trie<Type>.Create();
            Type[] types = GetDerivedTypes(typeBase);

            foreach (Type type in types)
            {
                CategoryAttribute category = type
                    .GetCustomAttributes<CategoryAttribute>(true)
                    .FirstOrDefault();

                string[] paths = category?.Path ?? Array.Empty<string>();
                string name = category != null && string.IsNullOrEmpty(category.Name)
                    ? category.Name
                    : TextUtils.Humanize(type.ToString());

                Trie<Type> subTrie = trie;
                foreach (string section in paths)
                {
                    if (!subTrie.Children.TryGetValue(section, out Trie<Type> child))
                    {
                        child = subTrie.AddChild(new Trie<Type>(section, null));
                    }

                    subTrie = child;
                }

                subTrie.AddChild(new Trie<Type>(name, type));
            }

            return trie;
        }
        
        public static string GetTitleFromType(Type type, HashSet<string> forbiddenNames = null)
        {
            if (type == null) return "(none)";
            TitleAttribute title = type.GetCustomAttributes<TitleAttribute>().FirstOrDefault();

            string titleName = title != null && !string.IsNullOrEmpty(title.Title)
                ? title.Title
                : GetNiceName(type);

            if (forbiddenNames == null) return titleName;
            if (string.IsNullOrEmpty(titleName)) return titleName;

            int number = 1;
            string complete = titleName;

            while (forbiddenNames.Contains(complete))
            {
                complete = $"{titleName} ({number})";
                number += 1;
            }

            return complete;
        }

        public static Type GetTypeFromProperty(SerializedProperty property, bool fullType)
        {
            if (property == null)
            {
                Debug.LogError("Null property was found at 'GetTypeFromProperty'");
                return null;
            }
            
            string[] split = fullType
                ? property.managedReferenceFullTypename.Split(ASSEMBLY_SEPARATOR)
                : property.managedReferenceFieldTypename.Split(ASSEMBLY_SEPARATOR);

            return split.Length != 2 
                ? null : 
                Type.GetType(Assembly.CreateQualifiedName(split[0], split[1]));
        }

        public static Type GetTypeFromName(string typeName)
        {
            RequireAssembliesTypesSetup();
            return CACHE_ASSEMBLIES_TYPES.TryGetValue(typeName.GetHashCode(), out Type typeFound)
                ? typeFound
                : null;
        }
        
        // ASSEMBLIES TYPES CACHE: ----------------------------------------------------------------

        private static void RequireAssembliesTypesSetup()
        {
            if (CACHE_ASSEMBLIES_TYPES != null) return;
            CACHE_ASSEMBLIES_TYPES = new Dictionary<int, Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    CACHE_ASSEMBLIES_TYPES[type.Name.GetHashCode()] = type;
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Type[] GetDerivedTypes(Type type)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();
            
            if (type == null) return types.ToArray();
            
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type assemblyType in assemblyTypes)
                {
                    if (assemblyType.IsInterface) continue;
                    if (assemblyType.IsAbstract) continue;
                    if (type.IsAssignableFrom(assemblyType)) types.Add(assemblyType);   
                }
            }

            return types.ToArray();
        }

        private static int CompareByType(Type a, Type b)
        {
            return string.CompareOrdinal(a.ToString(), b.ToString());
        }

        private static int CompareByTitle(Type a, Type b)
        {
            return string.CompareOrdinal(
                a.GetCustomAttributes<TitleAttribute>(true).FirstOrDefault()?.Title, 
                b.GetCustomAttributes<TitleAttribute>(true).FirstOrDefault()?.Title
            );
        }

        private static int CompareByCategory(Type a, Type b)
        {
            return string.CompareOrdinal(
                a.GetCustomAttributes<CategoryAttribute>(true).FirstOrDefault()?.ToString(), 
                b.GetCustomAttributes<CategoryAttribute>(true).FirstOrDefault()?.ToString()
            );
        }
    }
}