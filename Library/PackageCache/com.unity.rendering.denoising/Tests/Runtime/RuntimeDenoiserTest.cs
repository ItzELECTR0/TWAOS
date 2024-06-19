using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.Rendering.Denoising;

class RuntimeDenoiserTest
{
    static DenoiserType[] FixtureArgs =
    {
        DenoiserType.OpenImageDenoise,
        DenoiserType.Optix
    };

    void PlayModeCreateDenoiserTest(DenoiserType denoiserType) 
	{
		if (Denoiser.IsDenoiserTypeSupported(denoiserType) == false)
            return;

        // Create denoiser.
        Denoiser denoiser = new Denoiser();

        Denoiser.State result = denoiser.Init(denoiserType, 128, 128, 0, 0);
        Assert.AreEqual(Denoiser.State.Success, result);
    }

    [TestCaseSource("FixtureArgs")]
    public void CreateDenoiser(DenoiserType denoiserType)
    {
        PlayModeCreateDenoiserTest(denoiserType);
    }
    [Test]
    public void CreateDenoiser_OfTypeNone_Fails()
    {
        Denoiser denoiser = new Denoiser();
        Denoiser.State result = denoiser.Init(DenoiserType.None, 128, 128, 0, 0);
        Assert.AreEqual(Denoiser.State.Failure, result);
    }
}
