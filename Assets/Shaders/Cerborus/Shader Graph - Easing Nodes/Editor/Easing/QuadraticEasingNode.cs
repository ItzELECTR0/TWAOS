using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Quadratic Easing")]
	class QuadraticEasingNode : CodeFunctionNode
	{
		public QuadraticEasingNode() { name = "Quadratic Easing"; }

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
					methodName = "QuadraticIn";
					break;
				case EasingType.Out:
					methodName = "QuadraticOut";
					break;
				case EasingType.InOut:
					methodName = "QuadraticInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string QuadraticIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In * In;
}
";
		}

		static string QuadraticOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = -(In * (In - 2));
}
";
		}

		static string QuadraticInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In < 0.5)
        Out = 2 * In * In;
    else
        Out = -2 * In * In + 4 * In - 1;
}
";
		}
	}
}
