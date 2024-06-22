using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Exponential Easing")]
	class ExponentialEasingNode : CodeFunctionNode
	{
		public ExponentialEasingNode() { name = "Exponential Easing"; }

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
					methodName = "ExponentialIn";
					break;
				case EasingType.Out:
					methodName = "ExponentialOut";
					break;
				case EasingType.InOut:
					methodName = "ExponentialInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string ExponentialIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In == 0.0 ? In : pow(2, 10 * (In - 1));
}
";
		}

		static string ExponentialOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    Out = In == 1.0 ? In : 1 - pow(2, -10 * In);
}
";
		}

		static string ExponentialInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return @"
{
    if (In == 0.0 || In == 1.0)
        Out = In;
    else if (In < 0.5)
        Out = 0.5 * pow(2, 20 * In - 10);
    else
        Out = -0.5 * pow(2, -20 * In + 10) + 1;
}
";
		}
	}
}
