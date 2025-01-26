Shader "Universal Render Pipeline/Bubbles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_ST;
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

            float sdEllipse(float2 p, float2 ab)
            {
                p = abs(p); if( p.x > p.y ) {p=p.yx;ab=ab.yx;}
                float l = ab.y*ab.y - ab.x*ab.x;
                float m = ab.x*p.x/l;      float m2 = m*m; 
                float n = ab.y*p.y/l;      float n2 = n*n; 
                float c = (m2+n2-1.0)/3.0; float c3 = c*c*c;
                float q = c3 + m2*n2*2.0;
                float d = c3 + m2*n2;
                float g = m + m*n2;
                float co;
                if( d<0.0 )
                {
                    float h = acos(q/c3)/3.0;
                    float s = cos(h);
                    float t = sin(h)*sqrt(3.0);
                    float rx = sqrt( -c*(s + t + 2.0) + m2 );
                    float ry = sqrt( -c*(s - t + 2.0) + m2 );
                    co = (ry+sign(l)*rx+abs(g)/(rx*ry)- m)/2.0;
                }
                else
                {
                    float h = 2.0*m*n*sqrt( d );
                    float s = sign(q+h)*pow(abs(q+h), 1.0/3.0);
                    float u = sign(q-h)*pow(abs(q-h), 1.0/3.0);
                    float rx = -s - u - c*4.0 + 2.0*m2;
                    float ry = (s - u)*sqrt(3.0);
                    float rm = sqrt( rx*rx + ry*ry );
                    co = (ry/sqrt(rm-rx)+2.0*g/rm-m)/2.0;
                }
                float2 r = ab * float2(co, sqrt(1.0-co*co));
                return length(r-p) * sign(p.y-r.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
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

                float d = 10000000;
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
                    float2 pos = (p - position);
                    pos.x = ((pos.x + 1.0) % 2) - 1.0;
                    pos.y = ((pos.y + 1.0) % 2) - 1.0;
                    d = smin(d, sdCircle(pos, size), 0.1);
                }

                d = smoothstep(0.02, 0.03, d);
                d = saturate(1 - d);
                return d * _Color;
            }
            ENDCG
        }
    }
}