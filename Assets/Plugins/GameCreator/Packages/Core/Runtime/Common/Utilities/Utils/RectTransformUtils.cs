using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class RectTransformUtils
    {
        public static void RebuildChildren(RectTransform parent, GameObject prefab, int count)
        {
            int childrenCount = parent.childCount;
            if (childrenCount == count) return;
            
            if (childrenCount > count)
            {
                int removeCount = childrenCount - count;
                for (int i = 0; i < removeCount; ++i)
                {
                    Transform child = parent.GetChild(childrenCount - i - 1);
                    child.SetParent(null);
                    Object.Destroy(child.gameObject);
                }
            }
            else
            {
                int addCount = count - childrenCount;
                for (int i = 0; i < addCount; ++i)
                {
                    Object.Instantiate(prefab, parent);
                }
            }
        }
        
        public static void SetAndCenterToParent(RectTransform element, RectTransform parent)
        {
            element.SetParent(parent);

            element.localPosition = new Vector3(element.localPosition.x, element.localPosition.y, 0f);
            element.localScale = Vector3.one;
            element.localRotation = Quaternion.identity;
            
            element.anchorMin = new Vector2(0.5f, 0.5f);
            element.anchorMax = new Vector2(0.5f, 0.5f);
            element.pivot = new Vector2(0.5f, 0.5f);
            element.sizeDelta = Vector2.zero;
            element.offsetMin = Vector2.zero;
            element.offsetMax = Vector2.zero;
        }
        
        public static  void SetAndStretchToParentSize(RectTransform element, RectTransform parent)
        {
            element.SetParent(parent);

            element.localPosition = new Vector3(element.localPosition.x, element.localPosition.y, 0f);
            element.localScale = Vector3.one;
            element.localRotation = Quaternion.identity;
            
            element.anchorMin = new Vector2(0, 0);
            element.anchorMax = new Vector2(1, 1);
            element.pivot = new Vector2(0.5f, 0.5f);
            element.sizeDelta = Vector2.zero;
            element.offsetMin = Vector2.zero;
            element.offsetMax = Vector2.zero;
        }
    }
}