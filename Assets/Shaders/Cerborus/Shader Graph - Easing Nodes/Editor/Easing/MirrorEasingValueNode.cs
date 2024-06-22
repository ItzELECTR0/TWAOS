using System.Reflection;
using UnityEditor.ShaderGraph;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Mirror Easing Value")]
	class MirrorEasingValueNode : CodeFunctionNode
	{
		public MirrorEasingValueNode() { name = "Mirror Easing Value"; }

		protected override MethodInfo GetFunctionToConvert() {
			return GetType().GetMethod("MirrorEasingValue", BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string MirrorEasingValue([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    if (In > 0.5)
        In = 1 - In;

    Out = In * 2;
}}
";
		}
	}
}
