using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Text")]
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    
    [Category("UI/Change Text")]
    [Description("Changes the chosen Text value")]

    [Serializable]
    public class SpotChangeText : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected PropertySetString m_Change = SetStringUIText.Create;
        [SerializeField] protected PropertyGetString m_Text = new PropertyGetString();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Change} = {this.m_Text}";

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnEnable(Hotspot hotspot)
        {
            base.OnEnable(hotspot);

            string text = this.m_Text.Get(hotspot.Args);
            this.m_Change.Set(text, hotspot.Args);
        }
    }
}