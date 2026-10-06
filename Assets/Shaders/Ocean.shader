// name shader
Shader "Ocean/Basic" 
{
    SubShader 
    {
        // one pass over the geometry
        Pass 
        {
            // contains both fragment and vertex shader
            // start computer graphics program code
            CGPROGRAM

            // vert() is vertex shader
            #pragma vertex vert
            // frag() is fragment shader
            #pragma fragment frag

            // StructureBuffer needs shader model 4.5 or later
            #pragma target 4.5

            // Adds Unity's built in shader helper functions
            #include "UnityCG.cginc"
            
            // AoS wave shape
            // Must match Wave in OceanSettings.cs
            struct Wave
            {
                float amplitude;
                float wavelength;
                float speed;
                float2 direction;
            };

            // GPU buffer containing waves
            StructuredBuffer<Wave> _Waves;

            // Number of waves currently stored in the buffer
            int _WaveCount;


            /// Vertex Input

            // defines all data entering the vertex shader
            struct Attributes 
            {
                float4 position : POSITION;
            };

            /// Vertex Output

            // defines all data leaving the vertex shader entering the fragment shader
            struct Varyings 
            {
                // identifies the position in clip space
                float4 position : SV_POSITION;
                // normal vector
                float3 normal : TEXCOORD0;
            };

            /// Vertex Shader

            Varyings vert(Attributes input) 
            {
                Varyings output;
                
                // current time in seconds
                float t = _Time.y;

                // Start with vertex position from mesh
                float3 position = input.position.xyz;


                /// Wave Calculator

                // Accumulate the contribution from every wave
                float height = 0.0;

                // These represent how steep the surface is in the
                // X and Z directions
                float slopeX = 0.0;
                float slopeZ = 0.0;


                // iterate waves in buffer
                for (int i = 0; i < _WaveCount; i++) 
                {
                    // Get wave from buffer
                    Wave wave = _Waves[i];

                    // normalize dir
                    float2 dir = normalize(wave.direction);

                    // Convert wavelength into wave Number
                    // A shorter wavelength means the waves oscillate more quickly across space
                    float waveNumber = 2.0 * UNITY_PI / wave.wavelength;

                    // Find where this vertex is along the wave's direction.
                    float distance = dot(position.xz, dir);

                    // Calculate current wave phase
                    float phase = waveNumber * distance + wave.speed * t;

                    // calculate sine once
                    float sine = exp(sin(phase));
                    // calculate cosine once
                    float cosine = exp(cos(phase));

                    /// height

                    // Add this wave's vertical displacement
                    // Multiple waves are added togheter
                    height += wave.amplitude * sine;


                    /// surface slope

                    // Calculate how quickly height changes in xz dir
                    // partial derivatives
                    slopeX += wave.amplitude * waveNumber * dir.x * cosine;
                    slopeZ += wave.amplitude * waveNumber * dir.y * cosine;
                }

                // Move the vertex vertically according to all waves.
                position.y += height;

                /// Calculate normal
                // calculate tangents
                float3 tangentX = normalize(float3(1, slopeX, 0));
                float3 tangentZ = normalize(float3(0, slopeZ, 1));

                // normal
                float3 normal = normalize(cross(tangentZ, tangentX));

                // normal to world space
                output.normal = UnityObjectToWorldNormal(normal);
                // position in clip space
                output.position = UnityObjectToClipPos(float4(position, 1.0));

                return output;
            }

            /// Fragment Shader

            float4 frag(Varyings input) : SV_Target 
            {
                // dir from surface toward the light source
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);


                // Lambertian diffuse lighting
                // dot product measures how directly the surface faces the light
                // max() prevents the surface from becoming negatively lit when facing away from the light
                float diffuse = max(0, dot(input.normal, lightDir));

                // base color
                float3 waterColor = float3(0.0, 0.5, 1.0);

                // Apply lambertian lighting to color
                float3 finalColor = waterColor * diffuse;

                return float4(finalColor, 1);
            }

            // end computer graphics program code
            ENDCG
        }
    }
}