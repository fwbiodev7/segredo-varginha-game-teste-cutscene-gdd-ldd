Shader "Hidden/Varginha/CinematicPixelResolve"
{
    Properties { _MainTex("Source",2D)="white"{} }
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
            fixed4 frag(v2f_img i):SV_Target
            {
                fixed4 c=tex2D(_MainTex,i.uv);
                // Preserve the generated material colours. Quantising gamma/linear
                // values here changed the skin tone and crushed dark upholstery.
                c.a=step(.5,c.a);
                return c;
            }
            ENDCG
        }
    }
}
