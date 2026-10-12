using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class OceanSettings : MonoBehaviour
{
    // Struct descibes one wave
    // must match Wave in HLSL shader
    [StructLayout(LayoutKind.Sequential)]
    public struct Wave
    {
        public float amplitude;
        public float frequency;
        public float speed;
        public Vector2 direction;
    }

    [Header("Signals")]
    [SerializeField]
    private Signal listen_RegenerateWaves;

    [Header("Reference Parameters")]
    // The MeshRenderer whose material will recieve the wave buffer
    [SerializeField]
    private MeshRenderer meshRenderer;

    [Space]

    //[Header("Wave Settings")]
    //[SerializeField, Min(0)]
    //private int waveCount = 6;
    //[SerializeField, Min(0.01f)]
    //private float baseAmplitude = 0.5f;

    //// Spacial Frequency in cycles per world Unit
    //// 0.04 means a wavelength of 25 world units.
    //[SerializeField, Min(0.001f)]
    //private float baseFrequency = 0.04f;
    //[SerializeField, Min(1f)]
    //private float frequencyMultiplier = 1.16f;
    //[SerializeField, Range(0.1f, 1f)]
    //private float amplitudeMultiplier = 0.83f;

    //// 1 is the baseline amplitude-to-wavelength relationship
    //[SerializeField, Range(0.1f, 5f)]
    //private float amplitudeRatio = 1f;

    //[SerializeField]
    //private float baseSpeed = 1f;
    //[SerializeField]
    //private float speedMultiplier = 1.07f;

    //[Header("Visual Settings")]
    //[SerializeField, Min(4f)]
    //private float shininess = 64f;
    //[SerializeField, Range(0f, 1f)]
    //private float warpStrength = 0.25f;

    //[Header("Generation Parameters")]
    //[Range(10000, 99999)]
    //[SerializeField]
    //private int seed = 12345;

    [Header("Vertex Settings")]
    [SerializeField, Range(1, 32)]
    private int vertexWaveCount = 8;
    [SerializeField]
    private float vertexSeed = 4f;
    [SerializeField]
    private float vertexSeedIterator = 4.3f;
    [Space]
    [SerializeField]
    private float vertexFrequency = 1f;
    [SerializeField]
    private float vertexFrequencyMultiplier = 1.16f;
    [Space]
    [SerializeField]
    private float vertexAmplitude = 1f;
    [SerializeField]
    private float vertexAmplitudeMultiplier = 0.83f;
    [Space]
    [SerializeField]
    private float vertexMaxPeak = 1f;
    [SerializeField]
    private float vertexPeakOffset = 1.14f;
    [Space]
    [SerializeField]
    private float vertexInitialSpeed = 2f;
    [SerializeField]
    private float vertexSpeedRamp = 1.07f;
    [SerializeField, Range(0f, 2f)]
    private float vertexDrag = 0.5f;
    [SerializeField]
    private float vertexHeight = 1.48f;
    [Space]

    [Header("Fragment Settings")]
    [SerializeField]
    private float fragmentSeed = 4f;
    [SerializeField]
    private float fragmentSeedIterator = 4.3f;
    [Space]
    [SerializeField]
    private float fragmentFrequency = 1f;
    [SerializeField]
    private float fragmentFrequencyMultiplier = 1.16f;
    [Space]
    [SerializeField]
    private float fragmentAmplitude = 1f;
    [SerializeField]
    private float fragmentAmplitudeMultiplier = 0.83f;
    [Space]
    [SerializeField]
    private float fragmentMaxPeak = 1f;
    [SerializeField]
    private float fragmentPeakOffset = 1.14f;
    [Space]
    [SerializeField]
    private float fragmentInitialSpeed = 2f;
    [SerializeField]
    private float fragmentSpeedRamp = 1.07f;
    [SerializeField, Range(0f, 2f)]
    private float fragmentDrag = 0.5f;
    [SerializeField]
    private float fragmentHeight = 1.34f;

    [Header("Lighting")]
    [SerializeField, Range(0f, 4f)]
    private float normalStrength = 1f;
    [SerializeField, Min(4f)]
    private float shininess = 64f;


    private Material oceanMaterial;

    //// GPU buffer containg our wave structs
    //private ComputeBuffer waveBuffer;


    private void OnEnable()
    {
        if (listen_RegenerateWaves == null)
            throw new MissingReferenceException(
                $"{nameof(listen_RegenerateWaves)} is not assigned on {name}.");
        if (meshRenderer == null)
            throw new MissingReferenceException(
                $"{nameof(meshRenderer)} is not assigned on {name}.");

        oceanMaterial = meshRenderer.material;

        listen_RegenerateWaves.Register(StartSimulation);
    }

    private void OnDisable()
    {
        listen_RegenerateWaves.UnRegister(StartSimulation);
    }



    private void Start()
    {
        StartSimulation();
    }

    private void StartSimulation()
    {
        ApplySettings();

        //    Wave[] waves = GenerateWaves();

        //    // find size of wave in bytes of memory
        //    int waveStride = Marshal.SizeOf<Wave>();

        //    // Create a GPU buffer large enough to hold every wave
        //    waveBuffer = new(waveCount, waveStride);

        //    // Copy the wave data from the CPU into the GPU buffer
        //    waveBuffer.SetData(waves);

        //    // Give the buffer to the material
        //    oceanMaterial.SetBuffer("_Waves", waveBuffer);

        //    // Tell the shader how many waves are in the buffer
        //    oceanMaterial.SetInt("_WaveCount", waves.Length);

        //    // set the specular highlighting sharpness
        //    oceanMaterial.SetFloat("_Shininess", shininess);

        //    // set the domain warping ration - effect of previous wave movement on new waves
        //    oceanMaterial.SetFloat("_WarpStrength", warpStrength);
    }

    private void ApplySettings()
    {
        // Vertex displacement parameters.
        oceanMaterial.SetInt("_VertexWaveCount", vertexWaveCount); // number of waves used in simulation
        oceanMaterial.SetFloat("_VertexSeed", vertexSeed); // randomized seed
        oceanMaterial.SetFloat("_VertexSeedIter", vertexSeedIterator);
        oceanMaterial.SetFloat("_VertexFrequency", vertexFrequency);
        oceanMaterial.SetFloat("_VertexFrequencyMult", vertexFrequencyMultiplier);
        oceanMaterial.SetFloat("_VertexAmplitude", vertexAmplitude);
        oceanMaterial.SetFloat("_VertexAmplitudeMult", vertexAmplitudeMultiplier);
        oceanMaterial.SetFloat("_VertexMaxPeak", vertexMaxPeak);
        oceanMaterial.SetFloat("_VertexPeakOffset", vertexPeakOffset);
        oceanMaterial.SetFloat("_VertexInitialSpeed", vertexInitialSpeed);
        oceanMaterial.SetFloat("_VertexSpeedRamp", vertexSpeedRamp);
        oceanMaterial.SetFloat("_VertexDrag", vertexDrag);
        oceanMaterial.SetFloat("_VertexHeight", vertexHeight);

        // Fragment-stage wave and normal parameters.
        oceanMaterial.SetFloat("_FragmentSeed", fragmentSeed); // randomized seed
        oceanMaterial.SetFloat("_FragmentSeedIter", fragmentSeedIterator);
        oceanMaterial.SetFloat("_FragmentFrequency", fragmentFrequency);
        oceanMaterial.SetFloat("_FragmentFrequencyMult", fragmentFrequencyMultiplier);
        oceanMaterial.SetFloat("_FragmentAmplitude", fragmentAmplitude);
        oceanMaterial.SetFloat("_FragmentAmplitudeMult", fragmentAmplitudeMultiplier);
        oceanMaterial.SetFloat("_FragmentMaxPeak", fragmentMaxPeak);
        oceanMaterial.SetFloat("_FragmentPeakOffset", fragmentPeakOffset);
        oceanMaterial.SetFloat("_FragmentInitialSpeed", fragmentInitialSpeed);
        oceanMaterial.SetFloat("_FragmentSpeedRamp", fragmentSpeedRamp);
        oceanMaterial.SetFloat("_FragmentDrag", fragmentDrag);
        oceanMaterial.SetFloat("_FragmentHeight", fragmentHeight);

        // Lighting
        oceanMaterial.SetFloat("_NormalStrength", normalStrength);
        oceanMaterial.SetFloat("_Shininess", shininess);
    }


    //private Wave[] GenerateWaves()
    //{
        //// create seeded random num
        //System.Random random = new(seed);

        //// array of our waves
        //Wave[] waves = new Wave[waveCount];

        //// Generate waves independently
        //for (int i = 0; i < waveCount; i++)
        //{
        //    // Each iteration adds a higher-frequency layer
        //    float freq = baseFrequency * Mathf.Pow(frequencyMultiplier, i);

        //    // Keep amplitude proportional to wavelength
        //    // Apply fBm amplitude falloff
        //    float amp = baseAmplitude
        //        * (baseFrequency / freq)
        //        * Mathf.Pow(amplitudeMultiplier, i)
        //        * amplitudeRatio;

        //    float spd = baseSpeed * Mathf.Pow(speedMultiplier, i);

        //    // random angle around the circle
        //    float angle = (float)random.NextDouble()
        //        * Mathf.PI * 2f;

        //    waves[i] = new()
        //    {
        //        amplitude = amp,
        //        frequency = freq,
        //        speed = spd,
        //        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))
        //    };
        //}

        //return waves;
    //}

    public void RandomizeSeed()
    {
        fragmentSeed = UnityEngine.Random.Range(1f, 100f);
        vertexSeed = fragmentSeed;
        //seed = UnityEngine.Random.Range(10000, 100000);
    }

    //private void OnDestroy()
    //{
    //    // Release the GPU bugger when this object is destroyed
    //    waveBuffer?.Release();
    //}
}
