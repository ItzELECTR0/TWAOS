using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Serializable]
    public class TextReference
    {
        private enum Type
        {
            Text = 0,
            TMP = 1
        }

        private const int MAX_CHARACTER_COUNT = 99999;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Type m_Type = Type.Text;
        [SerializeField] private Text m_Text;
        [SerializeField] private TMP_Text m_TMP;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private string m_Value;
        [NonSerialized] private int m_CharactersVisible = MAX_CHARACTER_COUNT;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Text
        {
            get => this.m_Value;
            set
            {
                this.m_Value = value;
                switch (this.m_Type)
                {
                    case Type.Text:
                        this.RefreshLegacyText();
                        break;
                
                    case Type.TMP:
                        this.RefreshTMPText();
                        this.RefreshTMPCharactersVisible();
                        break;
                
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int CharactersVisible
        {
            get => this.m_CharactersVisible;
            set
            {
                this.m_CharactersVisible = value;
                switch (this.m_Type)
                {
                    case Type.Text:
                        this.RefreshLegacyText();
                        break;
                
                    case Type.TMP:
                        this.RefreshTMPCharactersVisible();
                        break;
                
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Color Color
        {
            get => this.m_Type switch
            {
                Type.Text => this.m_Text.color,
                Type.TMP => this.m_TMP.color,
                _ => throw new ArgumentOutOfRangeException()
            };
            set
            {
                switch (this.m_Type)
                {
                    case Type.Text: this.m_Text.color = value; break;
                    case Type.TMP: this.m_TMP.color = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool AreAllCharactersVisible => this.m_Value.Length <= this.CharactersVisible;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public TextReference()
        {
            this.m_Value = this.Text;
        }

        public TextReference(Text text) : this()
        {
            this.m_Type = Type.Text;
            this.m_Text = text;
        }

        public TextReference(TMP_Text text)
        {
            this.m_Type = Type.TMP;
            this.m_TMP = text;
        }

        // TO STRING: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Type switch
            {
                Type.Text => this.m_Text != null ? this.m_Text.gameObject.name : "(none)",
                Type.TMP => this.m_TMP != null ? this.m_TMP.gameObject.name : "(none)",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshLegacyText()
        {
            if (this.m_Text == null) return;
            int count = Math.Min(this.m_Value.Length, this.CharactersVisible);
            string newValue = this.m_Value[..count];
            if (newValue != this.m_Text.text)
            {
                this.m_Text.text = newValue;
            }
        }

        private void RefreshTMPText()
        {
            if (this.m_TMP == null) return;
            if (this.m_TMP.text != this.m_Value)
            {
                this.m_TMP.text = this.m_Value;
            }
        }

        private void RefreshTMPCharactersVisible()
        {
            int visibleCount = this.CharactersVisible;
            if (this.m_TMP.maxVisibleCharacters != visibleCount)
            {
                this.m_TMP.maxVisibleCharacters = visibleCount;
            }
        }
    }
}