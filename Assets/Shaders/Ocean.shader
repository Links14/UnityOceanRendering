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

            // Vertex modifiers
            int _VertexWaveCount;
            float _VertexSeed;
            float _VertexSeedIter;

            float _VertexFrequency;
            float _VertexFrequencyMult;

            float _VertexAmplitude;
            float _VertexAmplitudeMult;

            float _VertexMaxPeak;
            float _VertexPeakOffset;

            float _VertexInitialSpeed;
            float _VertexSpeedRamp;

            float _VertexDrag;
            float _VertexHeight;

            // Fragment modifiers
            float _FragmentSeed;
            float _FragmentSeedIter;
            float _FragmentFrequency;
            float _FragmentFrequencyMult;
            float _FragmentAmplitude;
            float _FragmentAmplitudeMult;
            float _FragmentMaxPeak;
            float _FragmentPeakOffset;
            float _FragmentInitialSpeed;
            float _FragmentSpeedRamp;
            float _FragmentDrag;
            float _FragmentHeight;

            // Lighting
            float _NormalStrength;
            // control sharpness of specular glare // higher values = tighter spread
            float _Shininess;


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
            
            // ---------------------------------------------------------
            // fBm / exponential-sine wave calculation
            //
            // Returns:
            // x = accumulated wave height
            // y = derivative in X
            // z = derivative in Z
            // ---------------------------------------------------------

            float3 vertexFBM(float3 vertexPosition)
            {
                float frequency = _VertexFrequency;
                float amplitude = _VertexAmplitude;
                float speed = _VertexInitialSpeed;
                float seed = _VertexSeed;

                // Sampling position changes as each wave warps
                // the domain used by subsequent waves
                float2 samplePosition = vertexPosition.xz;

                float height = 0.0;
                float2 derivativeSum = float2(0.0, 0.0);
                float amplitudeSum = 0.0;

                for (int i = 0; i < _VertexWaveCount; i++) 
                {
                    // Deterministic direction derived from the seed
                    // The same settings produce the same directions
                    float2 direction = normalize(
                        float2(cos(seed), sin(seed))
                    );

                    // Angular spatial frequency, in radians per unit
                    float phase =
                        dot(direction, samplePosition) * frequency
                        + _Time.y * speed;

                    float sine = sin(phase);
                    float cosine = cos(phase);

                    // Euler's Wave
                    float wave = amplitude
                        * exp(_VertexMaxPeak * sine - _VertexPeakOffset);

                    // Derivative with respect to phase
                    float waveDerivative = _VertexMaxPeak * wave * cosine;

                    // Spatial Derivative: d(height) / d(x, z)
                    float2 waveSlope = frequency * direction * waveDerivative;

                    height += wave;

                    // Accumulate derivatives for the surface normal
                    derivativeSum += waveSlope;

                    // Progressive domain warping:
                    // later waves sample a displaced XZ POSITION
                    samplePosition += -waveSlope * amplitude * _VertexDrag;

                    amplitudeSum += amplitude;

                    // Advance parameters for the next fBm layer
                    frequency *= _VertexFrequencyMult;
                    amplitude *= _VertexAmplitudeMult;
                    speed *= _VertexSpeedRamp;
                    seed += _VertexSeedIter;
                }

                // Avoid division by zero if configured with no waves (edge-case)
                if (amplitudeSum > 0.00001) 
                {
                    height /= amplitudeSum;
                    derivativeSum /= amplitudeSum;
                }

                // Apply overall vertical scale consistently to both
                height *= _VertexHeight;
                derivativeSum *= _VertexHeight;

                return float3(height, derivativeSum.x, derivativeSum.y);
            }


            /// Vertex Shader

            Varyings vert(Attributes input) 
            {
                Varyings output;
                
                // current time in seconds
                float t = _Time.y;

                // Start with vertex position from mesh
                float3 position = input.position.xyz;

                // evaluate fBm
                float3 fBm = vertexFBM(position);

                // Apply vertical displacement
                position.y += fBm.x;

                // For y = height(x, z), the surface normal is
                //  proportional to (-dh/dx, 1, -dh/dz)
                float3 objectNormal = normalize(
                    float3(-fBm.y, 1.0, -fBm.z)
                );

                // position in clip space
                output.position = UnityObjectToClipPos(float4(position, 1.0));
                // position in world space
                output.worldPosition = mul(unity_ObjectToWorld, float4(position, 1.0)).xyz;
                // object normal
                output.normal = UnityObjectToWorldNormal(objectNormal);

                return output;
            }

            /// Fragment Shader

            float4 frag(Varyings input) : SV_Target 
            {
                // normal from vertex shader
                float3 normal = normalize(input.normal);
                // direction from surface toward the light source
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                // point to camera direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPosition);

                float3 halfwayDir = normalize(lightDir + viewDir);

                // Blinn-Phong highlight
                float specularStrength = 0.75;
                float ambientStrength = 0.0f;

                // Specular highlight strength
                float specular = pow(
                    max(0.0, dot(normalize(input.normal), halfwayDir)),
                    _Shininess
                );

                // Lambertian diffuse lighting
                // dot product measures how directly the surface faces the light
                // max() prevents the surface from becoming negatively lit when facing away from the light
                float diffuse = max(0, dot(normal, lightDir));

                // base color
                float3 waterColor = float3(0.03, 0.09, 0.15);

                // Apply lambertian lighting to color
                float3 finalColor = 
                    waterColor * (diffuse + ambientStrength)
                    + specularStrength * specular;

                return float4(finalColor, 1.0);
            }

            // end computer graphics program code
            ENDCG
        }
    }
}