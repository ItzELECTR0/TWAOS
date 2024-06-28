using DaimahouGames.Editor.Common;
using DaimahouGames.Runtime.Abilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Abilities
{
    [CustomEditor(typeof(Indicator), true)]
    public class IndicatorInspector : UnityEditor.Editor
    {
        private VisualElement m_Root;
        
        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            serializedObject.CreateChildProperties(m_Root, false);
            return m_Root;
        }
    }
}