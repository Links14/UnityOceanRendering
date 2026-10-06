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
        public float wavelength;
        public float speed;
        public Vector2 direction;
    }

    [Header("Reference Parameters")]
    // The MeshRenderer whose material will recieve the wave buffer
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Generation Parameters")]
    [SerializeField] private int waveCount = 0;
    [SerializeField] private int seed = 12345;
    [SerializeField] private Vector2 amplitudeRange;
    [SerializeField] private Vector2 wavelengthRange;
    [SerializeField] private Vector2 speedRange;


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
            // random angle around the circle
            float angle = RandomRange(random, 0f, Mathf.PI * 2f);

            waves[i] = new()
            {
                amplitude = RandomRange(random, amplitudeRange.x, amplitudeRange.y),
                wavelength = RandomRange(random, wavelengthRange.x, wavelengthRange.y),
                speed = RandomRange(random, speedRange.x, speedRange.y),

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

    private void OnDestroy()
    {
        // Release the GPU bugger when this object is destroyed
        waveBuffer?.Release();
    }
}
