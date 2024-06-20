using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    partial class SequenceAssetFoldoutItem : SelectableScrollViewItem, INotifyValueChanged<bool>
    {
        public new class UxmlFactory : UxmlFactory<SequenceAssetFoldoutItem, UxmlTraits> {}

        static readonly string ussFoldoutDepthClassName = "unity-foldout--depth-";
        static readonly int ussFoldoutMaxDepth = 4;

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription { name = "value", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                Foldout f = ve as Foldout;
                if (f != null)
                {
                    f.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
                }
            }
        }

        public event Action<SequenceAssetFoldoutItem, GameObject, GameObject> newVariantSelection;

        Toggle m_Toggle;
        TextField m_SequenceAssetNameField;
        VisualElement m_HeaderContainer;
        VisualElement m_Container;

        ISequenceAssetView m_SequenceAssetView;

        GameObject m_SequenceAssetSource;  // Prefab asset source
        GameObject m_SequenceAssetSelected;  // Prefab asset source or variant currently selected
        GameObject m_SequenceAssetSelectedInstance;  // Prefab instance of m_SequenceAssetSelected

        public override VisualElement contentContainer => m_Container;

        object nameSuffix => m_Toggle.value ? "" : " (" + m_SequenceAssetView.variantsSelector.value.name + ")";
        string displayName => m_SequenceAssetSource.name + nameSuffix;

        [SerializeField] bool m_Value;

        public bool value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;

                using (ChangeEvent<bool> evt = ChangeEvent<bool>.GetPooled(m_Value, value))
                {
                    evt.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(evt);
                }
            }
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            m_Value = newValue;
            m_Toggle.value = m_Value;
            if (newValue)
            {
                RemoveFromClassList(foldedUssClassName);
                contentContainer.RemoveFromClassList(contentFoldedUssClassName);
            }
            else
            {
                AddToClassList(foldedUssClassName);
                contentContainer.AddToClassList(contentFoldedUssClassName);
            }
            parent.contentContainer.MarkDirtyRepaint();
            m_SequenceAssetNameField.SetValueWithoutNotify(displayName);
        }

        public static readonly string ussClassName = "unity-foldout";
        public static readonly string foldedUssClassName = ussClassName + "--folded";
        public static readonly string toggleUssClassName = ussClassName + "__toggle";
        public static readonly string contentUssClassName = ussClassName + "__content";
        public static readonly string contentFoldedUssClassName = contentUssClassName + "--folded";

        public SequenceAssetFoldoutItem()
        {
            m_Value = true;
            AddToClassList(ussClassName);

            m_HeaderContainer = new VisualElement();
            m_HeaderContainer.style.flexDirection = FlexDirection.Row;
            hierarchy.Add(m_HeaderContainer);

            m_Toggle = new Toggle
            {
                value = true
            };
            m_Toggle.RegisterValueChangedCallback((evt) =>
            {
                value = m_Toggle.value;
                evt.StopPropagation();
            });
            m_Toggle.AddToClassList(toggleUssClassName);
            m_HeaderContainer.Add(m_Toggle);

            m_SequenceAssetNameField = new TextField();
            m_SequenceAssetNameField.style.flexGrow = 1;
            m_SequenceAssetNameField.isDelayed = true;
            m_SequenceAssetNameField.RegisterCallback<ChangeEvent<string>>(OnSequenceAssetNameChange);
            m_SequenceAssetNameField.RegisterCallback<FocusInEvent>(OnFocusIn);
            m_SequenceAssetNameField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            m_HeaderContainer.Add(m_SequenceAssetNameField);

            m_Container = new VisualElement()
            {
                name = "unity-content",
            };
            m_Container.AddToClassList(contentUssClassName);
            hierarchy.Add(m_Container);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        public SequenceAssetFoldoutItem(PlayableDirector director) : this()
        {
            m_Director = director;
        }

        public override void Dispose()
        {
            m_SequenceAssetView.variantSelected -= OnVariantSelected;
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            var depth = 0;
            // Remove from all the depth classes...
            for (int i = 0; i <= ussFoldoutMaxDepth; i++)
            {
                RemoveFromClassList(ussFoldoutDepthClassName + i);
            }
            RemoveFromClassList(ussFoldoutDepthClassName + "max");

            // Figure out the real depth of this actual Foldout...
            if (parent != null)
            {
                var curParent = parent;
                while (curParent != null)
                {
                    if (curParent.GetType() == typeof(Foldout))
                    {
                        depth++;
                    }
                    curParent = curParent.parent;
                }
            }

            // Add the class name corresponding to that depth
            if (depth > ussFoldoutMaxDepth)
            {
                AddToClassList(ussFoldoutDepthClassName + "max");
            }
            else
            {
                AddToClassList(ussFoldoutDepthClassName + depth);
            }
        }

        void OnSequenceAssetNameChange(ChangeEvent<string> changeEvent)
        {
            var isValid =
                FilePathUtility.SanitizeAndValidateName(
                    changeEvent.previousValue,
                    changeEvent.newValue,
                    out var sanitizedName);

            var rename = false;
            if (isValid)
            {
                rename = EditorUtility.DisplayDialog("Rename Sequence asset?",
                    $"Rename Sequence asset \"{changeEvent.previousValue}\" to \"{sanitizedName}\"?",
                    "Rename", "Cancel");
            }

            if (!rename)
            {
                // We end up here if (1) the new name is invalid or (2) users chose to cancel the renaming.
                m_SequenceAssetNameField.SetValueWithoutNotify(changeEvent.previousValue);
                return;
            }

            SequenceAssetUtility.Rename(m_SequenceAssetSource, changeEvent.previousValue, changeEvent.newValue);
        }

        public void AssignSequenceAsset(GameObject instance)
        {
            m_SequenceAssetSource = SequenceAssetUtility.GetSource(instance);
            m_SequenceAssetSelected = SequenceAssetUtility.GetAssetFromInstance(instance);
            m_SequenceAssetSelectedInstance = instance;

            m_SequenceAssetNameField.value = m_SequenceAssetSource.name;
        }

        public void AddVariants(GameObject sequenceAsset)
        {
            var builder = SequenceAssetBuilder.GetBuilder(sequenceAsset.GetType());
            m_SequenceAssetView = builder.CreateSequenceAssetView();
            m_SequenceAssetView.variantSelected += OnVariantSelected;
            m_SequenceAssetView.Bind(sequenceAsset, m_Director);

            contentContainer.Add(m_SequenceAssetView.root);
        }

        void OnVariantSelected(GameObject previousVariant, GameObject newVariant)
        {
            // previousVariant is an asset and newVariant as well.
            // But the Foldout object knows the instance of the previousVariant and that's what matters.
            if (previousVariant != m_SequenceAssetSelected)
                return;

            newVariantSelection?.Invoke(this, m_SequenceAssetSelectedInstance, newVariant);
        }

        internal void SetVariantSelection(GameObject instanceSelection)
        {
            m_SequenceAssetSelected = SequenceAssetUtility.GetAssetFromInstance(instanceSelection);
            m_SequenceAssetSelectedInstance = instanceSelection;
            m_SequenceAssetView.variantsSelector.SetValueWithoutNotify(m_SequenceAssetSelected);
        }

        void OnFocusIn(FocusInEvent evt)
        {
            m_SequenceAssetNameField.SetValueWithoutNotify(m_SequenceAssetSource.name);
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            m_SequenceAssetNameField.SetValueWithoutNotify(displayName);
        }

        internal override void BindItem(PlayableDirector director, GameObject assetInstance)
        {
            m_Director = director;
            AssignSequenceAsset(assetInstance);
            AddVariants(assetInstance);

            newVariantSelection -= SelectSequenceAssetVariant;
            newVariantSelection += SelectSequenceAssetVariant;
        }

        void SelectSequenceAssetVariant(
            SequenceAssetFoldoutItem item,
            GameObject oldSelectionInstance,
            GameObject newSelectionAsset)
        {
            InteractionMode interactionMode =
                oldSelectionInstance == null ? InteractionMode.AutomatedAction : InteractionMode.UserAction;

            var newSelectionInstance = SequenceAssetUtility.UpdateInstanceInSequence(
                oldSelectionInstance,
                newSelectionAsset,
                m_Director,
                interactionMode);

            item.SetVariantSelection(newSelectionInstance == null ? oldSelectionInstance : newSelectionInstance);
        }
    }
}
