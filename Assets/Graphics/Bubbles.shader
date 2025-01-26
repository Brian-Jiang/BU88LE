Shader "Universal Render Pipeline/Bubbles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistanceTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define MAX_SHAPES 64

            int _BubbleCount;
            float _BubbleSizes[MAX_SHAPES];
            float4 _BubblePositions[MAX_SHAPES];
            float _BubbleRotationSpeeds[MAX_SHAPES];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DistanceTex;
            float4 _MainTex_ST;
            float4 _DistanceTex_ST;
            float4 _Color;

            //source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
            float smin(float a, float b, float k)
            {
                float h = max(k - abs(a - b), 0.0) / k;
                return min(a, b) - h * h * k * (1.0 / 4.0);
            }

            //source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
            float sdCircle(float2 p, float r)
            {
                return length(p) - r;
            }

            float2 RotateUV(float2 uv, float2 pivot, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);

                // Translate UV to the origin (pivot point)
                uv -= pivot;

                // Rotate UV
                float2 rotatedUV = float2(
                    uv.x * c - uv.y * s,
                    uv.x * s + uv.y * c
                );

                // Translate UV back from the origin
                return rotatedUV + pivot;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                // UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // // apply fog
                // // UNITY_APPLY_FOG(i.fogCoord, col);
                // return col;


                float2 p = i.uv;

                float2 closestBubbleCenterToPosition = float2(0, 0);
                float closestBubbleSize = 0;
                float closestBubbleRotationSpeed = 0;
                float closestBubbleDist = 10000000;
                float d = 10000000;
                float4 finalColor = float4(0, 0, 0, 0);
                float totalWeight = 0;
                for (int c = 0; c < _BubbleCount; ++c)
                {
                    // if (_SdfStartTimes[c] == -1)
                    //     continue;

                    const float size = _BubbleSizes[c];
                    const float3 position = _BubblePositions[c].xyz;

                    // float startTime = _SdfStartTimes[c];
                    // float2 direction = _SdfDirections[c].xy;
                    // float aliveTime = (startTime - _Time[1]) / 20.0;

                    // float2 movement = (aliveTime * direction * _EnableMovement);
                    float2 bubbleCenterToPosition = (p - position.xy);
                    // pos.x = ((pos.x + 1.0) % 2) - 1.0;
                    // pos.y = ((pos.y + 1.0) % 2) - 1.0;
                    float bubbleDist = sdCircle(bubbleCenterToPosition, size);
                    d = smin(d, bubbleDist, 0.05);
                    // d = min(d, bubbleDist);
                    // d = smin(d, sdCircle(pos, size), 0.1);
                    if (bubbleDist < closestBubbleDist)
                    {
                        closestBubbleCenterToPosition = bubbleCenterToPosition;
                        closestBubbleSize = size;
                        closestBubbleDist = bubbleDist;
                        closestBubbleRotationSpeed = _BubbleRotationSpeeds[c];
                    }

                    // Apply texture color for this bubble
                    // float mask = smoothstep(0.02, 0.03, bubbleDist);
                    // float mask = smoothstep(-0.01, 0.0, bubbleDist);
                    // float mask = smoothstep(-0.01, 0.01, bubbleDist);
                    // mask = saturate(1.0 - mask);
                    // float maskOutside = smoothstep(0.0, 0.02, bubbleDist);
                    // maskOutside = saturate(1.0 - maskOutside);
                    // mask = 1.0;
                    
                    // Sample the texture and add its contribution
                    // float2 uv = (bubbleCenterToPosition / (2.0 * size)) + 0.5; // Map pos to [0,1] for texture sampling
                    // float4 bubbleColor = tex2D(_MainTex, uv) * _Color;
                    // bubbleColor.a *= mask;
                    
                    // finalColor += bubbleColor; // Accumulate color contribution
                    
                    // totalWeight += mask * length(bubbleColor.xyz);
                    // totalWeight += 1;

                    if (bubbleDist < size / 4.0)
                    {
                        totalWeight += 1;
                    }
                    
                    // finalColor += float4(uv, 0, 1) * mask;
                }

                // Average the final color
                if (totalWeight > 0.0)
                {
                    // finalColor /= totalWeight;
                }
                
                // d = smoothstep(0.0, 0.02, d);
                // d = saturate(1 - d);
                // return d * _Color;

                // if (totalWeight > 2.1)
                // {
                //     return float4(1, 0, 0, 1);
                // }

                if (true || totalWeight > 1.0)
                {
                    // float mask = smoothstep(-0.01, 0.01, closestBubbleDist);
                    float mask = smoothstep(-0.01, 0.005, d);
                    mask = saturate(1.0 - mask);
                    
                    float2 closestBubbleUV = (closestBubbleCenterToPosition / (2.0 * closestBubbleSize)) + 0.5; // Map pos to [0,1] for texture sampling
                    float rotationSpeed = closestBubbleRotationSpeed;
                    float rotation = _Time[1] * rotationSpeed;
                    closestBubbleUV = RotateUV(closestBubbleUV, float2(0.5, 0.5), rotation);
                    float4 bubbleColor = tex2D(_MainTex, closestBubbleUV) * _Color;
                    bubbleColor.a *= mask;
                    finalColor = bubbleColor;
                }
                

                // float2 uv = (closestBubble / (2.0 * closestBubbleSize)) + 0.5;
                float2 distUV = float2(0.5, 1 - (-closestBubbleDist / closestBubbleSize));

                // distUV = TRANSFORM_TEX(distUV, _DistanceTex);
                // return float4(distUV, 0, 1);
                // return float4(1, 0, 0, 0.5);
                // return tex2D(_DistanceTex, distUV);

                float distProp = 1 - (-closestBubbleDist / closestBubbleSize);
                // float2 distUV = float2(0.5, d);
                // float4 distTransparency = tex2D(_DistanceTex, distUV);
                float threshold = 0.6;
                float trans = saturate((distProp - threshold) / (1 - threshold));
                finalColor.a = min(finalColor.a, trans);
                
                // finalColor.a = saturate(1.0 - smoothstep(0.02, 0.03, d));
                return finalColor;
                
            }
            ENDCG
        }
    }
}