using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TPolymorphicItemTool : VisualElement, IPolymorphicItemTool
    {
        private const string TIP_DUPLICATE = "Duplicate";
        private const string TIP_DELETE = "Delete";

        public const string CLASS_ROOT = "gc-list-item-root";
        public const string CLASS_HEAD = "gc-list-item-head";
        public const string CLASS_BODY = "gc-list-item-body";
        
        public const string CLASS_DROP_ABOVE = "gc-list-item-drop-above";
        public const string CLASS_DROP_BELOW = "gc-list-item-drop-below";
        
        public const string CLASS_HEAD_DRAG = "gc-list-item-head-drag";
        public const string CLASS_HEAD_BREAKPOINT = "gc-list-item-head-breakpoint";
        public const string CLASS_HEAD_DISABLE = "gc-list-item-head-disabled";
        public const string CLASS_HEAD_BUTTON = "gc-list-item-head-button";
        public const string CLASS_HEAD_INFO = "gc-list-item-head-info";
        public const string CLASS_HEAD_INFO_EXPANDED = "gc-list-item-head-info--expanded";

        private const string USS_PATH = EditorPaths.COMMON + "Polymorphism/Lists/StyleSheets/";
        public const string USS_HEAD_PATH = USS_PATH + "Polymorphic-Item-Head";
        public const string USS_BODY_PATH = USS_PATH + "Polymorphic-Item-Body";
        public const string USS_DROP_PATH = USS_PATH + "Polymorphic-Item-Drop";
        
        private static readonly IIcon ICON_DRAG = new IconDrag(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_BREAKPOINT = new IconBreakpoint(ColorTheme.Type.Red);
        private static readonly IIcon ICON_DISABLED = new IconCancel(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_REMOVE = new IconMinus(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_DUPLICATE = new IconDuplicate(ColorTheme.Type.TextNormal);

        // MEMBERS: -------------------------------------------------------------------------------
        
        protected readonly SerializedProperty m_Property;

        protected readonly VisualElement m_Head = new VisualElement();
        protected readonly VisualElement m_Body = new VisualElement();
        
        protected Label m_HeadDrag;
        protected Button m_HeadBreakpoint;
        protected Button m_HeadDisabled;
        protected Button m_HeadDelete;
        protected Button m_HeadButton;
        protected Button m_HeadDuplicate;
        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        
        private readonly VisualElement m_DropAbove;
        private readonly VisualElement m_DropBelow;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public IPolymorphicListTool ParentTool { get; }
        public int Index { get; }

        protected abstract object Value { get; }

        private Type ItemType => TypeUtils.GetTypeFromProperty(this.m_Property, false);

        private const string KEY_EXPANDED = "gc:polymorphic-item:is-expanded:{0}:{1}";
        
        public bool IsExpanded
        {
            get => SessionState.GetBool(KeyIsExpanded(this.m_Property), false);
            set => SessionState.SetBool(KeyIsExpanded(this.m_Property), value);
        }
        
        public bool IsEnabled
        {
            get
            {
                this.m_Property.serializedObject.Update();
                return this.m_Property?.FindPropertyRelative("m_IsEnabled")?.boolValue ?? false;
            }
            set
            {
                if (m_Property?.FindPropertyRelative("m_IsEnabled") == null) return;
                this.m_Property.FindPropertyRelative("m_IsEnabled").boolValue = value;
                SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
            }
        }
        
        public bool Breakpoint
        {
            get
            {
                this.m_Property.serializedObject.Update();
                return this.m_Property?.FindPropertyRelative("m_Breakpoint")?.boolValue ?? false;
            }
            set
            {
                if (m_Property?.FindPropertyRelative("m_Breakpoint") == null) return;
                this.m_Property.FindPropertyRelative("m_Breakpoint").boolValue = value;
                SerializationUtils.ApplyUnregisteredSerialization(this.m_Property.serializedObject);
            }
        }

        public virtual string Title
        {
            get
            {
                this.m_Property?.serializedObject.Update();
                return this.m_Property?.GetValue<IPolymorphicItem>()?.Title;
            }
        }

        protected virtual Color Color
        {
            get
            {
                this.m_Property?.serializedObject.Update();
                return this.m_Property?.GetValue<IPolymorphicItem>()?.Color 
                       ?? ColorTheme.Get(ColorTheme.Type.TextNormal);
            }
        }

        protected virtual List<string> CustomStyleSheetPaths => new List<string>();
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TPolymorphicItemTool(IPolymorphicListTool parentTool, int propertyIndex)
        {
            this.AddToClassList(CLASS_ROOT);
            
            this.m_Head.AddToClassList(CLASS_HEAD);
            this.m_Body.AddToClassList(CLASS_BODY);

            this.m_DropAbove = new VisualElement();
            this.m_DropBelow = new VisualElement();

            this.ParentTool = parentTool;
            this.Index = propertyIndex;
            
            this.m_Property = this.ParentTool.PropertyList.GetArrayElementAtIndex(propertyIndex);

            List<string> styleSheetsPaths = new List<string>
            {
                USS_HEAD_PATH,
                USS_BODY_PATH,
                USS_DROP_PATH
            };
            
            styleSheetsPaths.AddRange(this.CustomStyleSheetPaths);

            StyleSheet[] sheets = StyleSheetUtils.Load(styleSheetsPaths.ToArray());
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);

            this.Add(this.m_Head);
            this.Add(this.m_Body);

            this.SetupHead();
            this.SetupBody();
            this.SetupDrop();

            EditorApplication.playModeStateChanged -= this.OnChangePlayMode;
            EditorApplication.playModeStateChanged += this.OnChangePlayMode;

            this.OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.EnteredEditMode
            );
        }

        ~TPolymorphicItemTool()
        {
            this.OnDisable();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= this.OnChangePlayMode;
        }
        
        // SETUP METHODS: -------------------------------------------------------------------------

        protected virtual void SetupHead()
        {
            if (this.ParentTool.AllowReordering)
            {
                this.SetupHeadReordering();
            }

            this.m_HeadImage = new Image();
            this.m_HeadLabel = new Label("Undefined");

            this.m_HeadButton = new Button(() =>
            {
                this.IsExpanded = !this.IsExpanded;
                
                this.UpdateHead();
                this.UpdateBody(true);
            });

            if (this.ParentTool.AllowContextMenu)
            {
                ContextualMenuManipulator man = new ContextualMenuManipulator(this.OnOpenHeadMenu);
                this.m_HeadButton.AddManipulator(man);
            }
            
            this.m_HeadButton.AddToClassList(CLASS_HEAD_INFO);

            this.m_HeadButton.Add(this.m_HeadImage);
            this.m_HeadButton.Add(this.m_HeadLabel);
            this.m_Head.Add(this.m_HeadButton);

            if (this.ParentTool.AllowBreakpoint)
            {
                this.SetupHeadBreakpoint();
            }

            if (this.ParentTool.AllowDisable)
            {
                this.SetupHeadDisable();
            }

            this.SetupHeadExtras();

            if (this.ParentTool.AllowDuplicating)
            {
                this.SetupHeadDuplicating();
            }
            
            if (this.ParentTool.AllowDeleting)
            {
                this.SetupHeadDeleting();
            }
            
            this.UpdateHead();
        }

        private void SetupHeadReordering()
        {
            this.m_HeadDrag = new Label();
            this.m_HeadDrag.AddManipulator(this.ParentTool.ManipulatorSort);
            this.m_HeadDrag.AddManipulator(new ContextualMenuManipulator(this.OnOpenHeadMenu));
            this.m_HeadDrag.AddToClassList(CLASS_HEAD_DRAG);

            this.m_HeadDrag.Add(new Image
            {
                image = ICON_DRAG.Texture,
                focusable = false
            });
            
            this.m_Head.Add(this.m_HeadDrag);
        }

        private void SetupHeadBreakpoint()
        {
            this.m_HeadBreakpoint = new Button(() =>
            {
                this.Breakpoint = false;
                this.UpdateHead();
            });
            
            this.m_HeadBreakpoint.Add(new Image
            {
                image = ICON_BREAKPOINT.Texture,
                focusable = false
            });

            this.m_HeadBreakpoint.tooltip = "Pauses the Editor before reaching this element";
            this.m_HeadBreakpoint.AddToClassList(CLASS_HEAD_BREAKPOINT);
            this.m_Head.Add(this.m_HeadBreakpoint);
        }
        
        private void SetupHeadDisable()
        {
            this.m_HeadDisabled = new Button(() =>
            {
                this.IsEnabled = true;
                this.UpdateHead();
            });
            
            this.m_HeadDisabled.Add(new Image
            {
                image = ICON_DISABLED.Texture,
                focusable = false
            });
            
            this.m_HeadDisabled.tooltip = "A disabled element is ignored";
            this.m_HeadDisabled.AddToClassList(CLASS_HEAD_DISABLE);
            this.m_Head.Add(this.m_HeadDisabled);
        }

        protected virtual void SetupHeadExtras()
        { }
        
        private void SetupHeadDeleting()
        {
            this.m_HeadDelete = new Button(() =>
            {
                this.ParentTool.DeleteItem(this.Index);
            });

            this.m_HeadDelete.Add(new Image
            {
                image = ICON_REMOVE.Texture
            });
            
            this.m_HeadDelete.AddToClassList(CLASS_HEAD_BUTTON);
            this.m_HeadDelete.tooltip = TIP_DELETE;
            
            this.m_Head.Add(this.m_HeadDelete);
        }

        private void SetupHeadDuplicating()
        {
            this.m_HeadDuplicate = new Button(() =>
            {
                this.ParentTool.DuplicateItem(this.Index);
            });
            
            this.m_HeadDuplicate.Add(new Image
            {
                image = ICON_DUPLICATE.Texture
            });
            
            this.m_HeadDuplicate.AddToClassList(CLASS_HEAD_BUTTON);
            this.m_HeadDuplicate.tooltip = TIP_DUPLICATE;
            this.m_Head.Add(this.m_HeadDuplicate);
        }
        
        protected virtual void SetupBody()
        {
            this.m_Property.serializedObject.Update();
            
            PropertyField fieldBody = new PropertyField(this.m_Property, string.Empty);
            fieldBody.Bind(this.m_Property.serializedObject);
            
            fieldBody.RegisterValueChangeCallback(_ => this.UpdateHead());

            this.m_Body.Add(fieldBody);
            this.UpdateBody(false);
        }
        
        private void SetupDrop()
        {
            this.m_DropAbove.AddToClassList(CLASS_DROP_ABOVE);
            this.m_DropBelow.AddToClassList(CLASS_DROP_BELOW);

            this.Insert(0, this.m_DropAbove);
            this.Insert(3, this.m_DropBelow);
        }
        
        protected virtual Texture2D GetIcon()
        {
            this.m_Property.serializedObject.Update();
            object instance = this.m_Property.GetValue<IPolymorphicItem>();
                
            IEnumerable<ImageAttribute> iconAttrs = instance?.GetType()
                .GetCustomAttributes<ImageAttribute>();
            Texture2D icon = iconAttrs?.FirstOrDefault()?.Image;

            return icon != null ? icon : Texture2D.whiteTexture;
        }
        
        // CONTEXT MENU: --------------------------------------------------------------------------

        private void OnOpenHeadMenu(ContextualMenuPopulateEvent menu)
        {
            if (this.ParentTool.AllowCopyPaste)
            {
                menu.menu.AppendAction(
                    "Copy", 
                    action =>
                    {
                        CopyPasteUtils.SoftCopy(this.m_Property.GetManagedValue(), this.ItemType);
                    }
                );
                
                menu.menu.AppendAction(
                    "Paste", 
                    action =>
                    {
                        int pasteIndex = this.Index + 1;
                        this.ParentTool.InsertItem(pasteIndex, CopyPasteUtils.SourceObjectCopy);
                    },
                    action => CopyPasteUtils.CanSoftPaste(this.ItemType)
                        ? DropdownMenuAction.Status.Normal
                        : DropdownMenuAction.Status.Disabled
                );

                menu.menu.AppendSeparator();
            }
            
            if (this.ParentTool.AllowInsertion)
            {
                menu.menu.AppendAction(
                    "Replace...", 
                    action =>
                    {
                        int insertIndex = this.Index;
                        TypeSelectorFancyPopup.Open(
                            this.m_Head, this.ItemType, 
                            newType => this.OnReplace(insertIndex, newType)
                        );
                    }
                );
                
                menu.menu.AppendAction(
                    "Insert Above...", 
                    action =>
                    {
                        int insertIndex = this.Index;
                        TypeSelectorFancyPopup.Open(
                            this.m_Head, this.ItemType, 
                            newType => this.OnInsert(insertIndex, newType)
                        );
                    }
                );
            
                menu.menu.AppendAction(
                    "Insert Below...", 
                    action =>
                    {
                        int insertIndex = this.Index + 1;
                        TypeSelectorFancyPopup.Open(
                            this.m_Head, this.ItemType, 
                            newType => this.OnInsert(insertIndex, newType)
                        );
                    }
                );

                menu.menu.AppendSeparator();
            }

            if (this.ParentTool.AllowBreakpoint)
            {
                menu.menu.AppendAction(
                    "Breakpoint", 
                    action =>
                    {
                        this.Breakpoint = !this.Breakpoint;
                        this.UpdateHead();
                    },
                    action => this.Breakpoint 
                        ? DropdownMenuAction.Status.Checked 
                        : DropdownMenuAction.Status.Normal
                );   
            }

            if (this.ParentTool.AllowDisable)
            {
                menu.menu.AppendAction(
                    this.IsEnabled ? "Disable" : "Enable", 
                    action =>
                    {
                        this.IsEnabled = !this.IsEnabled;
                        this.UpdateHead();
                    }
                );
            }

            if (this.ParentTool.AllowGroupCollapse)
            {
                menu.menu.AppendAction(
                    "Collapse", 
                    action => this.ParentTool.Collapse()
                );
            }
            
            if (this.ParentTool.AllowGroupExpand)
            {
                menu.menu.AppendAction(
                    "Expand", 
                    action => this.ParentTool.Expand()
                );
            }

            if (this.ParentTool.AllowDocumentation)
            {
                menu.menu.AppendAction(
                    "Help", 
                    action => DocumentationPopup.Open(this.Value?.GetType())
                );
            }
        }
        
        private void OnReplace(int index, Type newType)
        {
            this.ParentTool.DeleteItem(index);
            this.OnInsert(index, newType);
        }
        
        private void OnInsert(int index, Type newType)
        {
            object instance = Activator.CreateInstance(newType);
            this.ParentTool.InsertItem(index, instance);
        }

        // REFRESH METHODS: -----------------------------------------------------------------------

        protected virtual void UpdateHead()
        {
            if (this.IsExpanded)
            {
                if (!this.m_HeadButton.ClassListContains(CLASS_HEAD_INFO_EXPANDED))
                {
                    this.m_HeadButton.AddToClassList(CLASS_HEAD_INFO_EXPANDED);
                }
            }
            else
            {
                this.m_HeadButton.RemoveFromClassList(CLASS_HEAD_INFO_EXPANDED);
            }

            if (this.ParentTool.AllowBreakpoint)
            {
                this.m_HeadBreakpoint.style.display = this.Breakpoint
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }

            if (this.ParentTool.AllowDisable)
            {
                this.m_HeadButton.style.opacity = this.IsEnabled ? 1f : 0.25f;
                this.m_HeadDisabled.style.display = this.IsEnabled
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }

            this.m_HeadLabel.text = this.Title;
            this.m_HeadLabel.style.color = this.Color;
            
            this.m_HeadImage.image = this.GetIcon();
        }

        protected virtual void UpdateBody(bool animate)
        {
            if (animate)
            {
                // TODO:
                // See if Unity will implement Scale uss symbol
                // and animate it to drop down/up.
                
                // Update 28/12/2020: Unity implemented Scale but it takes a float
                // instead of a Vector2. Meaning that the whole ITransform is batch-scaled
                
                // Update 16/11/2021: Unity implemented a new Transform property to set the
                // origin of an arbitrary object translation. However it does not change the
                // bounds of the element so it cannot slide up or down.

                // this.m_Body.style.transformOrigin = new TransformOrigin(Length.Percent(50), 0, 0);
                // this.m_Body.experimental.animation.Scale(this.IsExpanded ? 1f : 0f, 300);
                
                // Vector3 scale = this.IsExpanded
                //     ? new Vector3(1f, .5f, 1f)
                //     : new Vector3(1f, 0f, 1f);
                //
                // this.m_Body.style.transformOrigin = new TransformOrigin(Length.Percent(50), 0, 0);
                // this.m_Body.style.scale = new StyleScale(new Scale(scale));

                this.m_Body.style.display = this.IsExpanded
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
            else
            {
                this.m_Body.style.display = this.IsExpanded
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }

        // PROTECTED VIRTUAL METHODS: -------------------------------------------------------------

        protected virtual void OnChangePlayMode(PlayModeStateChange state)
        { }
        
        // DROP METHODS: --------------------------------------------------------------------------
        
        public void DisplayAsNormal()
        {
            this.m_Head.style.opacity = 1f;
            this.m_Body.style.opacity = 1f;

            this.m_DropAbove.style.display = DisplayStyle.None;
            this.m_DropBelow.style.display = DisplayStyle.None;
        }
        
        public void DisplayAsDrag()
        {
            this.m_Head.style.opacity = 0.25f;
            this.m_Body.style.opacity = 0.25f;
        }
        
        public void DisplayAsTargetAbove()
        {
            this.m_DropAbove.style.display = DisplayStyle.Flex;
        }
        
        public void DisplayAsTargetBelow()
        {
            this.m_DropBelow.style.display = DisplayStyle.Flex;
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------
        
        public static void SetIsExpanded(SerializedProperty property, bool isExpanded)
        {
            SessionState.SetBool(KeyIsExpanded(property), isExpanded);
        }
        
        public static string KeyIsExpanded(SerializedProperty property)
        {
            if (property?.serializedObject?.targetObject == null) return default;
            
            return string.Format(
                KEY_EXPANDED, 
                property.serializedObject.targetObject.GetInstanceID(),
                property.propertyPath
            );
        }
    }
}
