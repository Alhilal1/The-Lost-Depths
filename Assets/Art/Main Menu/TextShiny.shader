Shader "Custom/TextShinyShader"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _ShinyColor ("Shiny Color", Color) = (1, 1, 1, 1)
        _BaseColor ("Base Color", Color) = (0.7, 0.7, 0.7, 1) // B2B2B2 Color
        _ShineSpeed ("Shine Speed", Float) = 1.0 // Speed of the shiny effect
        _ShineIntensity ("Shine Intensity", Float) = 0.5 // How intense the shine is
        _ShineDirection ("Shine Direction", Vector) = (1, -1, 0, 0) // 45-degree direction
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // Properties
            sampler2D _MainTex;
            float4 _ShinyColor;
            float4 _BaseColor;
            float _ShineSpeed;
            float _ShineIntensity;
            float4 _ShineDirection; // Direction of the light
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                // Get the base color of the font
                float4 baseColor = _BaseColor;
                
                // Time-based "shine" effect
                float time = _Time.y * _ShineSpeed;
                float shine = sin(time + dot(i.normal, _ShineDirection.xyz)) * 0.5 + 0.5;
                
                // Lerp between base color and shiny color
                float4 finalColor = lerp(baseColor, _ShinyColor, shine * _ShineIntensity);
                
                // Sample the texture and apply the final color
                float4 texColor = tex2D(_MainTex, i.uv);
                finalColor *= texColor;
                
                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
