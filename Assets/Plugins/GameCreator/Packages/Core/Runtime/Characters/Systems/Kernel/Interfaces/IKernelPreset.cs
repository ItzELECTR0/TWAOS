using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Preset")]
    public interface IKernelPreset
    {
        TUnitPlayer MakePlayer { get; }
        TUnitMotion MakeMotion { get; }
        TUnitDriver MakeDriver { get; }
        TUnitFacing MakeFacing { get; }
        TUnitAnimim MakeAnimim { get; }
    }
}