using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Back Easing")]
	class BackEasingNode : CodeFunctionNode
	{
		public BackEasingNode() { name = "Back Easing"; }

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
					methodName = "BackIn";
					break;
				case EasingType.Out:
					methodName = "BackOut";
					break;
				case EasingType.InOut:
					methodName = "BackInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string BackIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = In * In * In - In * sin(In * {EasingUtils.E_PI});
}}
";
		}

		static string BackOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    const float f = 1 - In;
    Out = 1 - (f * f * f - f * sin(f * {EasingUtils.E_PI}));
}}
";
		}

		static string BackInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    float f;

    if (In < 0.5)
    {{
        f = 2 * In;
        Out = 0.5 * (f * f * f - f * sin(f * {EasingUtils.E_PI}));
    }}
    else
    {{
        f = 1 - (2 * In - 1);
        Out = 0.5 * (1 - (f * f * f - f * sin(f * {EasingUtils.E_PI}))) + 0.5;
    }}
}}
";
		}
	}
}
