namespace Cerborus.Editor.Easing
{
	internal static class EasingUtils
	{
		internal const string E_PI = "3.14159265358979323846";
		internal const string E_PI2 = "1.57079632679489661923";
	}

	internal enum EasingType
	{
		In,
		Out,
		InOut
	}

	internal enum StepType
	{
		FirstStepZero,
		LastStepOne
	}
}
