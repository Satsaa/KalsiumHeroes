Shader "UI/Colorize"
{
	Properties
	{
		[NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
		
		[KeywordEnum(Linear, RGB)]COLORSPACE("ColorSpace", Float) = 0

		[Toggle(USEOVERLAP)]USEOVERLAP("UseOverlap", Float) = 0
		Overlap("Overlap", Range(0, 1)) = 0.25
		OverlapStart("OverlapStart", Range(0, 1)) = 0.5
		OverlapColor("OverlapColor", Color) = (0, 1, 0.1192062, 1)

		[HideInInspector] _Stencil("Stencil ID", Float) = 0
		[HideInInspector] _StencilComp("StencilComp", Float) = 8
		[HideInInspector] _StencilOp("StencilOp", Float) = 0
		[HideInInspector] _StencilReadMask("StencilReadMask", Float) = 255
		[HideInInspector] _StencilWriteMask("StencilWriteMask", Float) = 255
		[HideInInspector] _ColorMask("ColorMask", Float) = 15
	}
	SubShader
	{
		Tags
		{
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Transparent"
			"UniversalMaterialType" = "Unlit"
			"Queue"="Transparent"
		}
		Pass
		{
			Name "Sprite Unlit"
			Tags
			{
				"LightMode" = "Universal2D"
			}

			// Render State
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask]

			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}

			HLSLPROGRAM

			// Pragmas
			#pragma target 2.0
			#pragma exclude_renderers d3d11_9x
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			#pragma shader_feature_local COLORSPACE_LINEAR COLORSPACE_RGB
			#pragma shader_feature_local _ USEOVERLAP

			#if defined(COLORSPACE_LINEAR) && defined(USEOVERLAP)
				#define KEYWORD_PERMUTATION_0
			#elif defined(COLORSPACE_LINEAR)
				#define KEYWORD_PERMUTATION_1
			#elif defined(COLORSPACE_RGB) && defined(USEOVERLAP)
				#define KEYWORD_PERMUTATION_2
			#else
				#define KEYWORD_PERMUTATION_3
			#endif

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _ALPHAPREMULTIPLY_ON 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define ATTRIBUTES_NEED_COLOR
			#define VARYINGS_NEED_TEXCOORD0
			#define VARYINGS_NEED_COLOR
			#define FEATURES_GRAPH_VERTEX
			/* WARNING: $splice Could not find named fragment 'PassInstancing' */
			#define SHADERPASS SHADERPASS_SPRITEUNLIT
			/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

			// Includes
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 positionOS : POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 normalOS : NORMAL;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 tangentOS : TANGENT;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 uv0 : TEXCOORD0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 color : COLOR;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				#endif
			};
			struct Varyings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 positionCS : SV_POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 texCoord0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 color;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 uv0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 VertexColor;
				#endif
			};
			struct VertexDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpaceNormal;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpaceTangent;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpacePosition;
				#endif
			};
			struct PackedVaryings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 positionCS : SV_POSITION;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 interp0 : INTERP0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 interp1 : INTERP1;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
				PackedVaryings PackVaryings (Varyings input)
				{
					PackedVaryings output;
					output.positionCS = input.positionCS;
					output.interp0.xyzw =  input.texCoord0;
					output.interp1.xyzw =  input.color;
					#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
					#endif
					return output;
				}
				Varyings UnpackVaryings (PackedVaryings input)
				{
					Varyings output;
					output.positionCS = input.positionCS;
					output.texCoord0 = input.interp0.xyzw;
					output.color = input.interp1.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
					#endif
					return output;
				}
			#endif

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			half Overlap;
			half OverlapStart;
			half4 OverlapColor;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions

			void Unity_ColorspaceConversion_Linear_HSV_half(half3 In, out half3 Out)
			{
				half3 sRGBLo = In * 12.92;
				half3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), half3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
				half3 Linear = half3(In <= 0.0031308) ? sRGBLo : sRGBHi;
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(Linear.bg, K.wz), half4(Linear.gb, K.xy), step(Linear.b, Linear.g));
				half4 Q = lerp(half4(P.xyw, Linear.r), half4(Linear.r, P.yzx), step(P.x, Linear.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_ColorspaceConversion_RGB_HSV_half(half3 In, out half3 Out)
			{
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_Subtract_half(half A, half B, out half Out)
			{
				Out = A - B;
			}

			void Unity_Add_half(half A, half B, out half Out)
			{
				Out = A + B;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
			{
				Out = A * B;
			}

			void Unity_Lerp_half(half A, half B, half T, out half Out)
			{
				Out = lerp(A, B, T);
			}

			void Unity_ColorspaceConversion_HSV_Linear_half(half3 In, out half3 Out)
			{
				half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
				half3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
				half3 linearRGBLo = RGB / 12.92;
				half3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), half3(2.4, 2.4, 2.4));
				Out = half3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
			}

			void Unity_ColorspaceConversion_HSV_RGB_half(half3 In, out half3 Out)
			{
				half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
				Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
			}

			void Unity_Preview_half(half In, out half Out)
			{
				Out = In;
			}

			void Unity_Modulo_half(half A, half B, out half Out)
			{
				Out = fmod(A, B);
			}

			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Minimum_half(half A, half B, out half Out)
			{
				Out = min(A, B);
			};

			void Unity_Remap_half(half In, half2 InMinMax, half2 OutMinMax, out half Out)
			{
				Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

			void Unity_Clamp_half(half In, half Min, half Max, out half Out)
			{
				Out = clamp(In, Min, Max);
			}

			void Unity_Lerp_half3(half3 A, half3 B, half3 T, out half3 Out)
			{
				Out = lerp(A, B, T);
			}


			// Custom interpolators pre vertex
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

			// Graph Vertex
			struct VertexDescription
			{
				half3 Position;
				half3 Normal;
				half3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Custom interpolators, pre surface
			Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
			{
				return output;
			}
			#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC

			// Graph Pixel
			struct SurfaceDescription
			{
				half3 BaseColor;
				half Alpha;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, IN.uv0.xy);
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
					#else
						half3 _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0 = _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_85dff094aeb8431bb83b308f04d92c8a_R_1 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[0];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_G_2 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[1];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_B_3 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[2];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0 = OverlapColor;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((_Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0.xyz), _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((_Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0.xyz), _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0 = _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1;
					#else
						half3 _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0 = _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_70a657084c0147aa95ae203c92fae9d8_R_1 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[0];
					half _Split_70a657084c0147aa95ae203c92fae9d8_G_2 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[1];
					half _Split_70a657084c0147aa95ae203c92fae9d8_B_3 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[2];
					half _Split_70a657084c0147aa95ae203c92fae9d8_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2;
					Unity_Subtract_half(_Split_85dff094aeb8431bb83b308f04d92c8a_R_1, _Split_70a657084c0147aa95ae203c92fae9d8_R_1, _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEOVERLAP)
						half _UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0 = _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2;
					#else
						half _UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0 = _Split_85dff094aeb8431bb83b308f04d92c8a_R_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0 = _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1;
					#else
						half3 _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0 = _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[0];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[1];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[2];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_bddc46084726488486404f0519eec7a2_Out_2;
					Unity_Add_half(_UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1, _Add_bddc46084726488486404f0519eec7a2_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2;
					Unity_Multiply_half_half(_Split_85dff094aeb8431bb83b308f04d92c8a_G_2, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2, _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2;
					Unity_Multiply_half_half(_Split_85dff094aeb8431bb83b308f04d92c8a_B_3, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3, _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3;
					Unity_Lerp_half(_Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3, _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2, _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0 = half3(_Add_bddc46084726488486404f0519eec7a2_Out_2, _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2, _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1;
					Unity_ColorspaceConversion_HSV_Linear_half(_Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0, _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1;
					Unity_ColorspaceConversion_HSV_RGB_half(_Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0, _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0 = _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1;
					#else
						half3 _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0 = _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1;
					Unity_Preview_half(_Split_70a657084c0147aa95ae203c92fae9d8_R_1, _Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2;
					Unity_Subtract_half(_Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1, _Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_79bffc4fe077474d9663b56d25efe072_Out_2;
					Unity_Add_half(_Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2, 1, _Add_79bffc4fe077474d9663b56d25efe072_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2;
					Unity_Modulo_half(_Add_79bffc4fe077474d9663b56d25efe072_Out_2, 1, _Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1;
					Unity_OneMinus_half(_Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2, _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2;
					Unity_Minimum_half(_Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2, _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1, _Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_b931e3eaf49e4f82bd117c09747fa211_Out_0 = OverlapStart;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_7e0d91b8365d44b4a3fe7cebc479fb8d_Out_0 = Overlap;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2;
					Unity_Multiply_half_half(_Property_7e0d91b8365d44b4a3fe7cebc479fb8d_Out_0, 0.5, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2;
					Unity_Multiply_half_half(_Property_b931e3eaf49e4f82bd117c09747fa211_Out_0, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2, _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2;
					Unity_Multiply_half_half(_Property_b931e3eaf49e4f82bd117c09747fa211_Out_0, _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2, _Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Vector2_b97f502493144dd885c0b273fe425fe7_Out_0 = half2(_Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Remap_e20f291471ea49129644ba9362df4bba_Out_3;
					Unity_Remap_half(_Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2, _Vector2_b97f502493144dd885c0b273fe425fe7_Out_0, half2 (1, 0), _Remap_e20f291471ea49129644ba9362df4bba_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3;
					Unity_Clamp_half(_Remap_e20f291471ea49129644ba9362df4bba_Out_3, 0, 1, _Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3;
					Unity_Lerp_half3((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0, (_Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3.xxx), _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEOVERLAP)
						half3 _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0 = _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3;
					#else
						half3 _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0 = _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
					half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
					half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
					half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
					Unity_Multiply_half_half(_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				#endif
				surface.BaseColor = _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0;
				surface.Alpha = _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpaceNormal =           input.normalOS;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpaceTangent =          input.tangentOS.xyz;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpacePosition =         input.positionOS;
				#endif


				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.uv0 = input.texCoord0;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.VertexColor = input.color;
				#endif

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}

			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

			ENDHLSL
		}
		Pass
		{
			Name "Sprite Unlit"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

			// Render State
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask]

			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}
			
			HLSLPROGRAM

			// Pragmas
			#pragma target 2.0
			#pragma exclude_renderers d3d11_9x
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			#pragma shader_feature_local COLORSPACE_LINEAR COLORSPACE_RGB
			#pragma shader_feature_local _ USEOVERLAP

			#if defined(COLORSPACE_LINEAR) && defined(USEOVERLAP)
				#define KEYWORD_PERMUTATION_0
			#elif defined(COLORSPACE_LINEAR)
				#define KEYWORD_PERMUTATION_1
			#elif defined(COLORSPACE_RGB) && defined(USEOVERLAP)
				#define KEYWORD_PERMUTATION_2
			#else
				#define KEYWORD_PERMUTATION_3
			#endif

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _ALPHAPREMULTIPLY_ON 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define ATTRIBUTES_NEED_COLOR
			#define VARYINGS_NEED_TEXCOORD0
			#define VARYINGS_NEED_COLOR
			#define FEATURES_GRAPH_VERTEX
			/* WARNING: $splice Could not find named fragment 'PassInstancing' */
			#define SHADERPASS SHADERPASS_SPRITEFORWARD
			/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

			// Includes
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 positionOS : POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 normalOS : NORMAL;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 tangentOS : TANGENT;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 uv0 : TEXCOORD0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 color : COLOR;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				#endif
			};
			struct Varyings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 positionCS : SV_POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 texCoord0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 color;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 uv0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 VertexColor;
				#endif
			};
			struct VertexDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpaceNormal;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpaceTangent;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float3 ObjectSpacePosition;
				#endif
			};
			struct PackedVaryings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 positionCS : SV_POSITION;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 interp0 : INTERP0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 interp1 : INTERP1;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
				PackedVaryings PackVaryings (Varyings input)
				{
					PackedVaryings output;
					output.positionCS = input.positionCS;
					output.interp0.xyzw =  input.texCoord0;
					output.interp1.xyzw =  input.color;
					#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
					#endif
					return output;
				}
				Varyings UnpackVaryings (PackedVaryings input)
				{
					Varyings output;
					output.positionCS = input.positionCS;
					output.texCoord0 = input.interp0.xyzw;
					output.color = input.interp1.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
					#endif
					return output;
				}
			#endif

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			half Overlap;
			half OverlapStart;
			half4 OverlapColor;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions

			void Unity_ColorspaceConversion_Linear_HSV_half(half3 In, out half3 Out)
			{
				half3 sRGBLo = In * 12.92;
				half3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), half3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
				half3 Linear = half3(In <= 0.0031308) ? sRGBLo : sRGBHi;
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(Linear.bg, K.wz), half4(Linear.gb, K.xy), step(Linear.b, Linear.g));
				half4 Q = lerp(half4(P.xyw, Linear.r), half4(Linear.r, P.yzx), step(P.x, Linear.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_ColorspaceConversion_RGB_HSV_half(half3 In, out half3 Out)
			{
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_Subtract_half(half A, half B, out half Out)
			{
				Out = A - B;
			}

			void Unity_Add_half(half A, half B, out half Out)
			{
				Out = A + B;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
			{
				Out = A * B;
			}

			void Unity_Lerp_half(half A, half B, half T, out half Out)
			{
				Out = lerp(A, B, T);
			}

			void Unity_ColorspaceConversion_HSV_Linear_half(half3 In, out half3 Out)
			{
				half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
				half3 RGB = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
				half3 linearRGBLo = RGB / 12.92;
				half3 linearRGBHi = pow(max(abs((RGB + 0.055) / 1.055), 1.192092896e-07), half3(2.4, 2.4, 2.4));
				Out = half3(RGB <= 0.04045) ? linearRGBLo : linearRGBHi;
			}

			void Unity_ColorspaceConversion_HSV_RGB_half(half3 In, out half3 Out)
			{
				half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
				Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
			}

			void Unity_Preview_half(half In, out half Out)
			{
				Out = In;
			}

			void Unity_Modulo_half(half A, half B, out half Out)
			{
				Out = fmod(A, B);
			}

			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Minimum_half(half A, half B, out half Out)
			{
				Out = min(A, B);
			};

			void Unity_Remap_half(half In, half2 InMinMax, half2 OutMinMax, out half Out)
			{
				Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

			void Unity_Clamp_half(half In, half Min, half Max, out half Out)
			{
				Out = clamp(In, Min, Max);
			}

			void Unity_Lerp_half3(half3 A, half3 B, half3 T, out half3 Out)
			{
				Out = lerp(A, B, T);
			}


			// Custom interpolators pre vertex
			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

			// Graph Vertex
			struct VertexDescription
			{
				half3 Position;
				half3 Normal;
				half3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Custom interpolators, pre surface
			Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
			{
				return output;
			}
			#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC

			// Graph Pixel
			struct SurfaceDescription
			{
				half3 BaseColor;
				half Alpha;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, IN.uv0.xy);
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
					#else
						half3 _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0 = _ColorspaceConversion_1de5bad0fa8044ef85c2a8f02f090582_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_85dff094aeb8431bb83b308f04d92c8a_R_1 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[0];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_G_2 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[1];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_B_3 = _ColorSpace_ce6c08cd19ef4bfd81ec3f424cfa1f84_Out_0[2];
					half _Split_85dff094aeb8431bb83b308f04d92c8a_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0 = OverlapColor;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((_Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0.xyz), _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((_Property_378fef514beb4fc7a0aeba4a33a37ae8_Out_0.xyz), _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0 = _ColorspaceConversion_e17c89934a2e4557bda83fd37e36e67b_Out_1;
					#else
						half3 _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0 = _ColorspaceConversion_7331c4e1b88d4938bbc1d3bb11324fdd_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_70a657084c0147aa95ae203c92fae9d8_R_1 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[0];
					half _Split_70a657084c0147aa95ae203c92fae9d8_G_2 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[1];
					half _Split_70a657084c0147aa95ae203c92fae9d8_B_3 = _ColorSpace_aab85a01da0b4f6aac1fb27a66674b0b_Out_0[2];
					half _Split_70a657084c0147aa95ae203c92fae9d8_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2;
					Unity_Subtract_half(_Split_85dff094aeb8431bb83b308f04d92c8a_R_1, _Split_70a657084c0147aa95ae203c92fae9d8_R_1, _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEOVERLAP)
						half _UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0 = _Subtract_f9e2b2e09e344552975427e1893a059e_Out_2;
					#else
						half _UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0 = _Split_85dff094aeb8431bb83b308f04d92c8a_R_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1;
					Unity_ColorspaceConversion_Linear_HSV_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1;
					Unity_ColorspaceConversion_RGB_HSV_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0 = _ColorspaceConversion_92950cdb74024e2897913c6fcf0e9b23_Out_1;
					#else
						half3 _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0 = _ColorspaceConversion_5e19ae980bf640e99c51362fe5af1205_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[0];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[1];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3 = _ColorSpace_cf551d3d836b46b8bb151ffcc27906c7_Out_0[2];
					half _Split_e84137ee560d4cbaaed76d12a0cfa3f7_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_bddc46084726488486404f0519eec7a2_Out_2;
					Unity_Add_half(_UseOverlap_0e691839effa407a9120b58a07f9e512_Out_0, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1, _Add_bddc46084726488486404f0519eec7a2_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2;
					Unity_Multiply_half_half(_Split_85dff094aeb8431bb83b308f04d92c8a_G_2, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2, _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2;
					Unity_Multiply_half_half(_Split_85dff094aeb8431bb83b308f04d92c8a_B_3, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3, _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3;
					Unity_Lerp_half(_Split_e84137ee560d4cbaaed76d12a0cfa3f7_B_3, _Multiply_f2f937848a954bfca5b5695f1639e7f1_Out_2, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_G_2, _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0 = half3(_Add_bddc46084726488486404f0519eec7a2_Out_2, _Multiply_5df4a1a9eb314ed1b1e9fe90c26d6e42_Out_2, _Lerp_8911a758a2124477aea5aa3c65c4e79f_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1;
					Unity_ColorspaceConversion_HSV_Linear_half(_Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0, _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1;
					Unity_ColorspaceConversion_HSV_RGB_half(_Vector3_d243e41b8dfb4f0295bcd238cb255081_Out_0, _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(COLORSPACE_LINEAR)
						half3 _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0 = _ColorspaceConversion_cd9cf334754b4f90a248a7d213e2b909_Out_1;
					#else
						half3 _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0 = _ColorspaceConversion_42cd7bf6e3644b999f207268ce832b49_Out_1;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1;
					Unity_Preview_half(_Split_70a657084c0147aa95ae203c92fae9d8_R_1, _Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2;
					Unity_Subtract_half(_Preview_6bcdedac70b24ee88e4dc231ab13ab6d_Out_1, _Split_e84137ee560d4cbaaed76d12a0cfa3f7_R_1, _Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_79bffc4fe077474d9663b56d25efe072_Out_2;
					Unity_Add_half(_Subtract_bc455be5dfd74d3bbb7b51db6a1db86a_Out_2, 1, _Add_79bffc4fe077474d9663b56d25efe072_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2;
					Unity_Modulo_half(_Add_79bffc4fe077474d9663b56d25efe072_Out_2, 1, _Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1;
					Unity_OneMinus_half(_Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2, _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2;
					Unity_Minimum_half(_Modulo_2ceb70dc575d4e7ba6df4719365b003c_Out_2, _OneMinus_8a1e15ae00fd42d1a7bde9a47a681827_Out_1, _Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_b931e3eaf49e4f82bd117c09747fa211_Out_0 = OverlapStart;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_7e0d91b8365d44b4a3fe7cebc479fb8d_Out_0 = Overlap;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2;
					Unity_Multiply_half_half(_Property_7e0d91b8365d44b4a3fe7cebc479fb8d_Out_0, 0.5, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2;
					Unity_Multiply_half_half(_Property_b931e3eaf49e4f82bd117c09747fa211_Out_0, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2, _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2;
					Unity_Multiply_half_half(_Property_b931e3eaf49e4f82bd117c09747fa211_Out_0, _Multiply_a704f6a4cc914cdcaf4c8871e2301168_Out_2, _Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Vector2_b97f502493144dd885c0b273fe425fe7_Out_0 = half2(_Multiply_b46d1c740e9c47109723d67bcccf94b0_Out_2, _Multiply_d24754a5062048ff9653fde0a0e2f52a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Remap_e20f291471ea49129644ba9362df4bba_Out_3;
					Unity_Remap_half(_Minimum_7759dfcda42a4d6db6c423b4a79a0ffb_Out_2, _Vector2_b97f502493144dd885c0b273fe425fe7_Out_0, half2 (1, 0), _Remap_e20f291471ea49129644ba9362df4bba_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3;
					Unity_Clamp_half(_Remap_e20f291471ea49129644ba9362df4bba_Out_3, 0, 1, _Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half3 _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3;
					Unity_Lerp_half3((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0, (_Clamp_715f9a13384c46aaa6bc1cf4ad7ebdca_Out_3.xxx), _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEOVERLAP)
						half3 _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0 = _Lerp_17861981aa1842be99abe3deb84c27e4_Out_3;
					#else
						half3 _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0 = _ColorSpace_221936e363f84dff95fc707740c33a60_Out_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
					half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
					half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
					half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
					Unity_Multiply_half_half(_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				#endif
				surface.BaseColor = _UseOverlap_3ca0b5e6d5f1428f9defa3c679e0e24a_Out_0;
				surface.Alpha = _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpaceNormal =           input.normalOS;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpaceTangent =          input.tangentOS.xyz;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.ObjectSpacePosition =         input.positionOS;
				#endif


				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.uv0 = input.texCoord0;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					output.VertexColor = input.color;
				#endif

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}

			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

			ENDHLSL
		}
	}
	FallBack "Hidden/Shader Graph/FallbackError"
}