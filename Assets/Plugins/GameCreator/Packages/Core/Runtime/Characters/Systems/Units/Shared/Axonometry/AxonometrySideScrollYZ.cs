using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Side-Scroller YZ")]
    [Category("Side-Scroller/Side-Scroller YZ")]

    [Image(typeof(IconSquareSolid), ColorTheme.Type.Green, typeof(OverlayArrowUp))]
    [Description("Freezes the character X translation axis and allows to move around its plane")]

    [Serializable]
    public class AxonometrySideScrollYZ : TAxonometry
    {
        public override void ProcessPosition(TUnitDriver driver, Vector3 position)
        {
            base.ProcessPosition(driver, position);
            driver.Transform.position = new Vector3(0f, position.y, position.z);
        }
        
        public override Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction)
        {
            return direction.z >= 0f ? Vector3.forward : Vector3.back;
        }
        
        // CLONE: ---------------------------------------------------------------------------------

        public override object Clone()
        {
            return new AxonometrySideScrollYZ();
        }
        
        // STRING: --------------------------------------------------------------------------------
        
        public override string ToString() => "Side-Scroll YZ";
    }
}