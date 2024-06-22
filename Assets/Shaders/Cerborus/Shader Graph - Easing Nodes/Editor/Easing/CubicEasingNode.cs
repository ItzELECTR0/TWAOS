using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Cubic Easing")]
	class CubicEasingNode : CodeFunctionNode
	{
		public CubicEasingNode() { name = "Cubic Easing"; }

		[SerializeField]
		EasingType _easingType = EasingType.In;

		[EnumControl("Type")]
		public EasingType EasingType {
			get => _easingType;
			set {
				if (_easingType == value) return;

				_easingType = value;
				Dirty(ModificationScope.Graph);
			}
		}

		protected override MethodInfo GetFunctionToConvert() {
			string methodName;

			switch (_easingType) {
				case EasingType.In:
					methodName = "CubicIn";
					break;
				case EasingType.Out:
					methodName = "CubicOut";
					break;
				case EasingType.InOut:
					methodName = "CubicInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string CubicIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In * In * In;
}
";
		}

		static string CubicOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    const float f = In - 1;
    Out = f * f * f + 1;
}
";
		}

		static string CubicInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In < 0.5)
        Out = 4 * In * In * In;
    else {
        const float f = 2 * In - 2;
        Out = 0.5 * f * f * f + 1;
    }
}
";
		}
	}
}
