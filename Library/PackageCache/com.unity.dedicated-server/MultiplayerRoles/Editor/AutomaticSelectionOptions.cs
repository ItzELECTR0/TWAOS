using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Unity.Multiplayer.Editor
{
    [Serializable]
    internal struct AutomaticSelectionOptions
    {
        private Dictionary<Type, MultiplayerRoleFlags> m_CompleteComponentsList;
        internal Dictionary<Type, MultiplayerRoleFlags> CompleteComponentsList
        {
            get
            {
                if (m_CompleteComponentsList == null)
                    BakeCompleteComponentsList();

                return m_CompleteComponentsList;
            }
        }

        [SerializeField] private bool m_StripRenderComponents;
        [SerializeField] private bool m_StripUIComponents;
        [SerializeField] private bool m_StripAudioComponents;

        public bool StripRenderingComponents
        {
            get => m_StripRenderComponents;
            set
            {
                // Don't bake if value didn't change
                if (m_StripRenderComponents == value)
                    return;

                m_StripRenderComponents = value;
                BakeCompleteComponentsList();
            }
        }
        public bool StripUIComponents
        {
            get => m_StripUIComponents;
            set
            {
                // Don't bake if value didn't change
                if (m_StripUIComponents == value)
                    return;

                m_StripUIComponents = value;
                BakeCompleteComponentsList();
            }
        }
        public bool StripAudioComponents
        {
            get => m_StripAudioComponents;
            set
            {
                // Don't bake if value didn't change
                if (m_StripAudioComponents == value)
                    return;

                m_StripAudioComponents = value;
                BakeCompleteComponentsList();
            }
        }

        [SerializeField] private SerializedDictionary<SerializedType, MultiplayerRoleFlags> m_CustomComponentsList;
        private SerializedDictionary<SerializedType, MultiplayerRoleFlags> CustomComponentsList
        {
            get
            {
                if (m_CustomComponentsList == null)
                    m_CustomComponentsList = new();

                return m_CustomComponentsList;
            }
            set
            {
                m_CustomComponentsList = value;
                BakeCompleteComponentsList();
            }
        }

        public Dictionary<Type, MultiplayerRoleFlags> GetCustomComponents()
        {
            var dictionary = new Dictionary<Type, MultiplayerRoleFlags>();

            foreach (var kvp in CustomComponentsList)
                dictionary[kvp.Key.Value] = kvp.Value;

            return dictionary;
        }

        public void SetCustomComponents(Dictionary<Type, MultiplayerRoleFlags> customComponents)
        {
            CustomComponentsList.Clear();

            if (customComponents != null)
                foreach (var kvp in customComponents)
                    SetCustomComponentMultiplayerRoleFlags(kvp.Key, kvp.Value, false);

            BakeCompleteComponentsList();
        }

        public MultiplayerRoleFlags GetMultiplayerRoleMaskForComponentType(Type type)
            => GetMultiplayerRoleFlagsForType(type);

        public void SetMultiplayerRoleMaskForComponentType(Type type, MultiplayerRoleFlags mask)
            => SetCustomComponentMultiplayerRoleFlags(type, mask, true);

        internal void SetCustomComponentMultiplayerRoleFlags(Type type, MultiplayerRoleFlags target, bool bake = true)
        {
            if (!type.IsSubclassOf(typeof(Component)))
                throw new ArgumentException("Only subclasses of Component can be selected.");

            if (target == MultiplayerRoleFlags.ClientAndServer)
                CustomComponentsList.Remove(new SerializedType(type));
            else
                CustomComponentsList[new SerializedType(type)] = target;

            if (bake)
                BakeCompleteComponentsList();
        }

        internal AutomaticSelectionOptions Clone()
        {
            var options = this;
            options.m_CustomComponentsList = null;
            options.m_CompleteComponentsList = null;
            options.SetCustomComponents(this.GetCustomComponents());

            return options;
        }

        internal static void GetBuiltinStrippingComponentsForServer(in AutomaticSelectionOptions options, List<Type> types)
        {
            if (options.StripRenderingComponents)
            {
                var components = TypeCache.GetTypesDerivedFrom<Renderer>()
                    .Concat(new[]
                    {
                        typeof(Camera),
                        typeof(Light),
                    });

#if UNITY_RENDER_PIPELINE_PRESENT
                components = components.Concat(TypeCache.GetTypesDerivedFrom<UnityEngine.Rendering.IAdditionalData>());
#endif

                foreach (var component in components)
                    types.Add(component);
            }

            if (options.StripUIComponents)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where
                    (
                        a =>
                            a.GetName().Name == "UnityEngine.UI" ||
                            a.GetName().Name == "UnityEngine.Canvas" ||
                            a.GetName().Name == "UnityEngine.UIElementsModule"
                    );

                var components = assemblies.SelectMany(a => a.GetTypes().Where(t => IsClassOrSubclassOf(t, typeof(Component))));

                foreach (var component in components)
                    types.Add(component);
            }

            if (options.StripAudioComponents)
            {
                var assembly = typeof(AudioClip).Assembly;
                var components = assembly.GetTypes().Where(t => IsClassOrSubclassOf(t, typeof(Component)));
                foreach (var component in components)
                    types.Add(component);
            }
        }

        private void BakeCompleteComponentsList()
        {
            if (m_CompleteComponentsList == null)
                m_CompleteComponentsList = new();

            m_CompleteComponentsList.Clear();

            var builtinServerTypes = new List<Type>();
            GetBuiltinStrippingComponentsForServer(this, builtinServerTypes);

            foreach (var type in builtinServerTypes)
            {
                m_CompleteComponentsList[type] = MultiplayerRoleFlags.ClientAndServer & ~MultiplayerRoleFlags.Server;
            }

            var roleValues = Enum.GetValues(typeof(MultiplayerRole));

            foreach (var component in CustomComponentsList)
            {
                if (!m_CompleteComponentsList.TryGetValue(component.Key.Value, out var maskValue))
                    maskValue = MultiplayerRoleFlags.ClientAndServer;

                maskValue &= component.Value;
                m_CompleteComponentsList[component.Key.Value] = maskValue;
            }
        }

        internal static bool IsClassOrSubclassOf(Type type, Type parentType)
            => type == parentType || type.IsSubclassOf(parentType);

        internal bool IsComponentSelected(Type type)
            => CompleteComponentsList.Keys.Any(selectedType => IsClassOrSubclassOf(type, selectedType));

        internal IEnumerable<KeyValuePair<Type, MultiplayerRoleFlags>> GetSelectedParentComponents(Type type)
            => CompleteComponentsList.Where(selectionValue => AutomaticSelectionOptions.IsClassOrSubclassOf(type, selectionValue.Key));

        internal MultiplayerRoleFlags GetMultiplayerRoleFlagsForType(Type type)
        {
            if (!CompleteComponentsList.TryGetValue(type, out var target))
                target = MultiplayerRoleFlags.ClientAndServer;

            return target;
        }

        internal MultiplayerRoleFlags GetInheritMultiplayerRoleFlagsForType(System.Type type)
        {
            var parentTypes = CompleteComponentsList.Where(kvp => IsClassOrSubclassOf(type, kvp.Key));
            var target = MultiplayerRoleFlags.ClientAndServer;

            foreach (var item in parentTypes)
                target &= item.Value;

            return target;
        }
    }
}
