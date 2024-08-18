using GameCreator.Runtime.Common.UnityUI;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common.UnityUI
{
    [CustomEditor(typeof(EventCallback))]
    public class EventCallbackEditor : UnityEditor.Editor
    {
        private VisualElement m_Root;
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            this.m_Root.Add(new InfoMessage("Handled via script"));
            
            return this.m_Root;
        }
    }
}