Shader "VRSculpting/SculptSurface" {
	Properties{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_SelectColor("Selection Color", Color) = (0.0, 1.0, 0.0, 1.0)
		_SelectColorMirrored("Selection Color Mirrored", Color) = (1.0, 0.0, 0.0, 1.0)
		_Glossiness("Smoothness", Range(0,1)) = 0.5

		_BrushPos("Brush Position", Vector) = (1.0, 1.0, 1.0, 1.0)
		_BrushPosMirrored("Brush Position Mirrored", Vector) = (1.0, 1.0, 1.0, 1.0)
		_BrushRadius("Brush Radius", Float) = 0.25
		_BrushHardness("Brush Hardness", Float) = 2.0

		[MaterialToggle] _MenuEnabled("Menu Enabled", Float) = 0.0
		[MaterialToggle] _SymmetryEnabled("Symmetry Enabled", Float) = 0.0
	}

	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		fixed4 _Color;
		fixed4 _SelectColor;
		fixed4 _SelectColorMirrored;

		fixed4 _BrushPos;
		fixed4 _BrushPosMirrored;
		float _BrushRadius;
		float _BrushHardness;

		float _MenuEnabled;
		float _SymmetryEnabled;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float smoothStep(float x) {
			return pow(x * x * (3 - 2 * x), _BrushHardness);
		}

		float amp(float3 worldPos, fixed4 brushPos) {
			float d = distance(worldPos, brushPos);
			return d < _BrushRadius ? 0.8 * max(0.0, 1.0 - smoothStep(d / _BrushRadius)) : 0.0;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			if (_MenuEnabled < 1.0) {
				o.Emission = amp(IN.worldPos, _BrushPos) * _SelectColor;
				o.Albedo = _Color;
			}
			else {
				o.Albedo = 0.5 * _Color;
			}

			if (_SymmetryEnabled == 1.0) {
				o.Emission += amp(IN.worldPos, _BrushPosMirrored) * _SelectColorMirrored;
			}

			o.Metallic = 0.0;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
