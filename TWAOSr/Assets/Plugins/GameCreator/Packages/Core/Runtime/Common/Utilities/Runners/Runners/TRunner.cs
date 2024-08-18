using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TRunner<TValue> : Runner
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected TValue m_Value;

        // PROPERTIES: ----------------------------------------------------------------------------

        public TValue Value => this.m_Value;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static GameObject CreateTemplate<TRunnerType>(TValue value) 
            where TRunnerType : TRunner<TValue>
        {
            HideFlags containerHideFlags = HideFlags.HideAndDontSave;
            
            #if UNITY_EDITOR

            if (UnityEditor.EditorPrefs.GetBool(KEY_RUNNER_SHOW_CONTAINER, false))
            {
                containerHideFlags = HideFlags.None;
            }
            
            #endif
            
            GameObject container = new GameObject
            {
                hideFlags = containerHideFlags
            };
            
            GameObject template = new GameObject
            {
                hideFlags = TEMPLATE_FLAGS
            };

            container.name = container.GetInstanceID().ToString();
            template.transform.SetParent(container.transform);
            
            TRunnerType runner = template.Add<TRunnerType>();
            runner.m_Value = value;
            
            return template;
        }

        public static TRunnerType Pick<TRunnerType>(GameObject template, RunnerConfig config, int prewarmCounter)
            where TRunnerType : TRunner<TValue>
        {
            if (template == null) return null;
            
            Vector3 position = config.Location.Position;
            Quaternion rotation = config.Location.Rotation;
            
            if (config.Location.Parent != null)
            {
                position = config.Location.Parent.TransformPoint(position);
                rotation = config.Location.Parent.rotation * rotation;
            }

            int templateHash = template.GetInstanceID();
            if (!Pool.ContainsKey(templateHash))
            {
                template.transform.parent.gameObject.name = config.Name;
                template.name = $"{config.Name}";
                
                RunnerPool runnerPool = new RunnerPool(template, prewarmCounter);
                Pool.Add(templateHash, runnerPool);
            }

            TRunnerType instance = Pool[templateHash].Pick<TRunnerType>();
            if (instance == null) return null;
            
            instance.transform.SetPositionAndRotation(position, rotation);
            if (config.Location.Parent != null)
            {
                instance.transform.SetParent(config.Location.Parent);
            }
            
            instance.gameObject.SetActive(true);
            return instance.Get<TRunnerType>();
        }

        public static void Restore(Runner runner)
        {
            if (runner == null) return;
            if (runner.Template == null) Destroy(runner.gameObject);
            
            if (Pool.TryGetValue(runner.Template.GetInstanceID(), out RunnerPool runnerPool))
            {
                runnerPool.Restore(runner.gameObject);
            }
        }
    }
}