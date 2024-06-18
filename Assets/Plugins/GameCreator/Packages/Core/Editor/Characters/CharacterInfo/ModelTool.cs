using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class ModelTool : VisualElement
    {
        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/Model";

        private const string PATH_DEFAULT_RTC = RuntimePaths.CHARACTERS + 
                                                "Assets/Controllers/CompleteLocomotion.controller";

        private const string NAME_ELEMENT = "GC-Character-Model";
        private const string NAME_DROPZONE = "GC-Character-Model-Dropzone";

        private const string HUMANOID_TITLE = "The model is not a Humanoid";
        private const string HUMANOID_MESSAGE = "Do you want to change the model type to Humanoid?";
        
        private const string PREFAB_TITLE = "This Character is a prefab instance";
        private const string PREFAB_MESSAGE = "Do you want to unpack it before adding the new Model?";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SerializedProperty m_Property;
        
        // FIELDS: --------------------------------------------------------------------------------

        public readonly Character character;
        public readonly VisualElement dropzone;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ModelTool(SerializedProperty property)
        {
            this.m_Property = property;
            this.character = this.m_Property.serializedObject.targetObject as Character;

            this.name = NAME_ELEMENT;
            
            this.dropzone = new VisualElement
            {
                name = NAME_DROPZONE
            };
            
            DropModelManipulator manipulator = new DropModelManipulator(this);
            
            this.dropzone.AddManipulator(manipulator);
            
            Label dropZone = new Label("Drop a 3D model");
            
            this.dropzone.Add(dropZone);
            this.Add(this.dropzone);

            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.styleSheets.Add(styleSheet);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ChangeModelEditor(GameObject prefab, Vector3 offset)
        {
            this.m_Property.serializedObject.Update();
            CheckIsPrefab(this.m_Property.serializedObject.targetObject);

            Transform hull = this.character.Animim.Mannequin;
            
            if (this.character.Animim.Animator != null)
            {
                Object.DestroyImmediate(this.character.Animim.Animator.gameObject);
            }
            
            if (hull == null)
            {
                this.character.Animim.Mannequin = new GameObject("Mannequin").transform;
                this.character.Animim.Mannequin.transform.SetParent(this.character.transform);
            }
            
            Vector3 position = Vector3.down * this.character.Motion.Height * 0.5f;
            
            this.character.Animim.Mannequin.transform.localPosition = position + offset;
            this.character.Animim.Mannequin.transform.localRotation = Quaternion.identity;

            Avatar avatar = SetupAvatarType(prefab);
            GameObject model = Object.Instantiate(prefab, this.character.Animim.Mannequin);
            
            model.name = prefab.name;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            this.m_Property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            this.m_Property.serializedObject.Update();

            Animator animator = model.GetComponent<Animator>();
            
            if (animator == null) animator = model.AddComponent<Animator>();
            if (animator.avatar == null) animator.avatar = avatar;
            
            if (animator != null && animator.runtimeAnimatorController == null)
            {
                RuntimeAnimatorController rtc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PATH_DEFAULT_RTC);
                animator.runtimeAnimatorController = rtc;
            }

            SerializedProperty propertyAnimator = this.m_Property.FindPropertyRelative("m_Animator");
            propertyAnimator.objectReferenceValue = animator;

            SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
        }

        private static void CheckIsPrefab(Object target)
        {
            if (!PrefabUtility.IsPartOfPrefabInstance(target)) return;
            if (target is not Character character || character == null) return;
            
            bool unpackPrefab = EditorUtility.DisplayDialog(
                PREFAB_TITLE,
                PREFAB_MESSAGE,
                "Yes, unpack it", "No"
            );

            if (unpackPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(
                    character.gameObject,
                    PrefabUnpackMode.Completely,
                    UnityEditor.InteractionMode.AutomatedAction
                );   
            }
        }

        private static Avatar SetupAvatarType(GameObject prefab)
        {
            if (prefab.TryGetComponent(out Animator animator))
            {
                return animator.avatar;
            }

            string path = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(path)) return null;

            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) return null;

            if (importer.animationType == ModelImporterAnimationType.Human)
            {
                return LookForAvatar(path, importer);
            }
            
            bool changeToHumanoid = EditorUtility.DisplayDialog(
                HUMANOID_TITLE,
                HUMANOID_MESSAGE,
                "Yes, change to Humanoid", "No"
            );

            if (!changeToHumanoid)
            {
                return LookForAvatar(path, importer);
            }
            
            importer.animationType = ModelImporterAnimationType.Human;
            importer.SaveAndReimport();
            
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.autoGenerateAvatarMappingIfUnspecified = true;
            importer.SaveAndReimport();

            return LookForAvatar(path, importer);
        }

        private static Avatar LookForAvatar(string path, ModelImporter importer)
        {
            return importer.sourceAvatar != null
                ? importer.sourceAvatar
                : AssetDatabase.LoadAssetAtPath<Avatar>(path);
        }
    }
}