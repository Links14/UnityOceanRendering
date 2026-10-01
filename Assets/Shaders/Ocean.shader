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

            #include "UnityCG.cginc"

            // defines all data entering the vertex shader
            struct Attributes 
            {
                float4 position : POSITION;
            };

            // defines all data leaving the vertex shader entering the fragment shader
            struct Varyings 
            {
                // identifies the position in clip space
                float4 position : SV_POSITION;
            };

            Varyings vert(Attributes input) 
            {
                Varyings output;
                float t = _Time.y;
                float frequency = .33;
                float amplitude = 2;
                float speed = 3;

                float3 position = input.position.xyz;
                float wave = amplitude * (sin(frequency * position.x + t * speed) + sin(frequency * position.z + t * speed));
                position.y += wave;
                
                output.position = UnityObjectToClipPos(float4(position, 1.0));

                return output;
            }

            float4 frag(Varyings input) : SV_Target 
            {
                return float4(0.0, 0.5, 1, 1);
            }

            // end computer graphics program code
            ENDCG
        }
    }
}