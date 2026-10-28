Shader "Varginha/MenuLogoGrade"
{
    Properties
    {
        _MainTex ("Logo", 2D) = "white" {}
        _Saturation ("Saturation", Range(0,1)) = 0.22
        _Tint ("Night tint", Color) = (0.76,0.84,0.84,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Saturation;
            float4 _Tint;
            fixed4 frag(v2f_img input) : SV_Target
            {
                fixed4 source = tex2D(_MainTex, input.uv);
                float gray = dot(source.rgb, float3(0.2126,0.7152,0.0722));
                return fixed4(lerp(gray.xxx, source.rgb, _Saturation) * _Tint.rgb, source.a);
            }
            ENDCG
        }
    }
}
