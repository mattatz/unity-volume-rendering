Shader "VolumeRendering/VolumeRendering"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Volume ("Volume", 3D) = "" {}
		_Intensity("Intensity", Range(1.0, 5.0)) = 1.2
		_Threshold("Threshold", Range(0.0, 1.0)) = 0.95
		_SliceMin("Slice min", Vector) = (0.0, 0.0, 0.0, -1.0)
		_SliceMax("Slice max", Vector) = (1.0, 1.0, 1.0, -1.0)
	}

	CGINCLUDE

	half4 _Color;
	sampler3D _Volume;
	half _Intensity, _Threshold;
	half3 _SliceMin, _SliceMax;

	float3 localize(float3 p) {
		return mul(unity_WorldToObject, float4(p, 1)).xyz;
	}

	float3 get_uv(float3 p) {
		float3 local = localize(p);
		return (local + 0.5);
	}

	float sample_volume(float3 uv) {
		float v = tex3D(_Volume, uv).r * _Intensity;
		float min = step(_SliceMin.x, uv.x) * step(_SliceMin.y, uv.y) * step(_SliceMin.z, uv.z);
		float max = step(uv.x, _SliceMax.x) * step(uv.y, _SliceMax.y) * step(uv.z, _SliceMax.z);
		return v * min * max;
	}

	bool outside(float3 uv) {
		const float EPSILON = 0.01;
		float lower = -EPSILON;
		float upper = 1 + EPSILON;
		return (
			uv.x < lower || uv.y < lower || uv.z < lower ||
			uv.x > upper || uv.y > upper || uv.z > upper
		);
	}

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 world : TEXCOORD1;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	ENDCG

	SubShader {
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		// ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define COUNT 64

			fixed4 frag (v2f i) : SV_Target
			{
				float3 ro = i.world;
				float3 rd = normalize(i.world - _WorldSpaceCameraPos);

				float t = 0.01;
				float dt = 0.001;
				float4 dst = float4(0, 0, 0, 0);

				float3 uv = get_uv(ro);

				[loop]
				while (!outside(uv) && dst.r <= _Threshold) {
					uv = get_uv(ro);
					float v = sample_volume(uv);
					float4 src = float4(v, v, v, v);
					src.a *= 0.5;
					src.rgb *= src.a;

					// blend
					dst = (1.0 - dst.a) * src + dst;
					// dt = t * 0.01;
					t += dt;
					ro += rd * t;
				}

				dst.a *= dst.r;

				return saturate(dst) * _Color;
			}

			ENDCG
		}
	}
}
