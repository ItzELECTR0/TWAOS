using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomEditor(typeof(Skeleton))]
    public class SkeletonEditor : UnityEditor.Editor
    {
        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/Skeleton";
        
        private const string REPLACE_HEAD = "Creating a new Skeleton will replace the current one";
        private const string REPLACE_BODY = "Do you want to override the current Skeleton?";

        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Root;
        private VisualElement m_Head;
        private VisualElement m_Body;
        private VisualElement m_Foot;

        private Button m_ButtonSkeletonMode;
        private Button m_ButtonMakeHumanoid;

        private Button m_ButtonCharacter;
        private ObjectField m_CharacterField;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            SkeletonConfigurationStage.EventOpenStage -= this.RefreshSkeletonState;
            SkeletonConfigurationStage.EventOpenStage += this.RefreshSkeletonState;
            
            SkeletonConfigurationStage.EventCloseStage -= this.RefreshSkeletonState;
            SkeletonConfigurationStage.EventCloseStage += this.RefreshSkeletonState;
        }

        private void OnDisable()
        {
            SkeletonConfigurationStage.EventOpenStage -= this.RefreshSkeletonState;
            SkeletonConfigurationStage.EventCloseStage -= this.RefreshSkeletonState;
        }

        [OnOpenAsset]
        public static bool OpenSkeletonExecute(int instanceID, int line)
        {
            Skeleton skeleton = EditorUtility.InstanceIDToObject(instanceID) as Skeleton;
            if (skeleton == null) return false;

            if (SkeletonConfigurationStage.InStage) StageUtility.GoToMainStage();
            Selection.activeObject = skeleton;
            
            string skeletonPath = AssetDatabase.GetAssetPath(skeleton);
            SkeletonConfigurationStage.EnterStage(skeletonPath);
            
            return true;
        }
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            this.m_Foot = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets) this.m_Root.styleSheets.Add(styleSheet);

            this.m_ButtonSkeletonMode = new Button(this.ToggleSkeletonMode)
            {
                style = { height = new Length(30f, LengthUnit.Pixel)}
            };

            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(this.m_ButtonSkeletonMode);
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);
            this.m_Root.Add(this.m_Foot);

            this.m_CharacterField = new ObjectField(string.Empty)
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
                style =
                {
                    marginLeft = 0,
                    marginRight = 0,
                    marginTop = 0,
                    marginBottom = 0,
                }
            };

            this.m_ButtonCharacter = new Button(this.ChangeCharacter)
            {
                text = "Change Character",
                style =
                {
                    height = new Length(22f, LengthUnit.Pixel),
                    marginLeft = 0,
                    marginRight = 0,
                    marginTop = 0,
                    marginBottom = 0,
                }
            };

            PadBox characterBox = new PadBox();
            characterBox.Add(this.m_CharacterField);
            characterBox.Add(new SpaceSmaller());
            characterBox.Add(this.m_ButtonCharacter);
            
            this.m_Head.Add(new SpaceSmall());
            this.m_Head.Add(characterBox);

            SerializedProperty material = this.serializedObject.FindProperty("m_Material");
            SerializedProperty collision = this.serializedObject.FindProperty("m_CollisionDetection");
            SerializedProperty volumes = this.serializedObject.FindProperty("m_Volumes");
            
            PropertyField fieldMaterial = new PropertyField(material);
            PropertyField fieldCollision = new PropertyField(collision);

            this.m_Body.Add(new SpaceSmall());
            this.m_Body.Add(fieldMaterial);
            this.m_Body.Add(fieldCollision);
            
            PropertyField fieldVolumes = new PropertyField(volumes);
            TextSeparator separator = new TextSeparator("or");

            this.m_Foot.Add(new SpaceSmall());
            this.m_Foot.Add(fieldVolumes);
            
            this.m_Foot.Add(new SpaceSmall());
            this.m_Foot.Add(separator);
            this.m_Foot.Add(new SpaceSmall());
            
            this.m_ButtonMakeHumanoid = new Button(this.BuildCharacter)
            {
                text = "Create Humanoid",
                style = { height = new Length(25f, LengthUnit.Pixel)}
            };
            
            this.m_Foot.Add(this.m_ButtonMakeHumanoid);
            
            this.RefreshSkeletonState();
            return this.m_Root;
        }

        // AUTO BUILD CHARACTER: ------------------------------------------------------------------

        private void BuildCharacter()
        {
            Animator animator = SkeletonConfigurationStage.InStage
                ? SkeletonConfigurationStage.Stage.Animator
                : null;

            if (animator == null) return;
            
            Skeleton skeleton = this.serializedObject.targetObject as Skeleton;
            if (skeleton == null) return;
        
            if (!skeleton.IsEmpty)
            {
                bool replace = EditorUtility.DisplayDialog(
                    REPLACE_HEAD,
                    REPLACE_BODY,
                    "Yes", "Cancel"
                );
                
                if (!replace) return;
            }
        
            Volumes volumes = SkeletonBuilder.Make(animator);
            
            this.serializedObject.Update();
            this.serializedObject.FindProperty("m_Volumes").SetValue(volumes);
            
            this.serializedObject.ApplyModifiedProperties();
            this.serializedObject.Update();
            
            this.m_Root.Unbind();
            this.m_Root.Bind(this.serializedObject);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void ChangeCharacter()
        {
            GameObject character = this.m_CharacterField.value as GameObject;
            SkeletonConfigurationStage.ChangeCharacter(character);
        }

        private void ToggleSkeletonMode()
        {
            if (SkeletonConfigurationStage.InStage)
            {
                StageUtility.GoToMainStage();
                this.RefreshSkeletonState();
                return;
            }

            Skeleton skeleton = this.target as Skeleton;
            if (skeleton == null) return;

            string path = AssetDatabase.GetAssetPath(skeleton);
            SkeletonConfigurationStage.EnterStage(path);
        }

        private void RefreshSkeletonState()
        {
            if (this.m_ButtonSkeletonMode == null) return;
            
            bool inSkeletonStage = SkeletonConfigurationStage.InStage;
            this.m_ButtonSkeletonMode.text = inSkeletonStage
                ? "Close Skeleton Mode" 
                : "Enter Skeleton Mode";

            Color borderColor = inSkeletonStage
                ? ColorTheme.Get(ColorTheme.Type.Green)
                : ColorTheme.Get(ColorTheme.Type.Dark);
            
            this.m_ButtonSkeletonMode.style.borderTopColor = borderColor;
            this.m_ButtonSkeletonMode.style.borderBottomColor = borderColor;
            this.m_ButtonSkeletonMode.style.borderLeftColor = borderColor;
            this.m_ButtonSkeletonMode.style.borderRightColor = borderColor;

            this.m_ButtonSkeletonMode.style.color = inSkeletonStage
                ? ColorTheme.Get(ColorTheme.Type.Green)
                : ColorTheme.Get(ColorTheme.Type.TextNormal);

            this.m_Head.SetEnabled(inSkeletonStage);
            this.m_Foot.SetEnabled(inSkeletonStage);
            
            if (inSkeletonStage)
            {
                this.m_CharacterField.value = SkeletonConfigurationStage.CharacterReference;
            }
        }
    }
}
