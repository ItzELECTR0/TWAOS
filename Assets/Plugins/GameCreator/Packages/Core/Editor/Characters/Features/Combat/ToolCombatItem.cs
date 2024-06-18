using System;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class ToolCombatItem : VisualElement
    {
        private const string NAME_ROOT = "GC-Characters-Combat-Item-Root";
        private const string NAME_HEAD = "GC-Characters-Combat-Item-Head";
        private const string NAME_BODY = "GC-Characters-Combat-Item-Body";
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly VisualElement m_Root;
        [NonSerialized] private readonly VisualElement m_Head;
        [NonSerialized] private readonly VisualElement m_Body;

        [NonSerialized] private readonly Image m_HeadImage;
        [NonSerialized] private readonly Label m_HeadLabel;

        [NonSerialized] private readonly Weapon m_Weapon;
        [NonSerialized] private readonly TMunitionValue m_Munition;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ToolCombatItem(Weapon weapon, TMunitionValue munition)
        {
            this.m_Weapon = weapon;
            this.m_Munition = munition;
            
            this.m_Root = new VisualElement { name = NAME_ROOT };
            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);

            this.Add(this.m_Root);

            this.m_HeadImage = new Image { image = weapon.Asset?.EditorIcon };
            this.m_HeadLabel = new Label(weapon.Asset != null
                ? weapon.Asset.ToString()
                : "(unknown)"
            );
            
            this.m_Head.Add(this.m_HeadImage);
            this.m_Head.Add(this.m_HeadLabel);

            this.m_Head.RegisterCallback<MouseDownEvent>(_ =>
            {
                bool newState = !SessionState.GetBool(this.m_Weapon.Id.ToString(), false);
                SessionState.SetBool(this.m_Weapon.Id.ToString(), newState);

                this.m_Body.style.display = newState 
                    ? DisplayStyle.Flex 
                    : DisplayStyle.None;
            });
            
            this.m_Body.style.display = SessionState.GetBool(this.m_Weapon.Id.ToString(), false) 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;

            this.m_Munition.EventChange -= this.Refresh;
            this.m_Munition.EventChange += this.Refresh;
            
            this.Refresh();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Refresh()
        {
            this.m_HeadImage.image = this.m_Weapon.Asset?.EditorIcon;
            this.m_HeadLabel.text = this.m_Weapon.Asset != null
                ? this.m_Weapon.Asset.ToString()
                : "(unknown)";
            
            this.m_Body.Clear();
            this.m_Body.Add(new Label(this.m_Munition.ToString()));
        }
    }
}