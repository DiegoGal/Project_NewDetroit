// Shader created with Shader Forge Beta 0.25 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:0,x:32343,y:32851|diff-138-RGB,spec-163-OUT,gloss-144-OUT,normal-123-RGB,emission-261-OUT;n:type:ShaderForge.SFN_Tex2d,id:123,x:32850,y:33122,ptlb:Normal,tex:839587738573a48e28a59f1905941428,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:138,x:33144,y:32598,ptlb:Diffuse,tex:3d403fe3184a448fa8bc190c7f07f28c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:144,x:32827,y:33017,ptlb:Gloss,min:0,cur:0.8700649,max:1;n:type:ShaderForge.SFN_Power,id:147,x:32941,y:32771|VAL-138-A,EXP-205-OUT;n:type:ShaderForge.SFN_Multiply,id:163,x:32724,y:32866|A-147-OUT,B-232-OUT;n:type:ShaderForge.SFN_Slider,id:205,x:33144,y:32813,ptlb:PowerIntensity,min:0,cur:3.970745,max:10;n:type:ShaderForge.SFN_Slider,id:232,x:33144,y:32932,ptlb:MultiplyIntensity,min:0,cur:6.406479,max:10;n:type:ShaderForge.SFN_Cubemap,id:236,x:33140,y:33301,ptlb:node_236,cube:f466cf7415226e046b096197eb7341aa,pvfc:0|DIR-251-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:251,x:33379,y:33295;n:type:ShaderForge.SFN_Fresnel,id:257,x:33140,y:33467|NRM-123-RGB;n:type:ShaderForge.SFN_Power,id:259,x:32907,y:33402|VAL-236-RGB,EXP-257-OUT;n:type:ShaderForge.SFN_Slider,id:260,x:32963,y:33636,ptlb:node_260,min:0,cur:1.289274,max:5;n:type:ShaderForge.SFN_Multiply,id:261,x:32491,y:33425|A-269-OUT,B-260-OUT;n:type:ShaderForge.SFN_Multiply,id:269,x:32710,y:33402|A-138-A,B-259-OUT;proporder:138-123-205-232-144-236-260;pass:END;sub:END;*/

Shader "Custom/Cube Map" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _PowerIntensity ("PowerIntensity", Range(0, 10)) = 3.970745
        _MultiplyIntensity ("MultiplyIntensity", Range(0, 10)) = 6.406479
        _Gloss ("Gloss", Range(0, 1)) = 0.8700649
        _node236 ("node_236", Cube) = "_Skybox" {}
        _node260 ("node_260", Range(0, 5)) = 1.289274
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform float _PowerIntensity;
            uniform float _MultiplyIntensity;
            uniform samplerCUBE _node236;
            uniform float _node260;
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
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_285 = i.uv0;
                float3 node_123 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_285.rg, _Normal)));
                float3 normalLocal = node_123.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz*2;
////// Emissive:
                float4 node_138 = tex2D(_Diffuse,TRANSFORM_TEX(node_285.rg, _Diffuse));
                float3 emissive = ((node_138.a*pow(texCUBE(_node236,viewReflectDirection).rgb,(1.0-max(0,dot(node_123.rgb, viewDirection)))))*_node260);
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_163 = (pow(node_138.a,_PowerIntensity)*_MultiplyIntensity);
                float3 specularColor = float3(node_163,node_163,node_163);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_138.rgb;
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform float _PowerIntensity;
            uniform float _MultiplyIntensity;
            uniform samplerCUBE _node236;
            uniform float _node260;
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
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_286 = i.uv0;
                float3 node_123 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_286.rg, _Normal)));
                float3 normalLocal = node_123.rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_138 = tex2D(_Diffuse,TRANSFORM_TEX(node_286.rg, _Diffuse));
                float node_163 = (pow(node_138.a,_PowerIntensity)*_MultiplyIntensity);
                float3 specularColor = float3(node_163,node_163,node_163);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_138.rgb;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
