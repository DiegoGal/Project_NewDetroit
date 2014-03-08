// Shader created with Shader Forge Beta 0.25 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32703,y:32721|diff-558-OUT,spec-2-A,normal-3-RGB,alpha-44-OUT,clip-164-R;n:type:ShaderForge.SFN_Tex2d,id:2,x:33313,y:32446,ptlb:Diffuse,tex:f4e68e511d6c5f44f8b0e01da2bd75ef,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3,x:33184,y:32771,ptlb:Normal,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:12,x:33311,y:32633,ptlb:Alpha Color,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Time,id:25,x:33807,y:33251;n:type:ShaderForge.SFN_Divide,id:37,x:33195,y:33122|A-111-OUT,B-38-OUT;n:type:ShaderForge.SFN_Vector1,id:38,x:33477,y:33380,v1:5;n:type:ShaderForge.SFN_Add,id:44,x:33018,y:33218|A-37-OUT,B-46-OUT;n:type:ShaderForge.SFN_Vector1,id:46,x:33195,y:33324,v1:0.65;n:type:ShaderForge.SFN_Cos,id:111,x:33477,y:33219|IN-25-TTR;n:type:ShaderForge.SFN_TexCoord,id:162,x:33849,y:32908,uv:0;n:type:ShaderForge.SFN_Tex2d,id:164,x:33037,y:32963,ptlb:Alpha Noise Texture,tex:a2b573b0be2ba504bb1cabac4d218ac9,ntxv:0,isnm:False|UVIN-229-OUT;n:type:ShaderForge.SFN_Add,id:229,x:33427,y:32956|A-414-OUT,B-550-OUT;n:type:ShaderForge.SFN_Multiply,id:414,x:33652,y:32940|A-162-UVOUT,B-514-OUT;n:type:ShaderForge.SFN_ValueProperty,id:514,x:33849,y:33089,ptlb:Alpha Noise UV mult,v1:3;n:type:ShaderForge.SFN_Divide,id:550,x:33652,y:33089|A-25-T,B-552-OUT;n:type:ShaderForge.SFN_ValueProperty,id:552,x:33849,y:33171,ptlb:Alpha Noise Velocity,v1:10;n:type:ShaderForge.SFN_Add,id:558,x:33033,y:32516|A-2-RGB,B-12-RGB;n:type:ShaderForge.SFN_ValueProperty,id:571,x:33977,y:33217,ptlb:Alpha Noise UV mult_copy,v1:3;proporder:2-3-12-164-514-552;pass:END;sub:END;*/

Shader "Custom/Alpha Color Noised Beat" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "white" {}
        _AlphaColor ("Alpha Color", Color) = (1,0,0,1)
        _AlphaNoiseTexture ("Alpha Noise Texture", 2D) = "white" {}
        _AlphaNoiseUVmult ("Alpha Noise UV mult", Float ) = 3
        _AlphaNoiseVelocity ("Alpha Noise Velocity", Float ) = 10
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float4 _AlphaColor;
            uniform sampler2D _AlphaNoiseTexture; uniform float4 _AlphaNoiseTexture_ST;
            uniform float _AlphaNoiseUVmult;
            uniform float _AlphaNoiseVelocity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_25 = _Time + _TimeEditor;
                float2 node_229 = ((i.uv0.rg*_AlphaNoiseUVmult)+(node_25.g/_AlphaNoiseVelocity));
                clip(tex2D(_AlphaNoiseTexture,TRANSFORM_TEX(node_229, _AlphaNoiseTexture)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_575 = i.uv0;
                float3 normalLocal = tex2D(_Normal,TRANSFORM_TEX(node_575.rg, _Normal)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz*2;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_2 = tex2D(_Diffuse,TRANSFORM_TEX(node_575.rg, _Diffuse));
                float3 specularColor = float3(node_2.a,node_2.a,node_2.a);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (node_2.rgb+_AlphaColor.rgb);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor,((cos(node_25.a)/5.0)+0.65));
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float4 _AlphaColor;
            uniform sampler2D _AlphaNoiseTexture; uniform float4 _AlphaNoiseTexture_ST;
            uniform float _AlphaNoiseUVmult;
            uniform float _AlphaNoiseVelocity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_25 = _Time + _TimeEditor;
                float2 node_229 = ((i.uv0.rg*_AlphaNoiseUVmult)+(node_25.g/_AlphaNoiseVelocity));
                clip(tex2D(_AlphaNoiseTexture,TRANSFORM_TEX(node_229, _AlphaNoiseTexture)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_576 = i.uv0;
                float3 normalLocal = tex2D(_Normal,TRANSFORM_TEX(node_576.rg, _Normal)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_2 = tex2D(_Diffuse,TRANSFORM_TEX(node_576.rg, _Diffuse));
                float3 specularColor = float3(node_2.a,node_2.a,node_2.a);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (node_2.rgb+_AlphaColor.rgb);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * ((cos(node_25.a)/5.0)+0.65),0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _AlphaNoiseTexture; uniform float4 _AlphaNoiseTexture_ST;
            uniform float _AlphaNoiseUVmult;
            uniform float _AlphaNoiseVelocity;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_25 = _Time + _TimeEditor;
                float2 node_229 = ((i.uv0.rg*_AlphaNoiseUVmult)+(node_25.g/_AlphaNoiseVelocity));
                clip(tex2D(_AlphaNoiseTexture,TRANSFORM_TEX(node_229, _AlphaNoiseTexture)).r - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _AlphaNoiseTexture; uniform float4 _AlphaNoiseTexture_ST;
            uniform float _AlphaNoiseUVmult;
            uniform float _AlphaNoiseVelocity;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 node_25 = _Time + _TimeEditor;
                float2 node_229 = ((i.uv0.rg*_AlphaNoiseUVmult)+(node_25.g/_AlphaNoiseVelocity));
                clip(tex2D(_AlphaNoiseTexture,TRANSFORM_TEX(node_229, _AlphaNoiseTexture)).r - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
