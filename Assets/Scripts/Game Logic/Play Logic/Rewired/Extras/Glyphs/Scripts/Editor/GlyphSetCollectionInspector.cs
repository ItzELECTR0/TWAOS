// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs.Editor {
    using Rewired.Editor.Libraries.Rotorz.ReorderableList;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [UnityEditor.CustomEditor(typeof(GlyphSetCollection))]
    public sealed class GlyphSetCollectionInspector : UnityEditor.Editor {

        private const string fieldName_sets = "_sets";
        private const string fieldName_collections = "_collections";

        new private SpriteGlyphSet target { get { return (SpriteGlyphSet)base.target; } }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            ReorderableListGUI.Title("Sets");
            ReorderableListGUI.ListField(serializedObject.FindProperty(fieldName_sets));

            ReorderableListGUI.Title("Collections");
            ReorderableListGUI.ListField(serializedObject.FindProperty(fieldName_collections));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
