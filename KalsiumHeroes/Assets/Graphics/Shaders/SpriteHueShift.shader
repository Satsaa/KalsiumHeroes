Shader "UI/HueShift"
{
	Properties
	{
		[NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
		
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
			#pragma target 2.0
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
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
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions

			void Unity_ColorspaceConversion_RGB_HSV_half(half3 In, out half3 Out)
			{
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_Hue_Normalized_half(half3 In, half Offset, out half3 Out)
			{
				// RGB to HSV
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half E = 1e-4;
				half3 hsv = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

				half hue = hsv.x + Offset;
				hsv.x = (hue < 0)
				? hue + 1
				: (hue > 1)
				? hue - 1
				: hue;

				// HSV to RGB
				half4 K2 = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
				Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
			}

			void Unity_Saturation_half(half3 In, half Saturation, out half3 Out)
			{
				half luma = dot(In, half3(0.2126729, 0.7151522, 0.0721750));
				Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
			}

			void Unity_Multiply_half3_half3(half3 A, half3 B, out half3 Out)
			{
				Out = A * B;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
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
				UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, IN.uv0.xy);
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				half3 _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
				Unity_ColorspaceConversion_RGB_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1);
				half _Split_85dff094aeb8431bb83b308f04d92c8a_R_1 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[0];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_G_2 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[1];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_B_3 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[2];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_A_4 = 0;
				half3 _Hue_6d99ccf9a56f4234a17261730a127cec_Out_2;
				Unity_Hue_Normalized_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _Split_85dff094aeb8431bb83b308f04d92c8a_R_1, _Hue_6d99ccf9a56f4234a17261730a127cec_Out_2);
				half3 _Saturation_fc256e8183a146098e3779d9a49e1102_Out_2;
				Unity_Saturation_half(_Hue_6d99ccf9a56f4234a17261730a127cec_Out_2, _Split_85dff094aeb8431bb83b308f04d92c8a_G_2, _Saturation_fc256e8183a146098e3779d9a49e1102_Out_2);
				half3 _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2;
				Unity_Multiply_half3_half3(_Saturation_fc256e8183a146098e3779d9a49e1102_Out_2, (_Split_85dff094aeb8431bb83b308f04d92c8a_B_3.xxx), _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2);
				half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
				half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
				half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
				half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				half _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				Unity_Multiply_half_half(_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				surface.BaseColor = _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2;
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
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
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
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions

			void Unity_ColorspaceConversion_RGB_HSV_half(half3 In, out half3 Out)
			{
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half  E = 1e-10;
				Out = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			void Unity_Hue_Normalized_half(half3 In, half Offset, out half3 Out)
			{
				// RGB to HSV
				half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
				half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
				half D = Q.x - min(Q.w, Q.y);
				half E = 1e-4;
				half3 hsv = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

				half hue = hsv.x + Offset;
				hsv.x = (hue < 0)
				? hue + 1
				: (hue > 1)
				? hue - 1
				: hue;

				// HSV to RGB
				half4 K2 = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				half3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
				Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
			}

			void Unity_Saturation_half(half3 In, half Saturation, out half3 Out)
			{
				half luma = dot(In, half3(0.2126729, 0.7151522, 0.0721750));
				Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
			}

			void Unity_Multiply_half3_half3(half3 A, half3 B, out half3 Out)
			{
				Out = A * B;
			}

			void Unity_Multiply_half_half(half A, half B, out half Out)
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
				UnityTexture2D _Property_2021c158adb44da38b0138945bd6f43d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
				half4 _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2021c158adb44da38b0138945bd6f43d_Out_0.tex, _Property_2021c158adb44da38b0138945bd6f43d_Out_0.samplerstate, IN.uv0.xy);
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_R_4 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.r;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_G_5 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.g;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_B_6 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.b;
				half _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7 = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.a;
				half3 _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1;
				Unity_ColorspaceConversion_RGB_HSV_half((IN.VertexColor.xyz), _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1);
				half _Split_85dff094aeb8431bb83b308f04d92c8a_R_1 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[0];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_G_2 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[1];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_B_3 = _ColorspaceConversion_449f615eb6d547df9d27973825c4c5a7_Out_1[2];
				half _Split_85dff094aeb8431bb83b308f04d92c8a_A_4 = 0;
				half3 _Hue_6d99ccf9a56f4234a17261730a127cec_Out_2;
				Unity_Hue_Normalized_half((_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz), _Split_85dff094aeb8431bb83b308f04d92c8a_R_1, _Hue_6d99ccf9a56f4234a17261730a127cec_Out_2);
				half3 _Saturation_fc256e8183a146098e3779d9a49e1102_Out_2;
				Unity_Saturation_half(_Hue_6d99ccf9a56f4234a17261730a127cec_Out_2, _Split_85dff094aeb8431bb83b308f04d92c8a_G_2, _Saturation_fc256e8183a146098e3779d9a49e1102_Out_2);
				half3 _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2;
				Unity_Multiply_half3_half3(_Saturation_fc256e8183a146098e3779d9a49e1102_Out_2, (_Split_85dff094aeb8431bb83b308f04d92c8a_B_3.xxx), _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2);
				half _Split_0accdd5b7c81443f95c7630edfec656f_R_1 = IN.VertexColor[0];
				half _Split_0accdd5b7c81443f95c7630edfec656f_G_2 = IN.VertexColor[1];
				half _Split_0accdd5b7c81443f95c7630edfec656f_B_3 = IN.VertexColor[2];
				half _Split_0accdd5b7c81443f95c7630edfec656f_A_4 = IN.VertexColor[3];
				half _Multiply_782824cb72be435f90e739ffeb82156f_Out_2;
				Unity_Multiply_half_half(_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7, _Split_0accdd5b7c81443f95c7630edfec656f_A_4, _Multiply_782824cb72be435f90e739ffeb82156f_Out_2);
				surface.BaseColor = _Multiply_f91f8f15f9d042ab95704ad1cad959c1_Out_2;
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
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

			ENDHLSL
		}
	}
	FallBack "Hidden/Shader Graph/FallbackError"
}