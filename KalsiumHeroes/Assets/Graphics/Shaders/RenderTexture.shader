Shader "UI/RenderTexture"
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
			Blend One OneMinusSrcAlpha
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
			half _Float;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions
			// GraphFunctions: <None>

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
				surface.BaseColor = (_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz);
				surface.BaseColor = surface.BaseColor * IN.VertexColor.w;
				surface.Alpha = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7;
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
			Blend One OneMinusSrcAlpha
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
			half _Float;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			// Graph Functions
			// GraphFunctions: <None>

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
				surface.BaseColor = (_SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_RGBA_0.xyz);
				surface.BaseColor = surface.BaseColor * IN.VertexColor.w;
				surface.Alpha = _SampleTexture2D_a30fb39cbcea49a494784ce9f304166d_A_7;
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