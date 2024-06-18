using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Side-Scroller XY")]
    [Category("Side-Scroller/Side-Scroller XY")]

    [Image(typeof(IconSquareSolid), ColorTheme.Type.Red, typeof(OverlayArrowRight))]
    [Description("Freezes the character Z translation axis and allows to move around its plane")]

    [Serializable]
    public class AxonometrySideScrollXY : TAxonometry
    {
        public override void ProcessPosition(TUnitDriver driver, Vector3 position)
        {
            base.ProcessPosition(driver, position);
            driver.Transform.position = new Vector3(position.x, position.y, 0f);
        }

        public override Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction)
        {
            return direction.x >= 0f ? Vector3.right : Vector3.left;
        }
        
        // CLONE: ---------------------------------------------------------------------------------

        public override object Clone()
        {
            return new AxonometrySideScrollXY();
        }
        
        // STRING: --------------------------------------------------------------------------------
        
        public override string ToString() => "Side-Scroll XY";
    }
}