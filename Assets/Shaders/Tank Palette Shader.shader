Shader "Unlit/Palette Shader"
{
    Properties // Input as defined on a material
    {
        depalettized_texture ("Depalettized Texture", 2D) = "white" {}
        palette ("Palette", 2D) = "white" {}
        color_count ("Color Count", float) = 0
    }
    SubShader // A single shader in this shader file; yes, there can be more
    {
        Tags {
            "RenderType"="Opaque" // Categorizes this shader somehow
            "Queue"="Transparent" // Order in rendering queue
        }
        Blend SrcAlpha OneMinusSrcAlpha // Makes input alpha be transparent, default is black for some reason

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            const sampler2D depalettized_texture;
            const sampler2D palette;
            float color_count;
            float4 depalettized_texture_ST;

            struct vertex_data
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragment_data
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fragment_data vert (const vertex_data v)
            {
                fragment_data o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, depalettized_texture);
                return o;
            }

            float4 frag (const fragment_data f) : SV_Target
            {
                const float color_index = tex2D(depalettized_texture, f.uv).a * 255 / color_count;
                return tex2D(palette, float2(color_index, 0));
            }
            ENDCG
        }
    }
}
