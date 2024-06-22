using System;
using System.Reflection;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace Cerborus.Editor.Easing
{
	[Title("Math", "Easing", "Bounce Easing")]
	class BounceEasingNode : CodeFunctionNode
	{
		const string Bounce_A = "0.3636364"; // 4 / 11.0

		const string Bounce_B = "0.7272727"; // 8 / 11.0	
		const string Bounce_B1 = "9.075";    // 363 / 40.0
		const string Bounce_B2 = "9.9";      // 99 / 10.0	
		const string Bounce_B3 = "3.4";      // 17 / 5.0	

		const string Bounce_C = "0.9";       // 9 / 10.0	
		const string Bounce_C1 = "12.06648"; // 4356 / 361.0	
		const string Bounce_C2 = "19.63546"; // 35442 / 1805.0	
		const string Bounce_C3 = "8.898061"; // 16061 / 1805.0	

		const string Bounce_D1 = "10.8";  // 54 / 5.0	
		const string Bounce_D2 = "20.52"; // 513 / 25.0	
		const string Bounce_D3 = "10.72"; // 268 / 25.0	

		public BounceEasingNode() { name = "Bounce Easing"; }

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
					methodName = "BounceIn";
					break;
				case EasingType.Out:
					methodName = "BounceOut";
					break;
				case EasingType.InOut:
					methodName = "BounceInOut";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		}

		public override void GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode) {
			BounceOutFunction(registry);
			base.GenerateNodeFunction(registry, generationMode);
		}

		const string BounceOutFunctionName = "BounceOutFunction";

		static void BounceOutFunction(FunctionRegistry registry) {
			registry.ProvideFunction(BounceOutFunctionName, s => s.Append(@"
inline float " + BounceOutFunctionName + $@"(const float In)
{{
    if (In < {Bounce_A}) return 121 * In * In / 16.0;
    if (In < {Bounce_B}) return {Bounce_B1} * In * In - {Bounce_B2} * In + {Bounce_B3};
    if (In < {Bounce_C}) return {Bounce_C1} * In * In - {Bounce_C2} * In + {Bounce_C3};
    return {Bounce_D1} * In * In - {Bounce_D2} * In + {Bounce_D3};
}}"));
		}

		static string BounceIn([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = 1 - {BounceOutFunctionName}(1 - In);
}}
";
		}

		static string BounceOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    Out = {BounceOutFunctionName}(In);
}}
";
		}

		static string BounceInOut([Slot(0, Binding.None)] Vector1 In, [Slot(1, Binding.None)] out Vector1 Out) {
			return $@"
{{
    if (In < 0.5)
        Out = 0.5 * (1 - {BounceOutFunctionName}(1 - In * 2));
    else
        Out = 0.5 * {BounceOutFunctionName}(In * 2 - 1) + 0.5;
}}
";
		}
	}
}
