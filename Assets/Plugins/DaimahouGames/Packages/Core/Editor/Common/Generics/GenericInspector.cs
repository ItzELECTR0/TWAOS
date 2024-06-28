using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Editor.Common;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TypeUtils = GameCreator.Editor.Common.TypeUtils;

namespace DaimahouGames.Editor.Core
{
    public class GenericInspector : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string USS_PATH = DaimahouPaths.COMMON + "StyleSheets/";
        private const string USS_HEAD_PATH = USS_PATH + "List-Head";
        private const string USS_BODY_PATH = USS_PATH + "List-Body";
        private const string USS_FOOT_PATH = USS_PATH + "List-Foot";
        
        private const string TIP_DELETE = "Delete";
        private const string TIP_DUPLICATE = "Duplicate";
        private const string TIP_EDIT = "Edit the element description";
        
        public const string CLASS_HEAD = "list-item-head";
        public const string CLASS_BODY = "list-item-body";
        
        public const string CLASS_DROP_ABOVE = "gc-list-item-drop-above";
        public const string CLASS_DROP_BELOW = "gc-list-item-drop-below";
        
        public const string CLASS_HEAD_DRAG = "list-item-head-drag";
        public const string CLASS_HEAD_INFO = "list-item-head-info";
        public const string CLASS_HEAD_INFO_EXPANDED = "list-item-head-info--expanded"; 
        public const string CLASS_HEAD_BREAKPOINT = "list-item-head-breakpoint";
        public const string CLASS_HEAD_DISABLE = "list-item-head-disabled";
        public const string CLASS_HEAD_BUTTON = "list-item-head-button";
        public const string CLASS_DROP = "list-item-drop";
        
        private static readonly IIcon ICON_DRAG = new IconDrag(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_DUPLICATE = new IconDuplicate(ColorTheme.Type.TextNormal);

        private static readonly IIcon ICON_BREAKPOINT = new IconBreakpoint(ColorTheme.Type.Red);
        private static readonly IIcon ICON_DISABLED = new IconCancel(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_REMOVE = new IconMinus(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_EDIT = new IconEdit(ColorTheme.Type.TextNormal);
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|

        protected readonly SerializedProperty m_Property;
        
        protected readonly int m_Index = -1;
        protected readonly ListInspector m_ListInspector;
        
        protected readonly VisualElement m_Head = new();
        protected readonly VisualElement m_Body = new();

        public readonly VisualElement m_DropAbove = new();
        public readonly VisualElement m_DropBelow = new();
        
        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        protected Button m_HeadButton;
        
        protected Button m_HeadEdit;
        
        protected Button m_HeadBreakpoint;
        protected Button m_HeadDisabled;
        protected Button m_HeadDelete;
        
        protected Label m_HeadDrag;
        protected Button m_HeadDuplicate;
        
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        
        public string Prefix { get; set; }
        public bool PrintItemNumber { get; set; }
        
        public int Index => m_Index;
        public ListInspector ListInspector => m_ListInspector;
        
        protected object PropertyValue => m_Property.propertyType == SerializedPropertyType.ManagedReference
            ? m_Property.managedReferenceValue
            : m_Property.GetSerializedValue<object>();
        
        public object DefaultPropertyValue => ItemType.IsValueType ? Activator.CreateInstance(ItemType) : null;
        
        protected Type ItemType => TypeUtils.GetTypeFromProperty(m_Property, false);
        
        public virtual bool IsExpanded
        {
            get
            {
                m_Property.serializedObject.Update();
                return PropertyValue is IGenericItem {IsExpanded: true};
            }
            set
            {
                m_Property.serializedObject.Update();
                if(PropertyValue is IGenericItem item) item.IsExpanded = value;
            }
        }
        
        public bool IsEnabled
        {
            get
            {
                m_Property.serializedObject.Update();
                return PropertyValue is not IEnable enable || enable.Enabled;
            }
            set
            {
                m_Property.serializedObject.Update();
                if(PropertyValue is IEnable item) item.Enabled = value;
            }
        }
        
        public bool Breakpoint
        {
            get
            {
                m_Property.serializedObject.Update();
                return PropertyValue is IBreakpoint {Breakpoint: true};
            }
            set
            {
                m_Property.serializedObject.Update();
                if(PropertyValue is IBreakpoint item) item.Breakpoint = value;
            }
        }

        public virtual string Title
        {
            get
            {
                m_Property.serializedObject.Update();

                var description = m_Property.FindPropertyRelative(GenericItem.DESCRIPTOR);
                if (description != null && !string.IsNullOrEmpty(description.stringValue))
                {
                    return description.stringValue;
                }
                
                var title = PropertyValue == null
                    ? $"select [{TypeUtils.GetNiceName(ItemType)}]"
                    : (PropertyValue as IGenericItem)?.Title ?? "Untitled";

                return PrintItemNumber ? $"{Prefix}{m_Index}:\t{title}" : $"{Prefix}{title}";
            }
        }

        protected virtual Color Color
        {
            get
            {
                m_Property.serializedObject.Update();
                return (PropertyValue as IGenericItem)?.Color ?? Color.gray;
            }
        }

        public bool IsGeneric { get; set; }
        public bool AllowDeletion => m_ListInspector != null;
        public bool AllowInsertion => m_ListInspector?.AllowInsertion ?? false;
        public bool AllowReordering => m_ListInspector?.AllowReordering ?? false;
        public bool AllowDuplicating => m_ListInspector?.AllowDuplicating ?? false;
        public bool AllowCopyPaste => m_ListInspector?.AllowCopyPaste ?? false;
        public bool AllowDisable => PropertyValue is IEnable;
        public bool AllowDocumentation => true;
        public virtual bool AllowBreakpoint => PropertyValue is IBreakpoint;
        public virtual bool AllowDescriptor => m_Property.FindPropertyRelative(GenericItem.DESCRIPTOR) != null;
        
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※

        public GenericInspector(ListInspector listInspector, int index)
        {
            m_Property = listInspector.GetPropertyAt(index);
            m_ListInspector = listInspector;
            m_Index = index;
            
            BuildInspector();
        }
        
        public GenericInspector(SerializedProperty property)
        {
            m_Property = property;
            BuildInspector();
        }

        private void BuildInspector()
        {
            m_Head.AddToClassList(CLASS_HEAD);
            m_Body.AddToClassList(CLASS_BODY);
            
            m_DropAbove.AddToClassList(CLASS_DROP_ABOVE);
            m_DropBelow.AddToClassList(CLASS_DROP_BELOW);
            
            SetStyles();
            
            Add(m_Head);
            Add(m_Body);
            
            Initialize();
        }

        ~GenericInspector()
        {
            OnDisable();
        }
        
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※

        private void Initialize()
        {
            SetupHead();
            SetupBody();
            SetupDrop();
            
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            EditorApplication.playModeStateChanged += OnChangePlayMode;
            
            OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.EnteredEditMode
            );
        }
        
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
        }
        
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public void Refresh()
        {
            m_Property.serializedObject.Update();
            
            UpdateHead();
            UpdateBody();
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        
        protected virtual void OnChangePlayMode(PlayModeStateChange state) { }

        protected virtual void SetupHead()
        {
            if (AllowReordering)
            {
                this.AddManipulator(new ElementDrop(this));
                SetupHeadReordering();
            }
            
            m_HeadImage = new Image();
            m_HeadLabel = new Label("Undefined");

            m_HeadButton = new Button(() =>
            {
                if (PropertyValue == null)
                {
                    m_HeadLabel.text = "(null)";
                    
                    TypeSelectorFancyPopup.Open(
                        m_Head, ItemType, 
                        OnReplace
                    );
                    
                    return;
                }
                
                IsExpanded = !IsExpanded;
                
                UpdateHead();
                UpdateBody();
            });

            var manipulator = new ContextualMenuManipulator(OnOpenHeadMenu);
            m_HeadButton.AddManipulator(manipulator);
            
            m_HeadButton.AddToClassList(CLASS_HEAD_INFO);

            m_HeadButton.Add(m_HeadImage);
            m_HeadButton.Add(m_HeadLabel);
            m_Head.Add(m_HeadButton);

            if (AllowBreakpoint) SetupHeadBreakpoint();
            if(AllowDescriptor) SetupHeadDescription();
            if(AllowDisable) SetupHeadDisable();
            if (AllowDuplicating) SetupHeadDuplicating();
            if(AllowDeletion) SetupHeadDeleting();
            
            UpdateHead();
        }
        
        protected virtual void SetupBody()
        {
            var fieldBody = new PropertyField
            {
                label = string.Empty
            };

            fieldBody.BindProperty(m_Property);
            fieldBody.RegisterValueChangeCallback(_ => UpdateHead());
            
            m_Body.Add(fieldBody);
            UpdateBody();
        }

        protected virtual void UpdateHead()
        {
            if (IsExpanded)
            {
                if (!m_HeadButton.ClassListContains(CLASS_HEAD_INFO_EXPANDED))
                {
                    m_HeadButton.AddToClassList(CLASS_HEAD_INFO_EXPANDED);
                }
            }
            else
            {
                m_HeadButton.RemoveFromClassList(CLASS_HEAD_INFO_EXPANDED);
            }

            if (AllowBreakpoint)
            {
                m_HeadBreakpoint.style.display = Breakpoint
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }

            if (AllowDisable)
            {
                m_HeadButton.style.opacity = IsEnabled ? 1f : 0.25f;
                m_HeadDisabled.style.display = IsEnabled
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
            
            m_HeadLabel.text = Title;
            m_HeadLabel.style.color = Color;
            
            SetIcon();
        }

        protected virtual void UpdateBody()
        {
            m_Body.style.display = IsExpanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
        
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        
        private void SetupHeadReordering()
        {
            m_HeadDrag = new Label();
            m_HeadDrag.AddManipulator(new ElementDrag(this));
            m_HeadDrag.AddManipulator(new ContextualMenuManipulator(OnOpenHeadMenu));
            m_HeadDrag.AddToClassList(CLASS_HEAD_DRAG);

            m_HeadDrag.Add(new Image
            {
                image = ICON_DRAG.Texture,
                focusable = false
            });
            
            m_Head.Add(m_HeadDrag);
        }
        
        private void SetupHeadDescription()
        {
            m_HeadEdit = new Button(() =>
            {
                InputDropdown.Open("Description", m_Head, result =>
                {
                    m_Property.serializedObject.Update();
                    m_Property.FindPropertyRelative(GenericItem.DESCRIPTOR).stringValue = result;
                    SerializationUtils.ApplyUnregisteredSerialization(m_Property.serializedObject);
                    Refresh();
                    ListInspector?.Refresh();
                });
            });
            
            m_HeadEdit.Add(new Image
            {
                image = ICON_EDIT.Texture
            });
            
            m_HeadEdit.AddToClassList(CLASS_HEAD_BUTTON);
            m_HeadEdit.tooltip = TIP_EDIT;
            m_Head.Add(m_HeadEdit);
        }

        private void SetupHeadBreakpoint()
        {
            m_HeadBreakpoint = new Button(() =>
            {
                Breakpoint = false;
                UpdateHead();
            });
            
            m_HeadBreakpoint.Add(new Image
            {
                image = ICON_BREAKPOINT.Texture,
                focusable = false
            });

            m_HeadBreakpoint.tooltip = "Pauses the Editor before reaching this element";
            m_HeadBreakpoint.AddToClassList(CLASS_HEAD_BREAKPOINT);
            m_Head.Add(m_HeadBreakpoint);
        }
        
        protected void SetupHeadDisable()
        {
            m_HeadDisabled = new Button(() =>
            {
                IsEnabled = true;
                UpdateHead();
            });
            
            m_HeadDisabled.Add(new Image
            {
                image = ICON_DISABLED.Texture,
                focusable = false
            });
            
            m_HeadDisabled.tooltip = "A disabled element is ignored";
            m_HeadDisabled.AddToClassList(CLASS_HEAD_DISABLE);
            m_Head.Add(m_HeadDisabled);
        }

        protected void SetupHeadDuplicating()
        {
            m_HeadDuplicate = new Button(() =>
            {
                ListInspector.DuplicateItem(m_Index);
            });
            
            m_HeadDuplicate.Add(new Image
            {
                image = ICON_DUPLICATE.Texture
            });
            
            m_HeadDuplicate.AddToClassList(CLASS_HEAD_BUTTON);
            m_HeadDuplicate.tooltip = TIP_DUPLICATE;
            m_Head.Add(m_HeadDuplicate);
        }
        
        protected void SetupHeadDeleting()
        {
            m_HeadDelete = new Button(() =>
            {
                m_Property.serializedObject.Update();
                m_Property.SetValue(DefaultPropertyValue);
                SerializationUtils.ApplyUnregisteredSerialization(m_Property.serializedObject);
                
                IsExpanded = false;
                
                UpdateHead();
                UpdateBody();

                m_ListInspector?.DeleteItem(m_Index);
            });

            m_HeadDelete.Add(new Image
            {
                image = ICON_REMOVE.Texture
            });
            
            m_HeadDelete.AddToClassList(CLASS_HEAD_BUTTON);
            m_HeadDelete.tooltip = TIP_DELETE;
            
            m_Head.Add(m_HeadDelete);
        }
        
        private void SetupDrop()
        {
            m_DropAbove.AddToClassList(CLASS_DROP);
            m_DropBelow.AddToClassList(CLASS_DROP);

            Insert(0, m_DropAbove);
            Insert(3, m_DropBelow);
        }
        
        protected virtual void OnOpenHeadMenu(ContextualMenuPopulateEvent menu)
        {
            if (AllowCopyPaste)
            {
                menu.menu.AppendAction(
                    "Copy", 
                    _ =>
                    {
                        CopyPasteUtils.SoftCopy(m_Property.GetManagedValue(), ItemType);
                    }
                );
                
                menu.menu.AppendAction(
                    "Paste", 
                    _ =>
                    {
                        ListInspector.InsertItem(m_Index, CopyPasteUtils.SourceObjectCopy);
                    },
                    _ => CopyPasteUtils.CanSoftPaste(ItemType)
                        ? DropdownMenuAction.Status.Normal
                        : DropdownMenuAction.Status.Disabled
                );

                menu.menu.AppendSeparator();
            }
            
            if (IsGeneric)
            {
                menu.menu.AppendAction(
                    "Replace...", 
                    _ =>
                    {
                        TypeSelectorFancyPopup.Open(
                            m_Head, ItemType, 
                            OnReplace
                        );
                    }
                );

                menu.menu.AppendSeparator();
            }
            
            if(AllowInsertion)
            {
                
                menu.menu.AppendAction(
                    "Insert Above...", 
                    _ =>
                    {
                        TypeSelectorFancyPopup.Open(
                            m_Head, ItemType, 
                            newType => OnInsert(newType)
                        );
                    }
                );
            
                menu.menu.AppendAction(
                    "Insert Below...", 
                    _ =>
                    {
                        TypeSelectorFancyPopup.Open(
                            m_Head, ItemType,
                            newType => OnInsert(newType)
                        );
                    }
                );

                menu.menu.AppendSeparator();
            }
            
            if (AllowBreakpoint)
            {
                menu.menu.AppendAction(
                    "Breakpoint", 
                    _ =>
                    {
                        Breakpoint = !Breakpoint;
                        UpdateHead();
                    },
                    _ => Breakpoint 
                        ? DropdownMenuAction.Status.Checked 
                        : DropdownMenuAction.Status.Normal
                );   
            }

            if (AllowDisable)
            {
                menu.menu.AppendAction(
                    IsEnabled ? "Disable" : "Enable", 
                    _ =>
                    {
                        IsEnabled = !IsEnabled;
                        UpdateHead();
                    }
                );
            }

            if (AllowDocumentation)
            {
                menu.menu.AppendAction(
                    "Help", 
                    _ => DocumentationPopup.Open(PropertyValue.GetType())
                );
            }
        }

        protected void OnInsert(Type newType)
        {
            var instance = Activator.CreateInstance(newType);
            m_ListInspector?.InsertItem(m_Index, instance);
        }

        private void OnReplace(Type newType)
        {
            m_Property.SetValue(Activator.CreateInstance(newType));
            m_Property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            
            m_Body.Clear();
            SetupBody();
            Refresh();
        }
        
        private void SetIcon()
        {
            m_Property.serializedObject.Update();

            var type = PropertyValue?.GetType() ?? ItemType;
            var iconAttrs = type?.GetCustomAttributes<ImageAttribute>();
            var icon = iconAttrs?.FirstOrDefault()?.Image;

            m_HeadImage.image = icon != null ? icon : Texture2D.whiteTexture;
            m_HeadImage.style.display = icon != null ? DisplayStyle.Flex : DisplayStyle.None;

            if(icon == null) m_HeadLabel.style.marginLeft = 8;
        }
        
        private void SetStyles()
        {
            var styleSheetsPaths = new List<string>
            {
                USS_HEAD_PATH,
                USS_BODY_PATH,
                USS_FOOT_PATH
            };

            var sheets = StyleSheetUtils.Load(styleSheetsPaths.ToArray());
            foreach (var sheet in sheets) styleSheets.Add(sheet);
        }
        
        //============================================================================================================||
    }
}