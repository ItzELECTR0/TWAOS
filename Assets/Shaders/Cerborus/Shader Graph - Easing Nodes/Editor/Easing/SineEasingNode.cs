using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Sine Easing")]
	class SineEasingNode : CodeFunctionNode
	{
		public SineEasingNode() { name = "Sine Easing"; }

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
					methodName = "SineIn";
					break;
				case EasingType.Out:
					methodName = "SineOut";
					break;
				case EasingType.InOut:
					methodName = "SineInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string SineIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = sin((In - 1) * {EasingUtils.E_PI2}) + 1;
}}
";
		}

		static string SineOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = sin(In * {EasingUtils.E_PI2});
}}
";
		}

		static string SineInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = 0.5 * (1 - cos(In * {EasingUtils.E_PI}));
}}
";
		}
	}
}
