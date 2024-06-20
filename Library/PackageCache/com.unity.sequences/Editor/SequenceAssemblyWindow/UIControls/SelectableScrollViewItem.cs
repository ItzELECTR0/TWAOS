using System;
using UnityEngine.Playables;

namespace UnityEngine.UIElements
{
    class SelectableScrollViewItem : BindableElement, IDisposable
    {
        public Action<SelectableScrollViewItem> itemSelected;
        internal PlayableDirector m_Director;

        public SelectableScrollViewItem()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public void OnMouseDown(MouseDownEvent evt)
        {
            SetSelected(true);
            itemSelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                AddToClassList(SelectableScrollView.itemSelectedVariantUssClassName);
            }
            else
            {
                RemoveFromClassList(SelectableScrollView.itemSelectedVariantUssClassName);
            }
        }

        internal virtual void BindItem(PlayableDirector director, GameObject instance)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
