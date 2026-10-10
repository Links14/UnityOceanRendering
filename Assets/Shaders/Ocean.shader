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
                float frequency;
                float speed;
                float2 direction;
            };

            // GPU buffer containing waves
            StructuredBuffer<Wave> _Waves;

            // Number of waves currently stored in the buffer
            int _WaveCount;

            // controls the sharpness of the specular highlights
            float _Shininess;
            
            // Control how strongerly earlier waves distort later waves;
            float _WarpStrength = 0.25f; // domain warping

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
                float3 worldPosition : TEXCOORD1;
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

                // Position where wave functions are evalutated
                // Initially starts as the vertex's original XZ position
                float2 samplePosition = position.xz;

                // iterate waves in buffer
                for (int i = 0; i < _WaveCount; i++) 
                {
                    // Get wave from buffer
                    Wave wave = _Waves[i];

                    // normalize dir
                    float2 dir = normalize(wave.direction);

                    // Convert frequency into wave Number
                    // A higher frequency means the waves oscillate more quickly across space
                    float waveNumber = 2.0 * UNITY_PI * wave.frequency;

                    // Find where this vertex is along the wave's direction.
                    float distance = dot(samplePosition, dir);

                    // Calculate current wave phase
                    float phase = waveNumber * distance + wave.speed * t;

                    // calculate sine once
                    float sine = sin(phase);
                    // calculate cosine once
                    float cosine = cos(phase);
                    // eular wave
                    float expSine = exp(sine);

                    /// height

                    // Add this wave's vertical displacement
                    // Multiple waves are added togther
                    height += wave.amplitude * (expSine - 1.266);
                    // subtract half of e height as an offset
                    // center waves around 0


                    // general derivative
                    float derivative = wave.amplitude * expSine * cosine;

                    /// surface slope

                    // Calculate how quickly height changes in xz dir
                    // partial derivatives
                    slopeX += derivative * waveNumber * dir.x;
                    slopeZ += derivative * waveNumber * dir.y;

                    // Use the accumulated slope to warp the sampling position for subsequent waves.
                    samplePosition += _WarpStrength * float2(slopeX, slopeZ);
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
                // world position in world space
                output.worldPosition = mul(unity_ObjectToWorld, float4(position, 1.0)).xyz;

                return output;
            }

            /// Fragment Shader

            float4 frag(Varyings input) : SV_Target 
            {
                // dir from surface toward the light source
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // point to camera direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPosition);


                // Blinn-Phong highlight
                float specularStrength = 0.75;
                float ambientStrength = 0.0f;

                // Halfway dir between lightDir and viewDir
                float3 halfwayDir = normalize(lightDir + viewDir);

                // Specular highlight strength
                float specular = pow(
                    max(0.0, dot(normalize(input.normal), halfwayDir)),
                    _Shininess
                );


                // Lambertian diffuse lighting
                // dot product measures how directly the surface faces the light
                // max() prevents the surface from becoming negatively lit when facing away from the light
                float diffuse = max(0, dot(input.normal, lightDir));

                // base color
                float3 waterColor = float3(0.03, 0.09, 0.15);

                // Apply lambertian lighting to color
                float3 finalColor = 
                    waterColor * (diffuse + ambientStrength) +
                    specularStrength * specular;

                return float4(finalColor, 1);
            }

            // end computer graphics program code
            ENDCG
        }
    }
}