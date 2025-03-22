Shader "Custom/MoveShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]  _SrcFactor("SrcFactor",int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _DestFactor("SrcFactor",int) = 0
        _Color("Color(RGB)",Color) = (1,1,1,1)
        _MainTex("MainTex",2D) = "gary"{}
        // 定义行列数以及速度
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
            // 顶点着色器的输入
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv :TEXCOORD0;
            };
            
            // 顶点着色器的输出
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv :TEXCOORD0;

            };
            
            
            // 顶点着色器
            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                //利用矩阵变换把摄像机从世界空间变换到本地空间,并进行归一化。
                float3 viewdir = normalize(mul(GetWorldToObjectMatrix(),_WorldSpaceCameraPos));
                // 假设向上方向等同于世界空间下的y轴
                float3 updir = float3(0,1,0);
                // 叉积计算向右的方向
                // Unity中本地空间为左手坐标系（只有观察空间才是右手坐标系），
                float3 rightdir = normalize(cross(viewdir,updir));
                // 对updir进行矫正
                updir = normalize(cross(rightdir,viewdir));
                // 构建旋转矩阵
                float4x4 M = float4x4(
                rightdir.x,updir.x,viewdir.x,0,
                rightdir.y,updir.y,viewdir.y,0,
                rightdir.z,updir.z,viewdir.z,0,
                0,0,0,1
                );
                // 计算旋转后的坐标
               float3 newpos = mul(M,v.positionOS);
                o.positionCS = TransformObjectToHClip(newpos);
                // 提取关键帧数量，并将其定义在左上角为开始的帧图像
                o.uv = float2(v.uv.x/_Squence.y,(v.uv.y/_Squence.x) + (1/_Squence.x)*(_Squence.x-1));
                //x方向流动，随着_Time不断增大出现精度问题，frac保证其精度
                o.uv.x += frac(floor(_Time.y*_Squence.z*_Squence.y)/_Squence.y);
                //使用减法，每一行结束，跳到下一行
                o.uv.y -= frac(floor(_Time.y*_Squence.z)/_Squence.x);
                
                return o;
            }

            // 片段着色器
            half4 frag(Varyings i) : SV_TARGET 
            {    
                //进行纹理采样 SAMPLE_TEXTURE2D(纹理名，采样器名，uv)
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex,smp,i.uv);
                half4 c = _Color * mainTex;
                return c;
            }
            
            ENDHLSL
        } 
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}