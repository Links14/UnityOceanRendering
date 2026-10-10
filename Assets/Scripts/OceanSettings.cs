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

    [Header("Reference Parameters")]
    // The MeshRenderer whose material will recieve the wave buffer
    [SerializeField]
    private MeshRenderer meshRenderer;

    [Header("Wave Settings")]
    [SerializeField, Min(0)]
    private int waveCount = 6;
    [SerializeField, Min(0.01f)]
    private float baseAmplitude = 0.5f;

    // Spacial Frequency in cycles per world Unit
    // 0.04 means a wavelength of 25 world units.
    [SerializeField, Min(0.001f)]
    private float baseFrequency = 0.04f;
    [SerializeField, Min(1f)]
    private float frequencyMultiplier = 1.16f;
    [SerializeField, Range(0.1f, 1f)]
    private float amplitudeMultiplier = 0.83f;

    // 1 is the baseline amplitude-to-wavelength relationship
    [SerializeField, Range(0.1f, 5f)]
    private float amplitudeRatio = 1f;

    [SerializeField]
    private float baseSpeed = 1f;
    
    [Header("Visual Settings")]
    [SerializeField, Min(4f)]
    private float shininess = 64f;
    [SerializeField, Range(0f, 1f)]
    private float warpStrength = 0.25f;

    [Header("Generation Parameters")]
    [Range(10000, 99999)]
    [SerializeField]
    private int seed = 12345;


    


    // GPU buffer containg our wave structs
    private ComputeBuffer waveBuffer;

    private void Start()
    {
        Wave[] waves = GenerateWaves();

        // find size of wave in bytes of memory
        int waveStride = Marshal.SizeOf<Wave>();

        // Create a GPU buffer large enough to hold every wave
        waveBuffer = new(waveCount, waveStride);

        // Copy the wave data from the CPU into the GPU buffer
        waveBuffer.SetData(waves);

        // Give the buffer to the material
        meshRenderer.material.SetBuffer("_Waves", waveBuffer);

        // Tell the shader how many waves are in the buffer
        meshRenderer.material.SetInt("_WaveCount", waves.Length);

        // set the specular highlighting sharpness
        meshRenderer.material.SetFloat("_Shininess", shininess);

        // set the domain warping ration - effect of previous wave movement on new waves
        meshRenderer.material.SetFloat("_WarpStrength", warpStrength);
    }


    private Wave[] GenerateWaves()
    {
        // create seeded random num
        System.Random random = new(seed);

        // array of our waves
        Wave[] waves = new Wave[waveCount];

        // Generate waves independently
        for (int i = 0; i < waveCount; i++)
        {
            // Each iteration adds a higher-frequency layer
            float freq = baseFrequency * Mathf.Pow(frequencyMultiplier, i);

            // Keep amplitude proportional to wavelength
            // Apply fBm amplitude falloff
            float amp = baseAmplitude
                * (baseFrequency / freq)
                * Mathf.Pow(amplitudeMultiplier, i)
                * amplitudeRatio;

            // random angle around the circle
            float angle = (float)random.NextDouble()
                * Mathf.PI * 2f;

            waves[i] = new()
            {
                amplitude = amp,
                frequency = freq,
                speed = baseSpeed,
                direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))
            };
        }

        return waves;
    }

    private float RandomRange(System.Random random, float min, float max)
    {
        // Next value between 0 and 1
        float normalized = (float)random.NextDouble();

        // remap [0, 1) to [min, max)
        return Mathf.Lerp(min, max, normalized);
    }

    public void RandomizeSeed()
    {
        seed = Random.Range(10000, 100000);
    }

    private void OnDestroy()
    {
        // Release the GPU bugger when this object is destroyed
        waveBuffer?.Release();
    }
}
