// Shader created with Shader Forge Beta 0.25 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:0,x:32002,y:32787|diff-236-OUT,spec-245-OUT,gloss-144-OUT,normal-261-OUT;n:type:ShaderForge.SFN_Tex2d,id:123,x:32795,y:33292,ptlb:Main Normal,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:138,x:33293,y:32261,ptlb:Main Texture,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:144,x:32827,y:33017,ptlb:Gloss,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Power,id:147,x:33029,y:32684|VAL-138-A,EXP-205-OUT;n:type:ShaderForge.SFN_Multiply,id:163,x:32791,y:32787|A-147-OUT,B-232-OUT;n:type:ShaderForge.SFN_Slider,id:205,x:33267,y:32745,ptlb:Main Spec Power,min:0,cur:5,max:10;n:type:ShaderForge.SFN_Slider,id:232,x:33299,y:32907,ptlb:Main Spec Mult,min:0,cur:5,max:10;n:type:ShaderForge.SFN_Lerp,id:236,x:32613,y:32415|A-138-RGB,B-237-RGB,T-240-OUT;n:type:ShaderForge.SFN_Tex2d,id:237,x:33285,y:32525,ptlb:Second Texture,tex:3d403fe3184a448fa8bc190c7f07f28c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:238,x:33517,y:33391,ptlb:Mask,tex:c275c3cbf3b03c44db48a2673f4df49f,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Multiply,id:240,x:33288,y:33421|A-238-RGB,B-242-OUT;n:type:ShaderForge.SFN_Slider,id:242,x:33496,y:33600,ptlb:Mask Intensity,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:245,x:32532,y:32733|A-163-OUT,B-254-OUT,T-240-OUT;n:type:ShaderForge.SFN_Slider,id:248,x:33366,y:33978,ptlb:Second Spec Mult,min:0,cur:5,max:10;n:type:ShaderForge.SFN_Slider,id:250,x:33334,y:33816,ptlb:Second Spec Power,min:0,cur:5,max:10;n:type:ShaderForge.SFN_Power,id:252,x:33096,y:33755|VAL-237-A,EXP-250-OUT;n:type:ShaderForge.SFN_Multiply,id:254,x:32858,y:33858|A-252-OUT,B-248-OUT;n:type:ShaderForge.SFN_Tex2d,id:259,x:32560,y:33533,ptlb:Second Normal,tex:839587738573a48e28a59f1905941428,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:261,x:32493,y:33161|A-123-RGB,B-259-RGB,T-240-OUT;proporder:138-123-205-232-237-259-248-250-144-238-242;pass:END;sub:END;*/

Shader "Custom/Lerp Blending" {
    Properties {
        _MainTexture ("Main Texture", 2D) = "white" {}
        _MainNormal ("Main Normal", 2D) = "bump" {}
        _MainSpecPower ("Main Spec Power", Range(0, 10)) = 5
        _MainSpecMult ("Main Spec Mult", Range(0, 10)) = 5
        _SecondTexture ("Second Texture", 2D) = "white" {}
        _SecondNormal ("Second Normal", 2D) = "bump" {}
        _SecondSpecMult ("Second Spec Mult", Range(0, 10)) = 5
        _SecondSpecPower ("Second Spec Power", Range(0, 10)) = 5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Mask ("Mask", 2D) = "black" {}
        _MaskIntensity ("Mask Intensity", Range(0, 1)) = 1
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
            uniform sampler2D _MainNormal; uniform float4 _MainNormal_ST;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _Gloss;
            uniform float _MainSpecPower;
            uniform float _MainSpecMult;
            uniform sampler2D _SecondTexture; uniform float4 _SecondTexture_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _MaskIntensity;
            uniform float _SecondSpecMult;
            uniform float _SecondSpecPower;
            uniform sampler2D _SecondNormal; uniform float4 _SecondNormal_ST;
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
                float2 node_307 = i.uv0;
                float3 node_240 = (tex2D(_Mask,TRANSFORM_TEX(node_307.rg, _Mask)).rgb*_MaskIntensity);
                float3 normalLocal = lerp(UnpackNormal(tex2D(_MainNormal,TRANSFORM_TEX(node_307.rg, _MainNormal))).rgb,UnpackNormal(tex2D(_SecondNormal,TRANSFORM_TEX(node_307.rg, _SecondNormal))).rgb,node_240);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz*2;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_138 = tex2D(_MainTexture,TRANSFORM_TEX(node_307.rg, _MainTexture));
                float4 node_237 = tex2D(_SecondTexture,TRANSFORM_TEX(node_307.rg, _SecondTexture));
                float node_245 = lerp((pow(node_138.a,_MainSpecPower)*_MainSpecMult),(pow(node_237.a,_SecondSpecPower)*_SecondSpecMult),node_240);
                float3 specularColor = float3(node_245,node_245,node_245);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * lerp(node_138.rgb,node_237.rgb,node_240);
                finalColor += specular;
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
            uniform sampler2D _MainNormal; uniform float4 _MainNormal_ST;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _Gloss;
            uniform float _MainSpecPower;
            uniform float _MainSpecMult;
            uniform sampler2D _SecondTexture; uniform float4 _SecondTexture_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _MaskIntensity;
            uniform float _SecondSpecMult;
            uniform float _SecondSpecPower;
            uniform sampler2D _SecondNormal; uniform float4 _SecondNormal_ST;
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
                float2 node_308 = i.uv0;
                float3 node_240 = (tex2D(_Mask,TRANSFORM_TEX(node_308.rg, _Mask)).rgb*_MaskIntensity);
                float3 normalLocal = lerp(UnpackNormal(tex2D(_MainNormal,TRANSFORM_TEX(node_308.rg, _MainNormal))).rgb,UnpackNormal(tex2D(_SecondNormal,TRANSFORM_TEX(node_308.rg, _SecondNormal))).rgb,node_240);
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
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_138 = tex2D(_MainTexture,TRANSFORM_TEX(node_308.rg, _MainTexture));
                float4 node_237 = tex2D(_SecondTexture,TRANSFORM_TEX(node_308.rg, _SecondTexture));
                float node_245 = lerp((pow(node_138.a,_MainSpecPower)*_MainSpecMult),(pow(node_237.a,_SecondSpecPower)*_SecondSpecMult),node_240);
                float3 specularColor = float3(node_245,node_245,node_245);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * lerp(node_138.rgb,node_237.rgb,node_240);
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
