using System;
using System.Collections.Generic;
using DaimahouGames.Core.Runtime.Common;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class ListInspector<T> : ListInspector
    {
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public ListInspector(SerializedProperty property) : base(property) {}
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected override Button CreateElementButton()
        {
            return new Button(() =>
            {
                var instance = Activator.CreateInstance(typeof(T));
                InsertItem(m_Property.arraySize, instance);
            });
        }
        
        //============================================================================================================||
    }
    
    public abstract class ListInspector : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string USS_PATH = DaimahouPaths.COMMON + "StyleSheets/";
        private const string USS_HEAD_PATH = USS_PATH + "List-Head";
        private const string USS_BODY_PATH = USS_PATH + "List-Body";
        private const string USS_FOOT_PATH = USS_PATH + "List-Foot";    
        
        private const string CLASS_HEAD = "list-item-head";
        private const string CLASS_BODY = "list-body";
        private const string CLASS_FOOT = "list-foot";
        
        private const string ELEMENT_NAME_HEAD = "List-Head";
        private const string ELEMENT_NAME_BODY = "List-Body";
        private const string ELEMENT_NAME_FOOT = "List-Foot";
        private const string ELEMENT_NAME_BUTTON_ADD = "List-Foot-Add";
        
        public const string CLASS_HEAD_INFO = "list-item-head-info";
        
        private static readonly IIcon ICON_ADD = new IconInstructions(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ARR_D = new IconArrowDropDown(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ARR_R = new IconArrowDropRight(ColorTheme.Type.TextLight);
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        
        protected readonly SerializedProperty m_Property;

        protected readonly VisualElement m_Head;
        protected readonly VisualElement m_Body;
        protected readonly VisualElement m_Foot;
        

        protected Button m_ButtonAdd;
        protected Label m_ButtonAddLabel;
        
        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        protected Button m_HeadButton;
        
        private bool m_Expanded = true;
        private bool m_AnnotateIndex;
        
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|

        public string ItemPrefix { get; set; }
        public bool PrintItemNumber { get; set; }
        public SerializedProperty PropertyList => m_Property;
        protected SerializedObject SerializedObject => m_Property?.serializedObject;

        public bool AnnotateIndex
        {
            get => m_AnnotateIndex;
            set
            {
                m_AnnotateIndex = value;
                Refresh();
            }
        }

        public bool AllowReordering { get; set; } = true;
        public bool AllowDuplicating { get; set;  } = true;
        public bool AllowCopyPaste { get; set;  } = true;
        public bool AllowInsertion { get; set;  } = true;
        
        // ---|　Events ------------------------------------------------------------------------------------------>|

        public event Action<object> EventItemInserted;
        public event Action<int> EventChangeSize;
        
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※

        public ListInspector(SerializedProperty property)
        {
            m_Head = new VisualElement { name = ELEMENT_NAME_HEAD };
            m_Body = new VisualElement { name = ELEMENT_NAME_BODY };
            m_Foot = new VisualElement { name = ELEMENT_NAME_FOOT };
            
            m_Head.AddToClassList(CLASS_HEAD);
            m_Body.AddToClassList(CLASS_BODY);
            m_Foot.AddToClassList(CLASS_FOOT);
            
            m_Property = property;

            SetupHead();
            SetupBody();
            SetupFoot();

            SetStyles();
            Refresh();
        }
        
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        
        public void Refresh()
        {
            SerializedObject.Update();
            m_Body.Clear();

            for (var i = 0; i < m_Property.arraySize; ++i)
            {
                var elementInspector = MakeItemInspector(i);
                m_Body.Add(elementInspector);
                
                elementInspector.Prefix = ItemPrefix;
                elementInspector.PrintItemNumber = PrintItemNumber;
            }
            
            m_Body.style.display = m_Expanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            m_Foot.style.display = m_Expanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
                
            if(m_HeadImage != null) m_HeadImage.image = m_Expanded
                ? ICON_ARR_D.Texture
                : ICON_ARR_R.Texture;
        }
        
        public void InsertItem(int index, object value)
        {
            SerializedObject.Update();

            if (!ValidateInsertion(value))
            {
                Debug.Log($"Failed to insert {value} at index {index}");
                return;
            }
            
            m_Property.InsertArrayElementAtIndex(index);
            m_Property.GetArrayElementAtIndex(index).SetValue(value);
            
            SerializationUtils.ApplyUnregisteredSerialization(SerializedObject);

            EventItemInserted?.Invoke(value);
            
            var size = m_Property.arraySize;
            EventChangeSize?.Invoke(size);
            
            using var changeEvent = ChangeEvent<int>.GetPooled(size, size);
            changeEvent.target = this;
            SendEvent(changeEvent);
            
            Refresh();
        }

        public void DeleteItem(int index)
        {
            SerializedObject.Update();
            if (index < 0) return;

            m_Property.DeleteArrayElementAtIndex(index);
            SerializationUtils.ApplyUnregisteredSerialization(SerializedObject);

            var size = m_Property.arraySize;
            EventChangeSize?.Invoke(size);
            
            using var changeEvent = ChangeEvent<int>.GetPooled(size, size);
            changeEvent.target = this;
            SendEvent(changeEvent);
            
            Refresh();
        }

        public void DuplicateItem(int index)
        {
            SerializedObject.Update();

            if (index < 0) return;

            var source = m_Property.GetArrayElementAtIndex(index).GetManagedValue();
            if (source == null) return;
            
            m_Property.InsertArrayElementAtIndex(index);
            var newElement = m_Property.GetArrayElementAtIndex(index + 1);

            CopyPasteUtils.Duplicate(newElement, source);
            SerializationUtils.ApplyUnregisteredSerialization(SerializedObject);

            var size = m_Property.arraySize;
            EventChangeSize?.Invoke(size);
            
            using var changeEvent = ChangeEvent<int>.GetPooled(size, size);
            changeEvent.target = this;
            SendEvent(changeEvent);
            
            Refresh();
        }

        public void MoveItems(int sourceIndex, int destinationIndex)
        {
            SerializedObject.Update();

            destinationIndex = Math.Max(destinationIndex, 0);
            destinationIndex = Math.Min(destinationIndex, m_Property.arraySize - 1);

            m_Property.MoveArrayElement(
                sourceIndex,
                destinationIndex
            );

            SerializationUtils.ApplyUnregisteredSerialization(SerializedObject);
            Refresh();
        }

        public void SetTitle(string title)
        {
            if (title == null) return;
            
            m_Head.style.display = DisplayStyle.Flex;
            m_HeadLabel.text = title;
        }

        public void SetAddButtonText(string title)
        {
            if (title == null) return;
            
            m_ButtonAddLabel.text = title;
        }
        
        public SerializedProperty GetPropertyAt(int index)
        {
            return index < m_Property.arraySize ? m_Property.GetArrayElementAtIndex(index) : null;
        }
        
        public void CollapseItems()
        {
            SerializedObject.Update();
            
            var arraySize = m_Property.arraySize;
            for (var i = 0; i < arraySize; ++i)
            {
                var property = m_Property.GetArrayElementAtIndex(i);
                if(property.managedReferenceValue is IGenericItem item) item.IsExpanded = false;
            }

            SerializationUtils.ApplyUnregisteredSerialization(SerializedObject);
            //Refresh();
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※

        protected virtual GenericInspector MakeItemInspector(int index)
        {
            return new GenericInspector(this, index);
        }

        protected abstract Button CreateElementButton();

        protected virtual bool ValidateInsertion(object value) => true;
        
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        
        private void SetupHead()
        {
            hierarchy.Add(m_Head);
            
            m_HeadImage = new Image
            {
                image = m_Expanded
                    ? ICON_ARR_D.Texture
                    : ICON_ARR_R.Texture
            };
            
            m_HeadLabel = new Label("Undefined");
            
            m_HeadButton = new Button(() =>
            {
                m_Expanded = !m_Expanded;
                
                Refresh();
            });
            
            m_HeadButton.AddToClassList(CLASS_HEAD_INFO);

            m_HeadButton.Add(m_HeadImage);
            m_HeadButton.Add(m_HeadLabel);
            
            m_Head.Add(m_HeadButton);

            m_Head.style.display = DisplayStyle.None;
        }

        private void SetupBody() 
        {
            hierarchy.Add(m_Body);
        }
        
        private void SetupFoot()
        {
            hierarchy.Add(m_Foot);

            m_ButtonAdd = CreateElementButton();
            m_ButtonAdd.name = ELEMENT_NAME_BUTTON_ADD;
            
            m_ButtonAdd.Add(new Image { image = ICON_ADD.Texture });
            m_ButtonAdd.Add(m_ButtonAddLabel = new Label { text = $"Add new item..." });

            m_Foot.Add(m_ButtonAdd);
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