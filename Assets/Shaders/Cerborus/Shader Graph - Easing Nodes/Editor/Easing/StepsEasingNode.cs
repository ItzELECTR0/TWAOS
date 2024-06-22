using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Steps Easing")]
	class StepsEasingNode : CodeFunctionNode
	{
		public StepsEasingNode() { name = "Steps Easing"; }

		[SerializeField]
		StepType _stepType = StepType.FirstStepZero;

		[EnumControl("Type")]
		public StepType StepType {
			get => _stepType;
			set {
				if (_stepType == value) return;

				_stepType = value;
				Dirty(ModificationScope.Graph);
			}
		}

		protected override MethodInfo GetFunctionToConvert() {
			string methodName;

			switch (_stepType) {
				case StepType.FirstStepZero:
					methodName = nameof(FirstStepZero);
					break;
				case StepType.LastStepOne:
					methodName = nameof(LastStepOne);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		static string FirstStepZero([Slot(0, Binding.None)] Vector1 In,
		                            [Slot(1, Binding.None, 4, 4, 4, 4)] Vector1 Steps,
		                            [Slot(2, Binding.None)] out Vector1 Out) {
			return @"
{
    Steps = max(Steps, 1);
    Out = floor(In * Steps) / Steps;
}
";
		}

		static string LastStepOne([Slot(0, Binding.None)] Vector1 In,
		                          [Slot(1, Binding.None, 4, 4, 4, 4)] Vector1 Steps,
		                          [Slot(2, Binding.None)] out Vector1 Out) {
			return @"
{
    Steps = max(Steps, 1);
    Out = floor((In + 1/Steps) * Steps) / Steps;
}
";
		}
	}
}
