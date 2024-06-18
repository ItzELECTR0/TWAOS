using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("3D Character Controller")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Green)]
    
    [Category("3D Character Controller")]
    [Description("Configures the default 3D character controller")]

    [Serializable]
    public class KernelPreset3DController : IKernelPreset
    {
        public TUnitPlayer MakePlayer => new UnitPlayerDirectional();
        public TUnitMotion MakeMotion => new UnitMotionController();
        public TUnitDriver MakeDriver => new UnitDriverController();
        public TUnitFacing MakeFacing => new UnitFacingPivot();
        public TUnitAnimim MakeAnimim => new UnitAnimimKinematic();
    }
}