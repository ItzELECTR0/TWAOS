using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class CopyRunner<TValue> : TCopyRunner
    {
        private const HideFlags HIDE_FLAGS = HideFlags.HideInHierarchy | HideFlags.DontSave;

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TValue m_Runner;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------
        
        /// <summary>
        /// Creates an invisible game object template that can create deep copies of an object
        /// instance. 
        /// </summary>
        /// <param name="runner"></param>
        /// <returns>Template runner component</returns>
        public static TCopy CreateTemplate<TCopy>(object runner) where TCopy : CopyRunner<TValue>
        {
            GameObject template = new GameObject
            {
                name = "Template",
                hideFlags = HIDE_FLAGS
            };

            TCopy copyRunnerTemplate = template.AddComponent<TCopy>();
            copyRunnerTemplate.m_Runner = (TValue) runner;
            
            return copyRunnerTemplate;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Returns the runner value with the casted value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetRunner<T>() => this.m_Runner is T runner ? runner : default;

        /// <summary>
        /// Creates a new instance of a CopyRunner component template, which contains
        /// a deep-copy of the runner class.
        /// </summary>
        /// <returns>deep-copied instance of the template component</returns>
        public TCopy CreateRunner<TCopy>() where TCopy : CopyRunner<TValue>
        {
            return this.CreateRunner<TCopy>(Vector3.zero, Quaternion.identity, null);
        }
        
        /// <summary>
        /// Creates a new instance of a CopyRunner component template, which contains
        /// a deep-copy of the runner class.
        /// </summary>
        /// <param name="parent">parent of the instantiated copy</param>
        /// <returns>deep-copied instance of the template component</returns>
        public TCopy CreateRunner<TCopy>(Transform parent) where TCopy : CopyRunner<TValue>
        {
            return this.CreateRunner<TCopy>(Vector3.zero, Quaternion.identity, parent);
        }
        
        /// <summary>
        /// Creates a new instance of a CopyRunner component template, which contains
        /// a deep-copy of the runner class.
        /// </summary>
        /// <param name="position">local position from the parent</param>
        /// <param name="rotation">local rotation from the parent</param>
        /// <param name="parent">parent of the instantiated copy</param>
        /// <returns>deep-copied instance of the template component</returns>
        public TCopy CreateRunner<TCopy>(Vector3 position, Quaternion rotation, Transform parent)
            where TCopy : CopyRunner<TValue>
        {
            if (parent != null)
            {
                position = parent.position + position;
                rotation = parent.rotation * rotation;
            }
            
            GameObject copy = Instantiate(this.gameObject, position, rotation, parent);
            copy.hideFlags = HIDE_FLAGS;
            
            return copy.GetComponent<TCopy>();
        }
    }
}