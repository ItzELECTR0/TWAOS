using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using InfoMessage = DaimahouGames.Editor.Core.InfoMessage;

namespace PrototypeCreator.Editor.Core
{
    [CustomEditor(typeof(Pawn), true)]
    public class PawnEditor : UnityEditor.Editor
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        public const string PAWN_FIELD = "m_Pawn";
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        
        private VisualElement m_Root;
        private NonSerializedListInspector m_StateInspector;
        private NonSerializedListInspector m_FeaturesInspector;
        
        private Pawn m_Pawn;

        private InfoMessage m_StateLabel;
        
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            m_Pawn = serializedObject.targetObject as Pawn;
            
            if (EditorApplication.isPlaying)
            {
                DrawStateLabel();
                DrawStateInspector();
            }
            
            DrawFeatureInspector();

            return m_Root;
        }

        public override bool RequiresConstantRepaint()
        {
            if (!EditorApplication.isPlaying) return false;
            
            if (m_Pawn != null && m_Pawn.CurrentState != null)
            {
                m_StateLabel.Text = $"  {m_Pawn.CurrentState.GetType().GetNiceName()}";
            }

            m_StateInspector?.Update();
            m_FeaturesInspector?.Update();

            return true;
        }

        [MenuItem("GameObject/Game Creator/Gameplay/Pawn", false, 0)]
        public static void CreatePawn(MenuCommand menuCommand)
        {
            var instance = new GameObject("Pawn"); 
            instance.AddComponent<Pawn>();

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        
        private void DrawStateLabel()
        {
            m_StateLabel = new InfoMessage("Display Runtime State")
            {
                Icon = new IconStateMachine(ColorTheme.Type.Gray).Texture
            };
            m_Root.Add(m_StateLabel);
        }
        
        private void DrawStateInspector()
        {
            m_StateInspector = new NonSerializedListInspector(GetItems);

            m_StateInspector.SetTitle("States");
            m_Root.Add(m_StateInspector);
            m_Root.Add(new SpaceSmall());
        }

        private IEnumerable<IGenericItem> GetItems()
        {
            var actor = serializedObject.targetObject as Pawn;
            var states = actor.GetFieldValue<Pawn, List<IPawnState>>("m_States");
            return states.Cast<IGenericItem>();
        }
        
        private void DrawFeatureInspector()
        {
            if (EditorApplication.isPlaying)
            {
                DrawRuntimeFeatureInspector();
            }
            else
            { 
                DrawEditorFeatureInspector();
            }
        }

        private void DrawRuntimeFeatureInspector()
        {
            m_FeaturesInspector = new NonSerializedListInspector(GetFeatures);
                
            m_FeaturesInspector.SetTitle("Features");
            m_Root.Add(m_FeaturesInspector);
            m_Root.Add(new SpaceSmall());
        }

        public IEnumerable<IGenericItem> GetFeatures()
        {
            var actor = serializedObject.targetObject as Pawn;
            return actor.GetFieldValue<Pawn, List<Feature>>("m_Features");
        }

        private void DrawEditorFeatureInspector()
        {
            var property = serializedObject.FindProperty("m_Features");
            var inspector = new GenericListInspector<Feature>(property);
            
            inspector.SetTitle("Features");
            inspector.SetAddButtonText("Add new feature");
            
            inspector.EventElementInserted += InitializeFeature;
            inspector.AllowDuplicating = false;
            inspector.AllowCopyPaste = false;
            
            m_Root.Add(inspector);
            
            void InitializeFeature(Feature feature)
            {
                var field = typeof(Feature).GetField(
                    PAWN_FIELD, 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                );
                
                field?.SetValue(feature, serializedObject.targetObject);
                serializedObject.Update();
            }
        }
        
        //============================================================================================================||
    }
}