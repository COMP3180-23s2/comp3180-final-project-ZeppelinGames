Shader "Custom/SurfaceNormal"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 normalColor;
        };

        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.normalColor = abs(v.normal);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {            
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Albedo *= IN.normalColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
