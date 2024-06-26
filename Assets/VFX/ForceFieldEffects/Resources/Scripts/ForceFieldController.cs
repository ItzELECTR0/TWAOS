using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldController : MonoBehaviour {

    public enum FFstate { SingleSpheres, MultipleSpheres };
    public FFstate forceFieldMode = FFstate.SingleSpheres;

    public int affectorCount = 20;
    [Range (-2,2)]
    public float openCloseProgress = 2;
    public bool openAutoAnimation = true;
    public float openSpeed = 0.6f;
    public AnimationCurve openCurve;

    public Material[] materialLayers;

    public bool procedrualGradientEnabled = true;
    public bool procedrualGradientUpdate = true;
    public Gradient procedrualGradientRamp;
    public Color procedrualRampColorTint = Color.white;

    public ParticleSystem controlParticleSystem;
    public GameObject getRenderersInChildren;
    public Renderer[] getRenderersCustom;

    private Renderer[] renderers;
    private Texture2D rampTexture;

    private Vector4[] spherePositions;
    private float[] sphereSizes;
    private int numberOfSpheres;
    private int numberOfSpheresOld;

    private ParticleSystem.Particle[] controlParticles;
    private Vector4[] controlParticlesPositions;
    private float[] controlParticlesSizes;
    private List<Material> rendererMaterials = new List<Material>();
    private ParticleSystem.MainModule psmain;

    private float openCloseValue;
    private float openCloseCurve;

    // Use this for initialization
    void Start () {
        psmain = controlParticleSystem.main;

        GetRenderers();
        GetNumberOfSpheres();
        GetSphereArrays();
        ApplyMaterials();

        if (procedrualGradientEnabled == true)
        {
            UpdateRampTexture();
        }        
    }

    // For Better Effects Change in DemoScene
    void OnEnable()
    {
        psmain = controlParticleSystem.main;

        GetRenderers();
        GetNumberOfSpheres();
        GetSphereArrays();

        controlParticles = new ParticleSystem.Particle[affectorCount];
        controlParticlesPositions = new Vector4[affectorCount];
        controlParticlesSizes = new float[affectorCount];
        psmain.maxParticles = affectorCount;
        //controlParticleSystem.maxParticles = affectorCount;
        controlParticleSystem.GetParticles(controlParticles);
        for (int i = 0; i < affectorCount; i++)
        {
            controlParticlesPositions[i] = controlParticles[i].position;
            controlParticlesSizes[i] = controlParticles[i].GetCurrentSize(controlParticleSystem) * controlParticleSystem.transform.lossyScale.x;
        }

        OpenCloseProgress();
        UpdateHitWaves();
    }

    // Update is called once per frame
    void Update () {
        GetNumberOfSpheres();

        if (numberOfSpheres != numberOfSpheresOld)
        {
            GetRenderers();
            ApplyMaterials();
        }
        numberOfSpheresOld = numberOfSpheres;

        GetSphereArrays();
        if (procedrualGradientEnabled == true)
        {
            if (procedrualGradientUpdate == true)
            {
                UpdateRampTexture();
            }
        }

        controlParticles = new ParticleSystem.Particle[affectorCount];
        controlParticlesPositions = new Vector4[affectorCount];
        controlParticlesSizes = new float[affectorCount];
        psmain.maxParticles = affectorCount;
        //controlParticleSystem.maxParticles = affectorCount;
        controlParticleSystem.GetParticles(controlParticles);
        for (int i = 0; i < affectorCount; i++)
        {
            controlParticlesPositions[i] = controlParticles[i].position;
            controlParticlesSizes[i] = controlParticles[i].GetCurrentSize(controlParticleSystem) * controlParticleSystem.transform.lossyScale.x;
        }
        UpdateHitWaves();

        if (openAutoAnimation == true)
        {
            OpenCloseProgress();
        }        
    }

    private void GetNumberOfSpheres()
    {
        //numberOfSpheres = renderers.Length;
        if (getRenderersCustom.Length > 0)
        {
            numberOfSpheres = getRenderersCustom.Length;
        }
        else
        {
            numberOfSpheres = getRenderersInChildren.transform.childCount;
        }
    }

    private void GetSphereArrays()
    {
        spherePositions = new Vector4[numberOfSpheres];
        sphereSizes = new float[numberOfSpheres];
        for (int i = 0; i < numberOfSpheres; i++)
        {
            spherePositions[i] = renderers[i].gameObject.transform.position;
            sphereSizes[i] = renderers[i].gameObject.transform.lossyScale.x;
        }
    }

    // Open Animation Progress
    private void OpenCloseProgress()
    {
        if (openCloseValue < 1f)
        {
            openCloseValue += Time.deltaTime * openSpeed;
        }
        else
        {
            openCloseValue = 1f;
        }
        openCloseCurve = openCurve.Evaluate(openCloseValue);
        openCloseProgress = openCloseCurve;
    }

    // Set Open Value from other Scripts
    public void SetOpenCloseValue(float val)
    {
        if (openAutoAnimation == true)
        {
            openCloseValue = val;
        }        
    }

    // Generating a texture from gradient variable
    Texture2D GenerateTextureFromGradient(Gradient grad)
    {
        float width = 256;
        float height = 1;
        Texture2D text = new Texture2D((int)width, (int)height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color col = grad.Evaluate(0 + (x / width));
                text.SetPixel(x, y, col);
            }
        }
        text.wrapMode = TextureWrapMode.Clamp;
        text.Apply();        
        return text;
    }

    // Applying material layers to objects
    public void ApplyMaterials()
    {
        for (int i = 0; i < materialLayers.Length; i++)
        {
            materialLayers[i] = new Material(materialLayers[i]);
        }
        foreach (Renderer rend in renderers)
        {
            rendererMaterials.Clear();
            foreach (Material mat in rend.sharedMaterials)
            {
                bool isEffect = false;
                for (int i = 0; i < materialLayers.Length; i++)
                {
                    if (materialLayers[i].name == mat.name)
                    {
                        isEffect = true;
                    }
                }
                if (isEffect != true)
                {
                    rendererMaterials.Add(mat);
                }
            }
            foreach (Material matt in materialLayers)
            {
                rendererMaterials.Add(matt);
            }
            rend.materials = rendererMaterials.ToArray();
        }
    }

    // Update procedural ramp textures and applying them to the shaders
    public void UpdateRampTexture()
    {
        rampTexture = GenerateTextureFromGradient(procedrualGradientRamp);
        GetRenderers();

        foreach (Renderer rend in renderers)
        {
            foreach (Material matt in materialLayers)
            {
                matt.SetTexture("_Ramp", rampTexture);
                matt.SetColor("_RampColorTint", procedrualRampColorTint);
            }
        }
    }

    // Getting all renderers for ForceField meshes
    public void GetRenderers()
    {
        if (getRenderersCustom.Length > 0)
        {
            renderers = getRenderersCustom;
        }
        else
        {
            renderers = getRenderersInChildren.GetComponentsInChildren<Renderer>();
        }
    }

    // Update Hit Waves and Control Particles
    public void UpdateHitWaves()
    {
        foreach (Renderer rend in renderers)
        {
            switch (forceFieldMode)
            {
                case FFstate.SingleSpheres:
                    foreach (Material matt in materialLayers)
                    {
                        matt.SetVectorArray("_ControlParticlePosition", controlParticlesPositions);
                        matt.SetFloatArray("_ControlParticleSize", controlParticlesSizes);
                        matt.SetInt("_AffectorCount", affectorCount);
                        matt.SetFloat("_PSLossyScale", controlParticleSystem.transform.lossyScale.x);
                        matt.SetFloat("_MaskAppearProgress", openCloseProgress);
                    }
                    break;
                case FFstate.MultipleSpheres:
                    foreach (Material matt in materialLayers)
                    {
                        matt.SetVectorArray("_ControlParticlePosition", controlParticlesPositions);
                        matt.SetFloatArray("_ControlParticleSize", controlParticlesSizes);
                        matt.SetInt("_AffectorCount", affectorCount);
                        matt.SetFloat("_PSLossyScale", controlParticleSystem.transform.lossyScale.x);
                        matt.SetFloat("_MaskAppearProgress", openCloseProgress);

                        matt.SetVectorArray("_FFSpherePositions", spherePositions);
                        matt.SetFloatArray("_FFSphereSizes", sphereSizes);
                        matt.SetFloat("_FFSphereCount", numberOfSpheres);
                    }
                    break;
            }            
        }        
    }
}
