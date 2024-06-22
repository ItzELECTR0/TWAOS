using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Quartic Easing")]
	class QuarticEasingNode : CodeFunctionNode
	{
		public QuarticEasingNode() { name = "Quartic Easing"; }

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
					methodName = "QuarticIn";
					break;
				case EasingType.Out:
					methodName = "QuarticOut";
					break;
				case EasingType.InOut:
					methodName = "QuarticInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string QuarticIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In * In * In * In;
}
";
		}

		static string QuarticOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    const float f = In - 1;
    Out = f * f * f * (1 - In) + 1;
}
";
		}

		static string QuarticInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In < 0.5)
        Out = 8 * In * In * In * In;
    else {
        const float f = In - 1;
        Out = -8 * f * f * f * f + 1;
    }
}
";
		}
	}
}
