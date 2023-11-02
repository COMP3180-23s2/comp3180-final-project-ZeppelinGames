Shader "Custom/VertexColor"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 lightDir = UnityWorldSpaceLightDir(i.vertex);
                half3 normal = UnityObjectToWorldNormal(i.vertex.xyz);
                half mag = sqrt(dot(lightDir, normal));
                normal /= mag;  // Manually normalize the normal

                half diff = max(0, dot(normal, lightDir));

                half4 col = i.color;
                col.rgb *= diff;
                return col;
            }
            ENDCG
        }
    }
}