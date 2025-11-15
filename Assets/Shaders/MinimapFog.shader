Shader "UI/MinimapFog"
{
    Properties
    {
        [PerRendererData] _MainTex ("Fog Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 採樣迷霧 Texture（使用 alpha 通道作為迷霧強度）
                fixed4 fogColor = tex2D(_MainTex, IN.texcoord);
                
                // 返回黑色迷霧，alpha 由 Texture 控制
                // Alpha = 1 表示完全遮蔽（未探索）
                // Alpha = 0 表示完全透明（已探索）
                fixed4 color = fixed4(0, 0, 0, fogColor.a);
                
                // 應用 UI 顏色和 tint
                color *= IN.color;
                
                return color;
            }
            ENDCG
        }
    }
}

