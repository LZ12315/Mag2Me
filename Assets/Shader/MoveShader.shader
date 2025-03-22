Shader "Custom/MoveShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]  _SrcFactor("SrcFactor",int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _DestFactor("SrcFactor",int) = 0
        _Color("Color(RGB)",Color) = (1,1,1,1)
        _MainTex("MainTex",2D) = "gary"{}
        // �����������Լ��ٶ�
        _Squence("Row(X) Cloum(Y) Speed(Z)",vector) = (3,3,1,0)
    }
    SubShader
    {
        Tags
        {
            
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                
            }
            
            // Render State
            Blend [_SrcFactor] [_DestFactor]
            Cull off
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            half4 _Squence;
            CBUFFER_END
            
            TEXTURE2D(_MainTex);

            #define smp SamplerState_Point_Repeat
            SAMPLER(smp);
            // ������ɫ��������
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv :TEXCOORD0;
            };
            
            // ������ɫ�������
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv :TEXCOORD0;

            };
            
            
            // ������ɫ��
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                //���þ���任�������������ռ�任�����ؿռ�,�����й�һ����
                float3 viewdir = normalize(mul(GetWorldToObjectMatrix(),_WorldSpaceCameraPos));
                // �������Ϸ����ͬ������ռ��µ�y��
                float3 updir = float3(0,1,0);
                // ����������ҵķ���
                // Unity�б��ؿռ�Ϊ��������ϵ��ֻ�й۲�ռ������������ϵ����
                float3 rightdir = normalize(cross(viewdir,updir));
                // ��updir���н���
                updir = normalize(cross(rightdir,viewdir));
                // ������ת����
                float4x4 M = float4x4(
                rightdir.x,updir.x,viewdir.x,0,
                rightdir.y,updir.y,viewdir.y,0,
                rightdir.z,updir.z,viewdir.z,0,
                0,0,0,1
                );
                // ������ת�������
               float3 newpos = mul(M,v.positionOS);
                o.positionCS = TransformObjectToHClip(newpos);
                // ��ȡ�ؼ�֡�����������䶨�������Ͻ�Ϊ��ʼ��֡ͼ��
                o.uv = float2(v.uv.x/_Squence.y,(v.uv.y/_Squence.x) + (1/_Squence.x)*(_Squence.x-1));
                //x��������������_Time����������־������⣬frac��֤�侫��
                o.uv.x += frac(floor(_Time.y*_Squence.z*_Squence.y)/_Squence.y);
                //ʹ�ü�����ÿһ�н�����������һ��
                o.uv.y -= frac(floor(_Time.y*_Squence.z)/_Squence.x);
                
                return o;
            }

            // Ƭ����ɫ��
            half4 frag(Varyings i) : SV_TARGET 
            {    
                //����������� SAMPLE_TEXTURE2D(������������������uv)
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex,smp,i.uv);
                half4 c = _Color * mainTex;
                return c;
            }
            
            ENDHLSL
        } 
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}