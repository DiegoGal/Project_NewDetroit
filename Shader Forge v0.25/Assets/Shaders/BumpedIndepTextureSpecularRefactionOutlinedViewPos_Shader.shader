// Shader created with Shader Forge Beta 0.25 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:0,x:32373,y:32853|spec-347-OUT,gloss-144-OUT,normal-123-RGB,alpha-449-OUT,refract-429-OUT,olwid-363-OUT,olcol-252-RGB;n:type:ShaderForge.SFN_Tex2d,id:123,x:33087,y:33234,ptlb:Normal,tex:2966d4b9495678d409120ea361832ed8,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:144,x:33087,y:33122,ptlb:Gloss,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Color,id:252,x:32709,y:33580,ptlb:Outline Color,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:327,x:32878,y:33529,ptlb:Outline Width,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:333,x:32910,y:32725|A-413-RGB,B-354-OUT;n:type:ShaderForge.SFN_Color,id:345,x:33123,y:32952,ptlb:Specular Color,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:347,x:32722,y:32848|A-333-OUT,B-345-RGB;n:type:ShaderForge.SFN_ValueProperty,id:354,x:33086,y:32857,ptlb:Specular Intensity,v1:1;n:type:ShaderForge.SFN_ViewPosition,id:361,x:33413,y:33534;n:type:ShaderForge.SFN_Multiply,id:363,x:32711,y:33403|A-372-OUT,B-327-OUT;n:type:ShaderForge.SFN_Multiply,id:372,x:32995,y:33403|A-403-OUT,B-373-OUT;n:type:ShaderForge.SFN_Vector1,id:373,x:33179,y:33603,v1:0.5;n:type:ShaderForge.SFN_ObjectPosition,id:382,x:33428,y:33403;n:type:ShaderForge.SFN_Distance,id:403,x:33179,y:33403|A-382-XYZ,B-361-XYZ;n:type:ShaderForge.SFN_Tex2d,id:413,x:33113,y:32669,ptlb:Specular,tex:8728117877f542f4db32ea6ba2f35f2b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:429,x:32625,y:33105|A-438-OUT,B-475-OUT;n:type:ShaderForge.SFN_ComponentMask,id:438,x:32723,y:33232,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-123-RGB;n:type:ShaderForge.SFN_Vector1,id:449,x:32670,y:33023,v1:0.3;n:type:ShaderForge.SFN_Slider,id:475,x:32855,y:33122,ptlb:Refraction Intensity,min:0,cur:0.05,max:0.1;proporder:413-123-354-144-345-475-327-252;pass:END;sub:END;*/

Shader "Custom/Bumped Indep Texture Specular Refraction Outlined View Position" {
    Properties {
        _Specular ("Specular", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _SpecularIntensity ("Specular Intensity", Float ) = 1
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _RefractionIntensity ("Refraction Intensity", Range(0, 0.1)) = 0.05
        _OutlineWidth ("Outline Width", Float ) = 0.1
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                float4 objPos = mul ( _Object2World, float4(0,0,0,1) );
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*((distance(objPos.rgb,_WorldSpaceCameraPos.rgb)*0.5)*_OutlineWidth),1));
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( _Object2World, float4(0,0,0,1) );
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Gloss;
            uniform float4 _SpecularColor;
            uniform float _SpecularIntensity;
            uniform sampler2D _Specular; uniform float4 _Specular_ST;
            uniform float _RefractionIntensity;
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
                float4 screenPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_484 = i.uv0;
                float3 node_123 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_484.rg, _Normal)));
                float3 normalLocal = node_123.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_123.rgb.rg*_RefractionIntensity);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                float NdotL = dot( normalDirection, lightDirection );
                NdotL = max(0.0, NdotL);
                float3 specularColor = ((tex2D(_Specular,TRANSFORM_TEX(node_484.rg, _Specular)).rgb*_SpecularIntensity)*_SpecularColor.rgb);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                finalColor += specular;
/// Final Color:
                return fixed4(lerp(sceneColor.rgb, finalColor,0.3),1);
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
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Gloss;
            uniform float4 _SpecularColor;
            uniform float _SpecularIntensity;
            uniform sampler2D _Specular; uniform float4 _Specular_ST;
            uniform float _RefractionIntensity;
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
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_485 = i.uv0;
                float3 node_123 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_485.rg, _Normal)));
                float3 normalLocal = node_123.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_123.rgb.rg*_RefractionIntensity);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                float NdotL = dot( normalDirection, lightDirection );
                NdotL = max(0.0, NdotL);
                float3 specularColor = ((tex2D(_Specular,TRANSFORM_TEX(node_485.rg, _Specular)).rgb*_SpecularIntensity)*_SpecularColor.rgb);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 0.3,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
