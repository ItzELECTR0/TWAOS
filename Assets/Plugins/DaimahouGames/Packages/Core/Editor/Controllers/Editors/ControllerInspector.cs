using DaimahouGames.Editor.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Editor.Common;
using DaimahouGames.Editor.Core;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    [CustomEditor(typeof(Controller), true)]
    public class ControllerInspector : UnityEditor.Editor
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|

        private const string INPUT_MODULES = "m_InputModules";
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        
        private VisualElement m_Root;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            var prop = serializedObject.FindProperty(INPUT_MODULES);
            var inspector = new GenericListInspector<InputModule>(prop);

            serializedObject.CreateChildProperties(m_Root, false, 
                CommonExcludes.Concat(INPUT_MODULES)
            );
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(inspector);
            
            return m_Root;
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}