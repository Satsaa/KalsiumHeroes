Shader "UI/StretchTiler"
{
	Properties
	{
		[NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
		WidthScale("WidthScale", Float) = 0.12
		RawTiling("RawTiling", Float) = 0.25
		BlockSize("ReferenceBlockSize", Int) = 50
		MaxValue("ReferenceMaxValue", Int) = 100
		
		[Toggle(USEREFERENCEVALUES)]USEREFERENCEVALUES("UseReferenceValues", Float) = 1
		[Toggle(SHOWFIRSTHALF)]SHOWFIRSTHALF("ShowFirstHalf", Float) = 1
		
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

			Stencil{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}

			HLSLPROGRAM

			// Pragmas
			#pragma target 5.0
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			
			#pragma shader_feature_local _ USEREFERENCEVALUES
			#pragma shader_feature_local _ SHOWFIRSTHALF

			#if defined(USEREFERENCEVALUES) && defined(SHOWFIRSTHALF)
				#define KEYWORD_PERMUTATION_0
			#elif defined(USEREFERENCEVALUES)
				#define KEYWORD_PERMUTATION_1
			#elif defined(SHOWFIRSTHALF)
				#define KEYWORD_PERMUTATION_2
			#else
				#define KEYWORD_PERMUTATION_3
			#endif
			
			// PassKeywords: <None>
			// GraphKeywords: <None>
			
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
			#include "Assets/Packages/MyUnityCollection/Graphics/Shaders/Functions/CalculateLevelOfDetail.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0;
				float4 color;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				float4 uv0;
				float4 VertexColor;
			};
			struct VertexDescriptionInputs
			{
				float3 ObjectSpaceNormal;
				float3 ObjectSpaceTangent;
				float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				float4 interp0 : INTERP0;
				float4 interp1 : INTERP1;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

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

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			half WidthScale;
			half RawTiling;
			half BlockSize;
			half MaxValue;
			CBUFFER_END
			
			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			
			// Graph Functions

			void Unity_Divide_half(half A, half B, out half Out)
			{
				Out = A / B;
			}

			void Unity_Flipbook_InvertY_half (half2 UV, half Width, half Height, half Tile, half2 Invert, out half2 Out)
			{
				Tile = floor(fmod(Tile + half(0.00001), Width*Height));
				half2 tileCount = half2(1.0, 1.0) / half2(Width, Height);
				half tileX = (Tile - Width * floor(Tile * tileCount.x));
				half tileY = (Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
				Out = (UV + half2(tileX, tileY)) * tileCount;
			}

			void Unity_Maximum_half(half A, half B, out half Out)
			{
				Out = max(A, B);
			}

			void Unity_Modulo_half(half A, half B, out half Out)
			{
				Out = fmod(A, B);
			}

			void Unity_Comparison_Greater_half(half A, half B, out half Out)
			{
				Out = A > B ? 1 : 0;
			}

			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
			{
				Out = A * B;
			}

			void Unity_Clamp_half(half In, half Min, half Max, out half Out)
			{
				Out = clamp(In, Min, Max);
			}

			void Unity_Branch_half(half Predicate, half True, half False, out half Out)
			{
				Out = Predicate ? True : False;
			}

			void Unity_Add_half(half A, half B, out half Out)
			{
				Out = A + B;
			}

			void Unity_Combine_half(half R, half G, half B, half A, out half4 RGBA, out half3 RGB, out half2 RG)
			{
				RGBA = half4(R, G, B, A);
				RGB = half3(R, G, B);
				RG = half2(R, G);
			}

			struct Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float
			{
			};

			void SG_LOD_a5843b2c6e893af4a9aecb9706eab9da_float(UnityTexture2D Texture2D_C5D7BE21, UnitySamplerState SamplerState_Linear_Repeat, float2 Vector2_899F7EA1, Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float IN, out float LOD_1)
			{
				UnityTexture2D _Property_e0b1733cb0cedc8aada4bf90231525f5_Out_0 = Texture2D_C5D7BE21;
				UnitySamplerState _Property_adcc2a87f41bb48eafe15f7b4937f091_Out_0 = SamplerState_Linear_Repeat;
				float2 _Property_53406f45ec6e2381b88bb386b7f00996_Out_0 = Vector2_899F7EA1;
				float _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1;
				CalculateLevelOfDetail_float(_Property_e0b1733cb0cedc8aada4bf90231525f5_Out_0.tex, _Property_adcc2a87f41bb48eafe15f7b4937f091_Out_0.samplerstate, _Property_53406f45ec6e2381b88bb386b7f00996_Out_0, _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1);
				LOD_1 = _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1;
			}

			void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
			{
				Out = A * B;
			}

			void Unity_Multiply_float_float(float A, float B, out float Out)
			{
				Out = A * B;
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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_ec5a1ff845404926a7f7a32c24cb0022_Out_0 = BlockSize;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_3ecdd1a445c64773a1362b39992b3e32_Out_0 = MaxValue;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2;
					Unity_Divide_half(_Property_ec5a1ff845404926a7f7a32c24cb0022_Out_0, _Property_3ecdd1a445c64773a1362b39992b3e32_Out_0, _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_66d75453f6b24d6e8cd52b7412ace7b8_Out_0 = RawTiling;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEREFERENCEVALUES)
						half _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0 = _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2;
					#else
						half _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0 = _Property_66d75453f6b24d6e8cd52b7412ace7b8_Out_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4;
					half2 _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Invert = half2 (0, 1);
					Unity_Flipbook_InvertY_half(IN.uv0.xy, _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0, 1, 0, _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Invert, _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_5da2c462e12d4c98990536188d4194fc_R_1 = _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4[0];
					half _Split_5da2c462e12d4c98990536188d4194fc_G_2 = _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4[1];
					half _Split_5da2c462e12d4c98990536188d4194fc_B_3 = 0;
					half _Split_5da2c462e12d4c98990536188d4194fc_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2;
					Unity_Maximum_half(_Split_5da2c462e12d4c98990536188d4194fc_R_1, 0.5, _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(SHOWFIRSTHALF)
						half _ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0 = _Split_5da2c462e12d4c98990536188d4194fc_R_1;
					#else
						half _ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0 = _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2;
					Unity_Modulo_half(_ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0, 1, _Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2;
					Unity_Comparison_Greater_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, 0.5, _Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1;
					Unity_OneMinus_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, _OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_fdeaf47904fc45feb941bee4125014a9_Out_0 = WidthScale;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2;
					Unity_Multiply_half_half(_Property_fdeaf47904fc45feb941bee4125014a9_Out_0, _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2;
					Unity_Multiply_half_half(_OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2, _Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_deda8689646341218595ddf3e66a2370_Out_3;
					Unity_Clamp_half(_Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2, 0, 0.5, _Clamp_deda8689646341218595ddf3e66a2370_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1;
					Unity_OneMinus_half(_Clamp_deda8689646341218595ddf3e66a2370_Out_3, _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2;
					Unity_Multiply_half_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2, _Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3;
					Unity_Clamp_half(_Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2, 0, 0.5, _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Branch_d1bc098230dc4ae1aac25a941113c487_Out_3;
					Unity_Branch_half(_Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2, _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1, _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3, _Branch_d1bc098230dc4ae1aac25a941113c487_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_822ae62de8124e80933fdd749794f3af_Out_2;
					Unity_Add_half(_Branch_d1bc098230dc4ae1aac25a941113c487_Out_3, 0.5, _Add_822ae62de8124e80933fdd749794f3af_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGBA_4;
					half3 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGB_5;
					half2 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6;
					Unity_Combine_half(_Add_822ae62de8124e80933fdd749794f3af_Out_2, _Split_5da2c462e12d4c98990536188d4194fc_G_2, 0, 0, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGBA_4, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGB_5, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_df65d82bbcc84f5e83dd34d8da9ae84f_Out_0 = WidthScale;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2;
					Unity_Divide_half(1, _Property_df65d82bbcc84f5e83dd34d8da9ae84f_Out_0, _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4;
					half2 _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Invert = half2 (0, 1);
					Unity_Flipbook_InvertY_half(IN.uv0.xy, _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2, 1, 0, _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Invert, _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float _LOD_0e67dc15cc0145f38281486207036260;
					float _LOD_0e67dc15cc0145f38281486207036260_LOD_1;
					SG_LOD_a5843b2c6e893af4a9aecb9706eab9da_float(_Property_2021c158adb44da38b0138945bd6f43d_Out_0, UnityBuildSamplerStateStruct(SamplerState_Linear_Repeat), _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4, _LOD_0e67dc15cc0145f38281486207036260, _LOD_0e67dc15cc0145f38281486207036260_LOD_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
						float4 _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
					#else
						float4 _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6, _LOD_0e67dc15cc0145f38281486207036260_LOD_1);
					#endif
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_R_5 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.r;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_G_6 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.g;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_B_7 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.b;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_A_8 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 _Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2;
					Unity_Multiply_float4_float4(_SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0, IN.VertexColor, _Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
					half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
					half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
					half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
					Unity_Multiply_float_float(_SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_A_8, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				#endif
				surface.BaseColor = (_Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2.xyz);
				surface.Alpha = _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal =           input.normalOS;
				output.ObjectSpaceTangent =          input.tangentOS.xyz;
				output.ObjectSpacePosition =         input.positionOS;

				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				output.uv0 = input.texCoord0;
				output.VertexColor = input.color;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign = IS_FRONT_VFACE(input.cullFace, true, false);
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
			#pragma target 5.0
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			
			#pragma shader_feature_local _ USEREFERENCEVALUES
			#pragma shader_feature_local _ SHOWFIRSTHALF

			#if defined(USEREFERENCEVALUES) && defined(SHOWFIRSTHALF)
				#define KEYWORD_PERMUTATION_0
			#elif defined(USEREFERENCEVALUES)
				#define KEYWORD_PERMUTATION_1
			#elif defined(SHOWFIRSTHALF)
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
			#include "Assets/Packages/MyUnityCollection/Graphics/Shaders/Functions/CalculateLevelOfDetail.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0;
				float4 color;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				float4 uv0;
				float4 VertexColor;
			};
			struct VertexDescriptionInputs
			{
				float3 ObjectSpaceNormal;
				float3 ObjectSpaceTangent;
				float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				float4 interp0 : INTERP0;
				float4 interp1 : INTERP1;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

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

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			half WidthScale;
			half RawTiling;
			half BlockSize;
			half MaxValue;
			CBUFFER_END
			
			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			
			// Graph Functions

			void Unity_Divide_half(half A, half B, out half Out)
			{
				Out = A / B;
			}

			void Unity_Flipbook_InvertY_half (half2 UV, half Width, half Height, half Tile, half2 Invert, out half2 Out)
			{
				Tile = floor(fmod(Tile + half(0.00001), Width*Height));
				half2 tileCount = half2(1.0, 1.0) / half2(Width, Height);
				half tileX = (Tile - Width * floor(Tile * tileCount.x));
				half tileY = (Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
				Out = (UV + half2(tileX, tileY)) * tileCount;
			}

			void Unity_Maximum_half(half A, half B, out half Out)
			{
				Out = max(A, B);
			}

			void Unity_Modulo_half(half A, half B, out half Out)
			{
				Out = fmod(A, B);
			}

			void Unity_Comparison_Greater_half(half A, half B, out half Out)
			{
				Out = A > B ? 1 : 0;
			}

			void Unity_OneMinus_half(half In, out half Out)
			{
				Out = 1 - In;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
			{
				Out = A * B;
			}

			void Unity_Clamp_half(half In, half Min, half Max, out half Out)
			{
				Out = clamp(In, Min, Max);
			}

			void Unity_Branch_half(half Predicate, half True, half False, out half Out)
			{
				Out = Predicate ? True : False;
			}

			void Unity_Add_half(half A, half B, out half Out)
			{
				Out = A + B;
			}

			void Unity_Combine_half(half R, half G, half B, half A, out half4 RGBA, out half3 RGB, out half2 RG)
			{
				RGBA = half4(R, G, B, A);
				RGB = half3(R, G, B);
				RG = half2(R, G);
			}

			struct Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float
			{
			};

			void SG_LOD_a5843b2c6e893af4a9aecb9706eab9da_float(UnityTexture2D Texture2D_C5D7BE21, UnitySamplerState SamplerState_Linear_Repeat, float2 Vector2_899F7EA1, Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float IN, out float LOD_1)
			{
				UnityTexture2D _Property_e0b1733cb0cedc8aada4bf90231525f5_Out_0 = Texture2D_C5D7BE21;
				UnitySamplerState _Property_adcc2a87f41bb48eafe15f7b4937f091_Out_0 = SamplerState_Linear_Repeat;
				float2 _Property_53406f45ec6e2381b88bb386b7f00996_Out_0 = Vector2_899F7EA1;
				float _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1;
				CalculateLevelOfDetail_float(_Property_e0b1733cb0cedc8aada4bf90231525f5_Out_0.tex, _Property_adcc2a87f41bb48eafe15f7b4937f091_Out_0.samplerstate, _Property_53406f45ec6e2381b88bb386b7f00996_Out_0, _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1);
				LOD_1 = _CalculateLevelOfDetailCustomFunction_b818f30f9399208ea0bba6ec33d36319_MipLevel_1;
			}

			void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
			{
				Out = A * B;
			}

			void Unity_Multiply_float_float(float A, float B, out float Out)
			{
				Out = A * B;
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
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_ec5a1ff845404926a7f7a32c24cb0022_Out_0 = BlockSize;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_3ecdd1a445c64773a1362b39992b3e32_Out_0 = MaxValue;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2;
					Unity_Divide_half(_Property_ec5a1ff845404926a7f7a32c24cb0022_Out_0, _Property_3ecdd1a445c64773a1362b39992b3e32_Out_0, _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_66d75453f6b24d6e8cd52b7412ace7b8_Out_0 = RawTiling;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(USEREFERENCEVALUES)
						half _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0 = _Divide_46405e428108440c8ad96f210d6ea4d4_Out_2;
					#else
						half _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0 = _Property_66d75453f6b24d6e8cd52b7412ace7b8_Out_0;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4;
					half2 _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Invert = half2 (0, 1);
					Unity_Flipbook_InvertY_half(IN.uv0.xy, _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0, 1, 0, _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Invert, _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_5da2c462e12d4c98990536188d4194fc_R_1 = _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4[0];
					half _Split_5da2c462e12d4c98990536188d4194fc_G_2 = _Flipbook_f92aa8a1adc74e76a37c931f83c66038_Out_4[1];
					half _Split_5da2c462e12d4c98990536188d4194fc_B_3 = 0;
					half _Split_5da2c462e12d4c98990536188d4194fc_A_4 = 0;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2;
					Unity_Maximum_half(_Split_5da2c462e12d4c98990536188d4194fc_R_1, 0.5, _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(SHOWFIRSTHALF)
						half _ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0 = _Split_5da2c462e12d4c98990536188d4194fc_R_1;
					#else
						half _ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0 = _Maximum_1bb3f4589fb04b2ea828d21ca161a956_Out_2;
					#endif
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2;
					Unity_Modulo_half(_ShowFirstHalf_d73cc770d38a4ffdbbc2ea4b5fb7aabb_Out_0, 1, _Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2;
					Unity_Comparison_Greater_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, 0.5, _Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1;
					Unity_OneMinus_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, _OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_fdeaf47904fc45feb941bee4125014a9_Out_0 = WidthScale;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2;
					Unity_Multiply_half_half(_Property_fdeaf47904fc45feb941bee4125014a9_Out_0, _UseReferenceValues_71822a02dc4c4ee982b677de1e39cf76_Out_0, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2;
					Unity_Multiply_half_half(_OneMinus_a375b381bd9e488baf1ff370d73297d4_Out_1, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2, _Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_deda8689646341218595ddf3e66a2370_Out_3;
					Unity_Clamp_half(_Multiply_c600e987a1734098a5f2d5eae405d99d_Out_2, 0, 0.5, _Clamp_deda8689646341218595ddf3e66a2370_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1;
					Unity_OneMinus_half(_Clamp_deda8689646341218595ddf3e66a2370_Out_3, _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2;
					Unity_Multiply_half_half(_Modulo_bfa18ba35cf94accb8365a07a779a9f4_Out_2, _Multiply_6bee4a36ccc6438f8e3f43a7b04382b3_Out_2, _Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3;
					Unity_Clamp_half(_Multiply_df2a843500ba4058a0139cba9f65fc6a_Out_2, 0, 0.5, _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Branch_d1bc098230dc4ae1aac25a941113c487_Out_3;
					Unity_Branch_half(_Comparison_66ad4897019546d5bd7e6a184b8d3e13_Out_2, _OneMinus_fa182c219f354e33909308e90d9eff44_Out_1, _Clamp_a88d47e7964f4259acea09b1245fd86d_Out_3, _Branch_d1bc098230dc4ae1aac25a941113c487_Out_3);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Add_822ae62de8124e80933fdd749794f3af_Out_2;
					Unity_Add_half(_Branch_d1bc098230dc4ae1aac25a941113c487_Out_3, 0.5, _Add_822ae62de8124e80933fdd749794f3af_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half4 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGBA_4;
					half3 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGB_5;
					half2 _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6;
					Unity_Combine_half(_Add_822ae62de8124e80933fdd749794f3af_Out_2, _Split_5da2c462e12d4c98990536188d4194fc_G_2, 0, 0, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGBA_4, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RGB_5, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Property_df65d82bbcc84f5e83dd34d8da9ae84f_Out_0 = WidthScale;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2;
					Unity_Divide_half(1, _Property_df65d82bbcc84f5e83dd34d8da9ae84f_Out_0, _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half2 _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4;
					half2 _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Invert = half2 (0, 1);
					Unity_Flipbook_InvertY_half(IN.uv0.xy, _Divide_d06ea28cce0e4a539fb143eeff03b7cb_Out_2, 1, 0, _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Invert, _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					Bindings_LOD_a5843b2c6e893af4a9aecb9706eab9da_float _LOD_0e67dc15cc0145f38281486207036260;
					float _LOD_0e67dc15cc0145f38281486207036260_LOD_1;
					SG_LOD_a5843b2c6e893af4a9aecb9706eab9da_float(_Property_2021c158adb44da38b0138945bd6f43d_Out_0, UnityBuildSamplerStateStruct(SamplerState_Linear_Repeat), _Flipbook_fdc34b02f1f94ec9bb0a4e01b2013ebd_Out_4, _LOD_0e67dc15cc0145f38281486207036260, _LOD_0e67dc15cc0145f38281486207036260_LOD_1);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					#if defined(SHADER_API_GLES) && (SHADER_TARGET < 30)
						float4 _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
					#else
						float4 _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0 = SAMPLE_TEXTURE2D_LOD(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, _Combine_feec4c3f7f984a738b42c8e5c79e876b_RG_6, _LOD_0e67dc15cc0145f38281486207036260_LOD_1);
					#endif
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_R_5 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.r;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_G_6 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.g;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_B_7 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.b;
					float _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_A_8 = _SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0.a;
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float4 _Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2;
					Unity_Multiply_float4_float4(_SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_RGBA_0, IN.VertexColor, _Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2);
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
					half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
					half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
					half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				#endif
				#if defined(KEYWORD_PERMUTATION_0) || defined(KEYWORD_PERMUTATION_1) || defined(KEYWORD_PERMUTATION_2) || defined(KEYWORD_PERMUTATION_3)
					float _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
					Unity_Multiply_float_float(_SampleTexture2DLOD_c6afe89ee1d74f5aa00c3b2855ddac65_A_8, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				#endif
				surface.BaseColor = (_Multiply_dd399f4f8bea46c78b92fe472e83af39_Out_2.xyz);
				surface.Alpha = _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal =           input.normalOS;
				output.ObjectSpaceTangent =          input.tangentOS.xyz;
				output.ObjectSpacePosition =         input.positionOS;

				return output;
			}
			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				





				output.uv0 = input.texCoord0;
				output.VertexColor = input.color;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign = IS_FRONT_VFACE(input.cullFace, true, false);
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