using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Circular Easing")]
	class CircularEasingNode : CodeFunctionNode
	{
		public CircularEasingNode() { name = "Circular Easing"; }

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
					methodName = "CircularIn";
					break;
				case EasingType.Out:
					methodName = "CircularOut";
					break;
				case EasingType.InOut:
					methodName = "CircularInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string CircularIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = 1 - sqrt(1 - In * In);
}
";
		}

		static string CircularOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = sqrt((2 - In) * In);
}
";
		}

		static string CircularInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In < 0.5)
        Out = 0.5 * (1 - sqrt(1 - 4 * (In * In)));
    else
        Out = 0.5 * (sqrt(-(2 * In - 3) * (2 * In - 1)) + 1);
}
";
		}
	}
}
