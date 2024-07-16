// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A glyph set collection represents a list of glyph sets. 
    /// This can be used to organize glyph sets into categories to work with them more easily. 
    /// Glyph Set Collections can be chained, allowing you to organize glyph sets under one top-level collection, 
    /// making it easy to swap out the entire set of glyphs for a different set. 
    /// Examples of this might include having different glyph styles for different consoles / platforms or for localization purposes.
    /// </summary>
    [Serializable]
    public class GlyphSetCollection : UnityEngine.ScriptableObject {

        [UnityEngine.Tooltip("The list of glyph sets.")]
        [UnityEngine.SerializeField]
        private List<GlyphSet> _sets;
        [UnityEngine.Tooltip("The list of glyph set collections.")]
        [UnityEngine.SerializeField]
        private List<GlyphSetCollection> _collections;

        /// <summary>
        /// The list of glyph sets.
        /// </summary>
        public List<GlyphSet> sets {
            get {
                return _sets;
            }
            set {
                _sets = value;
            }
        }

        /// <summary>
        /// The list of glyph set collections.
        /// </summary>
        public List<GlyphSetCollection> collections {
            get {
                return _collections;
            }
            set {
                if (value != null && value.Contains(this)) {
                    LogCircularDependency();
                    UnityEngine.Debug.LogWarning("Rewired: Set collections aborted due to circular dependency.");
                    return;
                }
                _collections = value;
            }
        }

        /// <summary>
        /// Iterates all glyph sets and glyph set collections recursively.
        /// </summary>
        /// <returns>IEnumerable</returns>
        public virtual IEnumerable<GlyphSet> IterateSetsRecursively() {
            return IterateSetsRecursively(new List<GlyphSetCollection>() { this });
        }
        /// <summary>
        /// Iterates all glyph sets and glyph set collections recursively.
        /// </summary>
        /// <param name="processedCollections">A list of collections that have been processed.</param>
        /// <returns>IEnumerable</returns>
        protected virtual IEnumerable<GlyphSet> IterateSetsRecursively(List<GlyphSetCollection> processedCollections) {
            if (processedCollections == null) throw new ArgumentNullException("processedCollections");
            int setCount;
            int collectionCount;
            if (_sets != null) {
                setCount = _sets.Count;
                for (int i = 0; i < setCount; i++) {
                    if (_sets[i] == null) continue;
                    yield return sets[i];
                }
            }
            if (_collections != null) {
                collectionCount = _collections.Count;
                for (int i = 0; i < collectionCount; i++) {
                    if (_collections[i] == null) continue;
                    if (processedCollections.Contains(_collections[i])) {
                        LogCircularDependency();
                        continue;
                    }
                    processedCollections.Add(_collections[i]);
                    foreach (var set in _collections[i].IterateSetsRecursively(processedCollections)) {
                        yield return set;
                    }
                }
            }
        }

        private static void LogCircularDependency() {
            UnityEngine.Debug.LogError("Rewired: Circular dependency detected. This collection is referenced in a child collection. This is not allowed.");
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            if (_collections != null && _collections.Contains(this)) {
                LogCircularDependency();
                _collections.Remove(this);
                UnityEngine.Debug.LogWarning("Rewired: Removed circular dependency.");
            }
        }
#endif
    }
}
