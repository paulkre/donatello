Shader "VRSculpting/FingerSphere" {
	Properties{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Alpha("Alpha", Float) = 0.8
	}

	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 viewDir;
		};

		fixed4 _Color;
		float _Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Emission = _Color;
			o.Alpha = _Alpha * pow(dot(normalize(IN.viewDir), o.Normal), 2);
		}
		ENDCG
	}

	FallBack "Diffuse"
}
