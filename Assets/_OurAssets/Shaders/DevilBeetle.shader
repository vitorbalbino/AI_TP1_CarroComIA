Shader "Custom/DevilBeetle"
{
    Properties
    {
        _FlagColor ("Flag Color", Color) = (1,1,1,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Smoothness ("Smoothness", 2D) = "grey" {}
        _Metallic ("Metallic", 2D) = "black" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Alpha ("Alpha", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _Diffuse;
        sampler2D _Smoothness;
        sampler2D _Metallic;
        sampler2D _Normal;
        sampler2D _Alpha;
        
         fixed4 _FlagColor;

        struct Input
        {
            float2 uv_Diffuse;
            float2 uv_Smoothness;
            float2 uv_Metallic;
            float2 uv_Normal;
            float2 uv_Alpha;
            
            float4 color : COLOR;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 Diffuse = tex2D(_Diffuse, IN.uv_Diffuse) * (_FlagColor + IN.color);
            fixed4 Metallic = tex2D(_Metallic, IN.uv_Metallic);
            fixed4 Smoothness = tex2D(_Smoothness, IN.uv_Smoothness);
            fixed4 Normal = tex2D(_Normal, IN.uv_Normal);
            fixed4 Alpha = tex2D(_Alpha, IN.uv_Alpha);
            
            o.Albedo = Diffuse;
            o.Metallic = Metallic;
            o.Smoothness = Smoothness;
            o.Normal = UnpackNormal(Normal);
            o.Alpha = Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
