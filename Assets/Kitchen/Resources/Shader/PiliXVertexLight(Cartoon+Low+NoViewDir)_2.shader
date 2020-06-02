// 目前設計只受平行光不受點光源影響
Shader "PiliX/VertexLight(Cartoon+Low+NoViewDir)_2" {
	Properties {
	    _Color ("整體顏色", Color) = (0.5,0.5,0.5,1)
		_LightColor ("亮面顏色", Color) = (1,1,1,1)
		_ShadowColor("暗面顏色", Color) = (0,0,0,1)
		_MainTex ("角色貼圖 (RGB)", 2D) = "white" {}
		_RampTex ("卡通色階", 2D) = "white" {}
		_Outline_sampler ("外邊線調整(灰階)", 2D) = "white" {}
		_Outline_Width ("外邊線寬度", float) = 0
		_BasicDistance("受影響距離基準", Range(0.5, 5)) = 3
		_Line_Color ("外邊現顏色", Color) = (0,0,0,0)
		_Atten("外發光", Range(0,1)) = 0
		_RimColor ("外光暈顏色", Color) = (1,1,1,1)
		_RimPower ("外光暈強度", Range(0.5,8.0)) = 3.0
		_SpecCularColor ("折射顏色", Color) = (0.5, 0.5, 0.5,1)
		_Gloss("折射範圍", Range(0, 1)) = 0.5
		_GlossStrenght("折射強度", Range(1, 10)) = 1
		_SpecularOffset("折射偏移量", Range(-2, 2)) = 0
		_DissolveTex("溶解貼圖 (灰階)", 2D) = "white" {}
		_Tile("溶解密度", Range(0, 1)) = 1
		_Amount("溶解程度", Range(0,1)) = 0
		_DissSize("溶解大小", Range(0,1)) = 0.1
		_DissDelayTime("延遲溶解時間", Range(0,3)) = 0
		_DissColor("溶解顏色", Color) = (1,1,1,1)
		_AddColor("加成顏色", Color) = (1,1,1,1)
		_CutOff("Cut Off", Range(0.0,1)) = 0.2
		_GrayScale("灰階", Range(0.0,1)) = 0.0

		[Toggle] _PointLight("受點光源影響", Float) = 0
		[Header(R light weight G is be use to control cartoon level)]
		_LightWeight("貼圖控制參數(R控制受光程度 G控制卡通色接程度 B反向補光 A控制折射強度)", 2D) = "white" {}
	}
	SubShader {

		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}

		Pass {
			Name "Outline"

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "UnityPBSLighting.cginc"
			//#include "UnityStandardBRDF.cginc"
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_fog
			#pragma target 3.0

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Outline_sampler; 
			sampler2D _DissolveTex;
			fixed4 _Outline_sampler_ST;
			int _Outline_Width;
			fixed _Amount;
			fixed _Tile;
			fixed4 _Line_Color;
			fixed4 _Color;
			fixed _CutOff;
			fixed _BasicDistance;

			struct VertexInput {
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput {
				fixed4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;

				//根據外邊線調整給的紋素來計算凹凸
				float2 transform = TRANSFORM_TEX(o.uv0, _Outline_sampler);
				float4 OutlineOffset = float4(transform, 0, 0);
				float4 _Outline_sampler_var = tex2Dlod(_Outline_sampler, OutlineOffset);

				//根據Camera與頂點距離和受影響距離基準的比例計算外邊線擴展程度
				float Dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
				if(Dist < _BasicDistance)
				{
					Dist = max(0.01, Dist / _BasicDistance);
				}
				else
				{
					Dist = 1;
				}

				//另外根據Camera設定Field of View計算外邊線擴展比
				Dist *= 2 / unity_CameraProjection._m00;

				//計算位置偏移距離 : 凹凸 * 距離比 * 線寬 * 縮小比
				float3 posOffset = _Outline_sampler_var.rgb * Dist * _Outline_Width * 0.0001;

				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal * posOffset,1));

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			float4 frag(VertexOutput i, float facing : VFACE) : COLOR {

				fixed4 c = fixed4(0, 0, 0, 0);
				c.a =  _Line_Color.a;

				//若邊線顏色全透明度，不計算色相
				if(c.a - _CutOff <= 0)
				{
					discard;
					return c;
				}

				//若模型主色全透明度，不計算色相
				if(_Color.a <= 0.0)
				{
					discard;
					return c;
				}

				//計算溶解
				fixed ClipTex = tex2D(_DissolveTex, i.uv0 / _Tile).r;
				fixed ClipAmount = ClipTex - _Amount;

				if(_Amount > 0 && ClipAmount < 0)
				{
					discard;
					return c;
				}

				//若貼圖紋素顏色透明，不計算色相
				float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				if(_MainTex_var.a - _CutOff <= 0)
				{
					discard;
					return c;
				}

				c.rgb = _Line_Color.rgb;
				c.a *= _Color.a;

				return c;
			}
			ENDCG
		}

		Pass{
			//模型本身
			Tags{"LightMode" = "ForwardBase"}

			Blend SrcAlpha OneMinusSrcAlpha
			
			//level of detail
			LOD 200
			
			CGPROGRAM
			
			#pragma multi_compile __ _POINTLIGHT_ON
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

				fixed4 _Color;
				fixed4 _LightColor;				
				sampler2D _MainTex;
				fixed4 _MainTex_ST;
								
				struct vOUT{
					fixed4 pos : SV_POSITION;
					fixed2 uv : TEXCOORD0;
					fixed3 normalDir : TEXCOORD1;
					fixed3 worldPos : TEXCOORD2;
				};
				
				vOUT vert(appdata_tan v){
					vOUT o;
					
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.normalDir = mul(fixed4(v.normal, 0.0), unity_WorldToObject);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					return o;
				}
				sampler2D _DissolveTex;
				sampler2D _RampTex;
				fixed4 _DissColor;
				fixed4 _ShadowColor;
				fixed4 _AddColor;
				fixed4 _RimColor;
				fixed4 _SpecCularColor;
				fixed _Gloss;
				fixed _GlossStrenght;
				fixed _Phong;
				fixed _Tile;
				fixed _Amount;
				fixed _DissSize;
				fixed _Atten;
				fixed _AttenStep;
				fixed _SpecularOffset;
				fixed _RimPower;
				fixed _CutOff;
				fixed _GrayScale;
				sampler2D _LightWeight;

				fixed4 frag(vOUT o) : COLOR{

					//取得頂點視線向量單位
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
					//取得主光源對頂點向量單位
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
					
					//漫反射:計算光與法線交叉點積是否 < 0，最小取0
					fixed diff = max(0, dot(o.normalDir, lightDir) * 0.5 + 0.5);

					// 獲取控制貼圖參數R 用來控制光源接受度，G用來控制卡通色階程度
					fixed4 lightWeight = tex2D(_LightWeight, o.uv);
					fixed lightWeightR = lightWeight.r;
					fixed lightWeightG = lightWeight.g;
					fixed lightWeightB = lightWeight.b;
					fixed lightWeightA = lightWeight.a;

					//卡通色階
					//1. 根據diff結果從_RampTex上斜線取值(明暗色階)
					//2. 計算陰影顏色，陰影顏色根據陰影色透明度過度為明亮色
					//3. 計算陰影顏色到明亮色的顏色過度中相對於色階程度的顏色
					fixed3 diff_ramp = tex2D(_RampTex, float2(diff, diff)).rgb;
					diff_ramp = lerp(fixed3(diff,diff,diff), diff_ramp, lightWeightG);
					//fixed4 shadowColor = lerp(_LightColor, _ShadowColor, _ShadowColor.a);
					//diff_ramp = lerp(shadowColor.rgb, _LightColor.rgb, diff_ramp);
					//return fixed4(diff_ramp, 1);

					//計算原本貼圖蒙皮
					fixed4 c = tex2D(_MainTex, o.uv);

					//蒙皮灰階處理
					if(_GrayScale > 0)
					{
						c.rgb = lerp(c.rgb, dot(c.rgb, fixed3(0.2, 0.7, 0)), _GrayScale);
					}

					//折射(受視線影響)
					fixed3 finalHS;
					if(_GlossStrenght > 0)
					{
						//計算法線和反射向量的交叉點積
						fixed NdotV = max(0, dot(o.normalDir, normalize(worldViewDir + lightDir + _SpecularOffset)));
						//弱化點積的漸層
						fixed3 hairS = smoothstep(0, 0.01, NdotV + _Gloss - 0.5) * _Gloss;
						//反射顏色加成
						finalHS = _SpecCularColor.rgb * hairS * lightWeightA * _GlossStrenght;
					}

					fixed3 lightColor = _LightColor0.rgb;

#if _POINTLIGHT_ON
					// 點光源計算
					for (int index = 0; index < 4; index++)
					{
						half3 vertexToLightSource = half3(
							unity_4LightPosX0[index],
							unity_4LightPosY0[index],
							unity_4LightPosZ0[index]) - o.worldPos;
						fixed attenuation = (1.0 / length(vertexToLightSource)) * 0.5;
						fixed3 lightDirection = normalize(vertexToLightSource);
						fixed diffuseL = saturate(dot(o.normalDir, lightDirection));
						fixed3 diffuse = unity_LightColor[index].xyz * diffuseL * attenuation;
						lightColor += diffuse;
					}
#endif

					//蒙皮 *= ((漫反射 + 折射 + 1 - 反向補光) * 主光源顏色) + 整體色 - 0.5; 
					//整體色要做 -0.5 處理式因為美術希望可以用主色灰階來控制角色整體是顯亮還是顯暗
					fixed3 final_diff_ramp = (diff_ramp + finalHS + 1 - lightWeightB) * lightWeightR;
					fixed3 lightColorOffset = lerp(_ShadowColor, _LightColor, final_diff_ramp.r);
					c.rgb *= (lightColorOffset * lightColor) + (_Color - 0.5);
					//return fixed4(c.rgb, 1.0);

					//處理溶解
					if(_Amount > 0)
					{
						//根據溶解紋理色彩r值取得溶解值，
						fixed ClipValue = tex2D(_DissolveTex, o.uv / _Tile).r;

						//若溶解值(0~1)和溶解程度(0~1)相減小於0代表完全此紋素被溶解
						fixed ClipAmount = ClipValue - _Amount;
						if(ClipAmount < 0)
						{
							discard;
							return c;
						}

						//取得該紋素溶解色，由_DissSize和ClipAmount比較來取得溶解邊緣
						fixed3 DissColor = fixed3(1,1,1);
						if(ClipAmount < _DissSize)
						{
							DissColor = _DissColor * _DissSize / ClipAmount;
							DissColor *= _AddColor * 2;
						}
					    c.rgb *= DissColor;
					}

					//處理視野邊緣光
					if(_Atten > 0)
					{
						half rim = 1.0 - max(0, dot (worldViewDir, o.normalDir));
						c.rgb += _RimColor.rgb * pow (rim, _RimPower);
					}

					if(_Color.a <= 0.0)
					{
						discard;
						return c;
					}

					if(c.a - _CutOff <= 0)
					{
						discard;
						return c;
					}

					c.a *= _Color.a;

					return c;
				}
			
			ENDCG
		}

		Pass{
			Tags{"LightMode" = "ShadowCaster"}

			Blend SrcAlpha OneMinusSrcAlpha
			
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

				sampler2D _MainTex;
				fixed4 _MainTex_ST;
								
				struct vOUT{
					fixed4 pos : SV_POSITION;
					fixed2 uv : TEXCOORD0;
					fixed3 normalDir : TEXCOORD4;
					SHADOW_COORDS(5)
				};
				
				vOUT vert(appdata_tan v){
					vOUT o;
					
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.normalDir = normalize(mul(fixed4(v.normal, 0.0), unity_WorldToObject).xyz);
					
					TRANSFER_SHADOW(o);

					return o;
				}

				sampler2D _BumpMap;
				sampler2D _DissolveTex;
				fixed4 _Color;
				fixed _Tile;
				fixed _Amount;
				fixed _DissSize;
				
				fixed4 frag(vOUT o) : COLOR{

					half4 c = tex2D(_MainTex, o.uv);

					fixed ClipTex = tex2D(_DissolveTex, o.uv / _Tile).r;
					fixed ClipAmount = ClipTex - _Amount;

					if(_Amount > 0 && ClipAmount < 0)
					{
						discard;
					}

					if(_Color.a <= 0.0)
					{
						discard;
					}

					UNITY_LIGHT_ATTENUATION(atten, o, o.normalDir)

					return c;
				}
			
			ENDCG
		}
	}

	FallBack "Diffuse"
}