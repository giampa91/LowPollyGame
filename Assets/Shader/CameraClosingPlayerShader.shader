Shader "Custom/CameraClosingPlayerShader" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Spec Color", Color) = (1,1,1,0)
        _Emission ("Emissive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.1, 1)) = 0.7
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    
    SubShader {
        Tags {"IgnoreProjector"="True" "RenderType"="Geometry" "Queue"="Transparent" }
        // Render into depth buffer only
            Pass {
            ColorMask 0
        }
        
        CGPROGRAM
        #pragma surface surf Lambert alpha exclude_path:prepass
    
        sampler2D _MainTex;
        fixed4 _Color;
    
        struct Input {
            float2 uv_MainTex;
        };
    
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        // Render normally
        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            Lighting On
            SetTexture [_MainTex] {
                Combine texture * primary DOUBLE, texture * primary
            }
        }
    }
    Fallback "Standard"
}
