using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TListView<T> : VisualElement
    {
        // MEMBERS: -------------------------------------------------------------------------------

        protected readonly T m_Runtime;
        
        protected readonly VisualElement m_Head;
        protected readonly VisualElement m_Body;
        protected readonly VisualElement m_Foot;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract string USSPath { get; }
    
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TListView(T runtime)
        {
            this.m_Runtime = runtime;

            StyleSheet[] sheets = StyleSheetUtils.Load(
                this.USSPath,
                EditorPaths.COMMON + "Polymorphism/Lists/StyleSheets/Polymorphic-List"
            );
            
            this.m_Head = new VisualElement { name = NameListTool.NAME_HEAD };
            this.m_Body = new VisualElement { name = NameListTool.NAME_BODY };
            this.m_Foot = new VisualElement { name = NameListTool.NAME_FOOT };
            
            this.m_Head.AddToClassList(TPolymorphicListTool.CLASS_HEAD);
            this.m_Body.AddToClassList(TPolymorphicListTool.CLASS_BODY);
            this.m_Foot.AddToClassList(TPolymorphicListTool.CLASS_FOOT);
            
            foreach (StyleSheet styleSheet in sheets) this.styleSheets.Add(styleSheet);
            this.Refresh();
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void Refresh()
        {
            this.Clear();
        }
    }
}