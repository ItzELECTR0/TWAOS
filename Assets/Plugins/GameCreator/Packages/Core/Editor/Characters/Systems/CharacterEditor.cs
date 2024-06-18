using GameCreator.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Editor.Characters
{
    [CustomEditor(typeof(Character), true)]
    public class CharacterEditor : UnityEditor.Editor
    {
        private const string NAME_GROUP_GENERAL   = "GC-Character-GroupGeneral";
        private const string NAME_GROUP_GENERAL_L = "GC-Character-GroupGeneral-L";
        private const string NAME_GROUP_GENERAL_R = "GC-Character-GroupGeneral-R";

        private const string USS_CHARACTER = EditorPaths.CHARACTERS + "StyleSheets/Character";
        private const string USS_KERNEL = EditorPaths.CHARACTERS + "StyleSheets/Kernel";
        
        public const string MODEL_PATH = RuntimePaths.CHARACTERS + "Assets/3D/Mannequin.fbx";
        private const string FOOTSTEPS_PATH = RuntimePaths.CHARACTERS + "Assets/3D/Footsteps.asset";
        private const string RTC_PATH = RuntimePaths.CHARACTERS + "Assets/Controllers/CompleteLocomotion.controller";

        private const string PLAYER = "m_Player";
        private const string MOTION = "m_Motion";
        private const string DRIVER = "m_Driver";
        private const string FACING = "m_Facing";
        private const string ANIMIM = "m_Animim";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_KernelContent;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Character character = this.serializedObject.targetObject as Character;
                if (character != null)
                {
                    character.Kernel.EventChangePlayer += this.DrawKernel;
                    character.Kernel.EventChangeMotion += this.DrawKernel;
                    character.Kernel.EventChangeFacing += this.DrawKernel;
                    character.Kernel.EventChangeDriver += this.DrawKernel;
                    character.Kernel.EventChangeAnimim += this.DrawKernel;
                }
            }
        }

        private void OnDisable()
        {
            Character character = this.serializedObject.targetObject as Character;
            if (character != null)
            {
                character.Kernel.EventChangePlayer -= this.DrawKernel;
                character.Kernel.EventChangeMotion -= this.DrawKernel;
                character.Kernel.EventChangeFacing -= this.DrawKernel;
                character.Kernel.EventChangeDriver -= this.DrawKernel;
                character.Kernel.EventChangeAnimim -= this.DrawKernel;
            }
        }

        // INSPECTOR: -----------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_CHARACTER, USS_KERNEL);
            foreach (StyleSheet styleSheet in styleSheets) root.styleSheets.Add(styleSheet);
            
            SerializedProperty propertyIsPlayer = this.serializedObject.FindProperty("m_IsPlayer");
            SerializedProperty propertyTime = this.serializedObject.FindProperty("m_Time");
            SerializedProperty propertyBusy = this.serializedObject.FindProperty("m_Busy");

            VisualElement groupGeneral  = new VisualElement { name = NAME_GROUP_GENERAL   };
            VisualElement groupGeneralL = new VisualElement { name = NAME_GROUP_GENERAL_L };
            VisualElement groupGeneralR = new VisualElement { name = NAME_GROUP_GENERAL_R };
            
            root.Add(groupGeneral);
            groupGeneral.Add(groupGeneralL);
            groupGeneral.Add(groupGeneralR);

            PropertyField fieldIsPlayer = new PropertyField(propertyIsPlayer);
            PropertyField fieldTime = new PropertyField(propertyTime);
            
            fieldIsPlayer.SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
            fieldTime.SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
            
            groupGeneralL.Add(new PropertyField(propertyBusy));
            groupGeneralR.Add(fieldIsPlayer);
            groupGeneralR.Add(fieldTime);
            
            this.m_KernelContent = new VisualElement();
            this.m_KernelContent.AddToClassList("gc-character-kernel-root");
            
            root.Add(this.m_KernelContent);
            this.DrawKernel();
            
            SerializedProperty propertyIK = this.serializedObject.FindProperty("m_InverseKinematics");
            SerializedProperty propertyFootsteps = this.serializedObject.FindProperty("m_Footsteps");
            SerializedProperty propertyRagdoll = this.serializedObject.FindProperty("m_Ragdoll");
            SerializedProperty propertyUniqueID = this.serializedObject.FindProperty("m_UniqueID");
            
            PropertyField fieldIK = new PropertyField(propertyIK);
            PropertyField fieldFootsteps = new PropertyField(propertyFootsteps);
            PropertyField fieldRagdoll = new PropertyField(propertyRagdoll);
            PropertyField fieldUniqueID = new PropertyField(propertyUniqueID);
            ToolCombat toolCombat = new ToolCombat(this.target as Character);
            
            root.Add(fieldIK);
            root.Add(fieldFootsteps);
            root.Add(fieldRagdoll);
            root.Add(toolCombat);
            root.Add(new SpaceSmallest());
            root.Add(fieldUniqueID);

            return root;
        }

        private void DrawKernel()
        {
            this.m_KernelContent.Clear();
            
            SerializedProperty kernel = this.serializedObject.FindProperty("m_Kernel");
            if (kernel == null) return;
            
            this.serializedObject.ApplyModifiedProperties();
            this.serializedObject.Update();

            SerializedProperty propertyPlayer = kernel.FindPropertyRelative(PLAYER);
            SerializedProperty propertyMotion = kernel.FindPropertyRelative(MOTION);
            SerializedProperty propertyDriver = kernel.FindPropertyRelative(DRIVER);
            SerializedProperty propertyFacing = kernel.FindPropertyRelative(FACING);
            SerializedProperty propertyAnimim = kernel.FindPropertyRelative(ANIMIM);

            PropertyField fieldPlayer = new PropertyField(propertyPlayer);
            PropertyField fieldMotion = new PropertyField(propertyMotion);
            PropertyField fieldDriver = new PropertyField(propertyDriver);
            PropertyField fieldFacing = new PropertyField(propertyFacing);
            PropertyField fieldAnimim = new PropertyField(propertyAnimim);

            this.m_KernelContent.Add(fieldPlayer);
            this.m_KernelContent.Add(fieldMotion);
            this.m_KernelContent.Add(fieldDriver);
            this.m_KernelContent.Add(fieldFacing);
            this.m_KernelContent.Add(fieldAnimim);

            fieldPlayer.Bind(kernel.serializedObject);
            fieldMotion.Bind(kernel.serializedObject);
            fieldDriver.Bind(kernel.serializedObject);
            fieldFacing.Bind(kernel.serializedObject);
            fieldAnimim.Bind(kernel.serializedObject);

            VisualElement errors = new VisualElement();
            this.m_KernelContent.Add(errors);
            
            this.CheckForceUnits(errors, kernel);

            fieldPlayer.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, kernel));
            fieldMotion.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, kernel));
            fieldDriver.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, kernel));
            fieldFacing.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, kernel));
            fieldAnimim.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, kernel));
        }

        // CREATION MENU: -------------------------------------------------------------------------

        private static void MakeCharacter(MenuCommand menuCommand, bool isPlayer)
        {
            GameObject instance = new GameObject(isPlayer ? "Player" : "Character");
            Character character = instance.AddComponent<Character>();

            float height = character.Motion.Height;
            character.transform.position += Vector3.up * (height * 0.5f);
            
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MODEL_PATH);
            MaterialSoundsAsset footsteps = AssetDatabase.LoadAssetAtPath<MaterialSoundsAsset>(FOOTSTEPS_PATH);
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(RTC_PATH);
            
            character.ChangeModel(prefab, new Character.ChangeOptions 
            {
                controller = controller,
                materials = footsteps,
                offset = Vector3.zero
            });
            
            character.IsPlayer = isPlayer;

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
        
        [MenuItem("GameObject/Game Creator/Characters/Character", false, 0)]
        public static void CreateCharacter(MenuCommand menuCommand)
        {
            MakeCharacter(menuCommand, false);
        }
        
        [MenuItem("GameObject/Game Creator/Characters/Player", false, 0)]
        public static void CreatePlayer(MenuCommand menuCommand)
        {
            MakeCharacter(menuCommand, true);
        }
        
        // PRIVATE KERNEL METHODS: ----------------------------------------------------------------
        
        private void CheckForceUnits(VisualElement content, SerializedProperty propertyKernel)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            
            content.Clear();

            ICharacterKernel kernel = propertyKernel.GetValue<ICharacterKernel>();
            if (kernel == null)
            {
                content.Add(new ErrorMessage("Kernel cannot be null"));
                return;
            }
            
            if (kernel.Player == null)
            {
                content.Add(new ErrorMessage("Player unit cannot be null"));
                return;
            }
            
            if (kernel.Motion == null)
            {
                content.Add(new ErrorMessage("Motion unit cannot be null"));
                return;
            }
            
            if (kernel.Driver == null)
            {
                content.Add(new ErrorMessage("Driver unit cannot be null"));
                return;
            }
            
            if (kernel.Facing == null)
            {
                content.Add(new ErrorMessage("Rotation unit cannot be null"));
                return;
            }
            
            if (kernel.Animim == null)
            {
                content.Add(new ErrorMessage("Animation unit cannot be null"));
                return;
            }
            
            IUnitCommon player = propertyKernel.FindPropertyRelative(PLAYER).GetValue<IUnitCommon>();
            IUnitCommon motion = propertyKernel.FindPropertyRelative(MOTION).GetValue<IUnitCommon>();
            IUnitCommon driver = propertyKernel.FindPropertyRelative(DRIVER).GetValue<IUnitCommon>();
            IUnitCommon facing = propertyKernel.FindPropertyRelative(FACING).GetValue<IUnitCommon>();
            IUnitCommon animim = propertyKernel.FindPropertyRelative(ANIMIM).GetValue<IUnitCommon>();
            
            this.CheckForceUnit(content, player, kernel);
            this.CheckForceUnit(content, motion, kernel);
            this.CheckForceUnit(content, driver, kernel);
            this.CheckForceUnit(content, facing, kernel);
            this.CheckForceUnit(content, animim, kernel);
        }

        private void CheckForceUnit(VisualElement body, IUnitCommon unit, ICharacterKernel kernel)
        {
            if (unit.ForcePlayer != null && unit.ForcePlayer != kernel.Player.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForcePlayer);
                body.Add(new ErrorMessage($"{unitName} requires Player of type {forceName}"));
            }
            
            if (unit.ForceMotion != null && unit.ForceMotion != kernel.Motion.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceMotion);
                body.Add(new ErrorMessage($"{unitName} requires Motion of type {forceName}"));
            }
            
            if (unit.ForceDriver != null && unit.ForceDriver != kernel.Driver.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceDriver);
                body.Add(new ErrorMessage($"{unitName} requires Driver of type {forceName}"));
            }
            
            if (unit.ForceFacing != null && unit.ForceFacing != kernel.Facing.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceFacing);
                body.Add(new ErrorMessage($"{unitName} requires Rotation of type {forceName}"));
            }
            
            if (unit.ForceAnimim != null && unit.ForceAnimim != kernel.Animim.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceAnimim);
                body.Add(new ErrorMessage($"{unitName} requires Animation of type {forceName}"));
            }
        }
    }
}