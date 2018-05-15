Shader "GUI/b"
{
        Properties
        {
                _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        }
       
        SubShader
        {
                Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
                ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
                LOD 110
               
                Pass
                {
                        CGPROGRAM
                        #pragma vertex vert_vct
                        #pragma fragment frag_mult
                        #pragma fragmentoption ARB_precision_hint_fastest
                        #include "UnityCG.cginc"
 
                        sampler2D _MainTex;
                        float4 _MainTex_ST;
 
                        struct vin_vct
                        {
                                float4 vertex : POSITION;
                                float4 color : COLOR;
                                float2 texcoord : TEXCOORD0;
                        };
 
                        struct v2f_vct
                        {
                                float4 vertex : POSITION;
                                fixed4 color : COLOR;
                                half2 texcoord : TEXCOORD0;
                        };
 
                        v2f_vct vert_vct(vin_vct input)
                        {
                v2f_vct output;
 
                output.vertex = mul(UNITY_MATRIX_P,
                    mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
                    + float4(input.vertex.x, input.vertex.y, 0.0, 0.0));
                   
                output.color = input.color;
                output.texcoord = input.texcoord;
 
                return output;
                        }
 
                        fixed4 frag_mult(v2f_vct i) : COLOR
                        {
                                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                                return col;
                        }
                       
                        ENDCG
                }
        }
 
        SubShader
        {
                LOD 100
 
                BindChannels
                {
                        Bind "Vertex", vertex
                        Bind "TexCoord", texcoord
                        Bind "Color", color
                }
 
                Pass
                {
                        Lighting Off
                        SetTexture [_MainTex] { combine texture * primary }
                }
        }
}