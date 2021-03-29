Shader "RenderTexture"
{
	Properties
	{
		[NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
		[KeywordEnum(Linear, Point)]FILTER("Filter", Float) = 0

		_Stencil("Stencil ID", Float) = 0
		_StencilComp("StencilComp", Float) = 8
		_StencilOp("StencilOp", Float) = 0
		_StencilReadMask("StencilReadMask", Float) = 255
		_StencilWriteMask("StencilWriteMask", Float) = 255
		_ColorMask("ColorMask", Float) = 15
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

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}
			ColorMask [_ColorMask]

			HLSLPROGRAM

			// Pragmas
			#pragma target 2.0
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			// PassKeywords: <None>
			#pragma shader_feature_local FILTER_LINEAR FILTER_POINT

			#if defined(FILTER_LINEAR)
				#define KEYWORD_PERMUTATION_0
			#else
				#define KEYWORD_PERMUTATION_1
			#endif


			// Defines
			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define _SURFACE_TYPE_TRANSPARENT 1
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_NORMAL
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_TANGENT
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_TEXCOORD0
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_COLOR
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define VARYINGS_NEED_TEXCOORD0
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define VARYINGS_NEED_COLOR
			#endif

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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 positionOS : POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 normalOS : NORMAL;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 tangentOS : TANGENT;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 uv0 : TEXCOORD0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 color : COLOR;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				#endif
			};
			struct Varyings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 positionCS : SV_POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 texCoord0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 color;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 uv0;
				#endif
			};
			struct VertexDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpaceNormal;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpaceTangent;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpacePosition;
				#endif
			};
			struct PackedVaryings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 positionCS : SV_POSITION;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 interp0 : INTERP0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 interp1 : INTERP1;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
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
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Clamp);
			SAMPLER(SamplerState_Point_Clamp);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions
			
			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Subtract_half4(half4 A, half4 B, out half4 Out)
			{
				Out = A - B;
			}

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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Linear_Clamp).samplerstate, IN.uv0.xy);
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					UnityTexture2D _Property_657928597c624e1a933092f653b1624d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0 = SAMPLE_TEXTURE2D(_Property_657928597c624e1a933092f653b1624d_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Point_Clamp).samplerstate, IN.uv0.xy);
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_R_4 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.r;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_G_5 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.g;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_B_6 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.b;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_A_7 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					#if defined(FILTER_LINEAR)
						half4 _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0;
					#else
						half4 _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half _Split_f2fad59f023c435f86e9a1669c51db65_R_1 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[0];
					half _Split_f2fad59f023c435f86e9a1669c51db65_G_2 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[1];
					half _Split_f2fad59f023c435f86e9a1669c51db65_B_3 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[2];
					half _Split_f2fad59f023c435f86e9a1669c51db65_A_4 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half _OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1;
					Unity_OneMinus_half(_Split_f2fad59f023c435f86e9a1669c51db65_A_4, _OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2;
					Unity_Subtract_half4(_Filter_5593f98bc9344e4f8a882782dccd0676_Out_0, (_OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1.xxxx), _Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2);
				#endif
				surface.BaseColor = (_Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2.xyz);
				surface.Alpha = _Split_f2fad59f023c435f86e9a1669c51db65_A_4;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpaceNormal =           input.normalOS;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpaceTangent =          input.tangentOS.xyz;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpacePosition =         input.positionOS;
				#endif


				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.uv0 =                         input.texCoord0;
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
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

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
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest [unity_GUIZTestMode]
			ZWrite Off

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

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
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			// PassKeywords: <None>
			#pragma shader_feature_local FILTER_LINEAR FILTER_POINT

			#if defined(FILTER_LINEAR)
				#define KEYWORD_PERMUTATION_0
			#else
				#define KEYWORD_PERMUTATION_1
			#endif


			// Defines
			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define _SURFACE_TYPE_TRANSPARENT 1
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_NORMAL
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_TANGENT
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_TEXCOORD0
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define ATTRIBUTES_NEED_COLOR
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define VARYINGS_NEED_TEXCOORD0
			#endif

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
				#define VARYINGS_NEED_COLOR
			#endif

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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 positionOS : POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 normalOS : NORMAL;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 tangentOS : TANGENT;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 uv0 : TEXCOORD0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 color : COLOR;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				#endif
			};
			struct Varyings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 positionCS : SV_POSITION;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 texCoord0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 color;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 uv0;
				#endif
			};
			struct VertexDescriptionInputs
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpaceNormal;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpaceTangent;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float3 ObjectSpacePosition;
				#endif
			};
			struct PackedVaryings
			{
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 positionCS : SV_POSITION;
				#endif
				#if UNITY_ANY_INSTANCING_ENABLED
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 interp0 : INTERP0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					float4 interp1 : INTERP1;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				#endif
			};

			#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
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
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Clamp);
			SAMPLER(SamplerState_Point_Clamp);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions
			
			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Subtract_half4(half4 A, half4 B, out half4 Out)
			{
				Out = A - B;
			}

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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Linear_Clamp).samplerstate, IN.uv0.xy);
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
					half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					UnityTexture2D _Property_657928597c624e1a933092f653b1624d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0 = SAMPLE_TEXTURE2D(_Property_657928597c624e1a933092f653b1624d_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Point_Clamp).samplerstate, IN.uv0.xy);
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_R_4 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.r;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_G_5 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.g;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_B_6 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.b;
					half _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_A_7 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					#if defined(FILTER_LINEAR)
						half4 _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0;
					#else
						half4 _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0 = _SampleTexture2D_42721252271c49e8a829f0a3a9fe7758_RGBA_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half _Split_f2fad59f023c435f86e9a1669c51db65_R_1 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[0];
					half _Split_f2fad59f023c435f86e9a1669c51db65_G_2 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[1];
					half _Split_f2fad59f023c435f86e9a1669c51db65_B_3 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[2];
					half _Split_f2fad59f023c435f86e9a1669c51db65_A_4 = _Filter_5593f98bc9344e4f8a882782dccd0676_Out_0[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half _OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1;
					Unity_OneMinus_half(_Split_f2fad59f023c435f86e9a1669c51db65_A_4, _OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					half4 _Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2;
					Unity_Subtract_half4(_Filter_5593f98bc9344e4f8a882782dccd0676_Out_0, (_OneMinus_ea5724814fd1456086fcecd9e7aac54a_Out_1.xxxx), _Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2);
				#endif
				surface.BaseColor = (_Subtract_a47e533121ac4fbdb0a71a700adfa0d3_Out_2.xyz);
				surface.Alpha = _Split_f2fad59f023c435f86e9a1669c51db65_A_4;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpaceNormal =           input.normalOS;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpaceTangent =          input.tangentOS.xyz;
				#endif

				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.ObjectSpacePosition =         input.positionOS;
				#endif


				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1)
					output.uv0 =                         input.texCoord0;
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
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

			ENDHLSL
		}
	}
	FallBack "Hidden/Shader Graph/FallbackError"
}