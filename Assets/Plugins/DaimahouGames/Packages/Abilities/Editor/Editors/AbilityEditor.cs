using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Abilities
{
    [CustomEditor(typeof(Ability), true)]
    public class AbilityEditor : UnityEditor.Editor
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string ERR_DUPLICATE_ID = "Another item has the same Item ID as this one";
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        
        private VisualElement m_Root;
        private VisualElement m_Head;
        
        private VisualElement m_ContentMsgID;
        private PropertyField m_FieldID;
        
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
            m_Root.Add(new SpaceSmall());
            
            SetupHead();
            
            var targetingProperty = serializedObject.FindProperty("m_Targeting");
            var activatorProperty = serializedObject.FindProperty("m_Activator");

            var targetStrategyInspector = new GenericInspector(targetingProperty);
            var activatorInspector = new GenericInspector(activatorProperty);

            targetStrategyInspector.IsGeneric = true;
            activatorInspector.IsGeneric = true;

            var requirementsProperty = serializedObject.FindProperty("m_Requirements");
            var filtersProperty = serializedObject.FindProperty("m_Filters");
            var effectsProperty = serializedObject.FindProperty("m_Effects");
            
            var requirementsInspector = new GenericListInspector<AbilityRequirement>(requirementsProperty);
            var filtersInspector = new GenericListInspector<AbilityFilter>(filtersProperty);
            var effectsInspector = new GenericListInspector<AbilityEffect>(effectsProperty);

            requirementsInspector.SetTitle("Requirements");
            requirementsInspector.SetAddButtonText("Requirements");
            
            filtersInspector.SetTitle("Filters");
            filtersInspector.SetAddButtonText("Filters");
            
            effectsInspector.SetTitle("Effects");
            effectsInspector.SetAddButtonText("Effects");
            effectsInspector.AllowTypeDuplicate = true;

            m_Root.Add(new SpaceSmall());

            m_Root.Add(targetStrategyInspector);
            m_Root.Add(activatorInspector);
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(requirementsInspector);
            
            m_Root.Add(new SpaceSmaller());
            m_Root.Add(filtersInspector);
            
            m_Root.Add(new SpaceSmaller());
            m_Root.Add(effectsInspector);
            
            return m_Root;
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|

        private void SetupHead()
        {
            var uidProperty = serializedObject.FindProperty("m_UID");
            m_FieldID = new PropertyField(uidProperty);
            
            m_Head = new VisualElement();
            m_ContentMsgID = new VisualElement();
            
            m_Head.Add(m_ContentMsgID);
            m_Head.Add(m_FieldID);
            
            RefreshErrorID();
            m_FieldID.RegisterValueChangeCallback(_ =>
            {
                RefreshErrorID();
            });

            m_Root.Add(m_Head);

            var general = new FoldoutInspector();
            general.SetTile("General");
            general.AddBodyElements(serializedObject, "m_Icon", "m_Range", "m_BetaFeature__ControllableWhileCasting");
            
            
            var rangeProperty = serializedObject.FindProperty("m_Range");
            var iconProperty = serializedObject.FindProperty("m_Icon");
            var rangeInspector = new PropertyField(rangeProperty);
            var iconInspector = new PropertyField(iconProperty);
            
            var box = new Box();
            box.Add(new SpaceSmall());
            box.Add(iconInspector);
            box.Add(rangeInspector);
            box.Add(new SpaceSmall());

            m_Root.Add(m_Head);
            m_Root.Add(new SpaceSmall());
            m_Root.Add(general);
        }
        
        private void RefreshErrorID()
        {
            serializedObject.Update();
            m_ContentMsgID.Clear();

            var id = serializedObject.FindProperty("m_UID");
            
            var itemID = id
                .FindPropertyRelative(UniqueIDDrawer.SERIALIZED_ID)
                .FindPropertyRelative(IdStringDrawer.NAME_STRING)
                .stringValue;

            var hasDuplicatedID = AssetDatabase.FindAssets($"t:{nameof(Ability)}")
                .Where(x => x.GetType() == typeof(Ability))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Ability>)
                .Any(ability => ability.ID.String == itemID && ability != target);

            if (!hasDuplicatedID) return;
            
            var error = new ErrorMessage(ERR_DUPLICATE_ID);
            m_ContentMsgID.Add(error);
        }
        
        //============================================================================================================||
    }
}