using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Quintic Easing")]
	class QuinticEasingNode : CodeFunctionNode
	{
		public QuinticEasingNode() { name = "Quintic Easing"; }

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
					methodName = "QuinticIn";
					break;
				case EasingType.Out:
					methodName = "QuinticOut";
					break;
				case EasingType.InOut:
					methodName = "QuinticInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string QuinticIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In * In * In * In * In;
}
";
		}

		static string QuinticOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    const float f = In - 1;
    Out = f * f * f * f * f + 1;
}
";
		}

		static string QuinticInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In < 0.5)
        Out = 16 * In * In * In * In * In;
    else {
        const float f = 2 * In - 2;
        Out = 0.5 * f * f * f * f * f + 1;
    }
}
";
		}
	}
}
