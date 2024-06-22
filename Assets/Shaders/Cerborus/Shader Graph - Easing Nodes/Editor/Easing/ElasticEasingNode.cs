using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Elastic Easing")]
	class ElasticEasingNode : CodeFunctionNode
	{
		public ElasticEasingNode() { name = "Elastic Easing"; }

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
					methodName = "ElasticIn";
					break;
				case EasingType.Out:
					methodName = "ElasticOut";
					break;
				case EasingType.InOut:
					methodName = "ElasticInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string ElasticIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = sin(13 * {EasingUtils.E_PI2} * In) * pow(2, 10 * (In - 1));
}}
";
		}

		static string ElasticOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = sin(-13 * {EasingUtils.E_PI2} * (In + 1)) * pow(2, -10 * In) + 1;
}}
";
		}

		static string ElasticInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    if (In < 0.5)
        Out = 0.5 * sin(13 * {EasingUtils.E_PI2} * (2 * In)) * pow(2, 10 * (2 * In - 1));
    else
        Out = 0.5 * (sin(-13 * {EasingUtils.E_PI2} * (2 * In - 1 + 1)) * pow(2, -10 * (2 * In - 1)) + 2);
}}
";
		}
	}
}
