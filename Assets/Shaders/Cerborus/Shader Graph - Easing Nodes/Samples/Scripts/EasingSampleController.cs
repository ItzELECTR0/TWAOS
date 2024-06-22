using UnityEngine;
using UnityEngine.UI;

namespace Cerborus.Samples.Scripts
{
	public class EasingSampleController : MonoBehaviour
	{
		static readonly int EasingValue = Shader.PropertyToID("_EasingValue");

		[SerializeField]
		Slider easingValueSlider;

		[SerializeField]
		Material easingMaterial;

		void Start() {
			SetEasingValueOfShader(0f);
			easingValueSlider.onValueChanged.AddListener(SetEasingValueOfShader);
		}

		void SetEasingValueOfShader(float value) { easingMaterial.SetFloat(EasingValue, value); }
	}
}
