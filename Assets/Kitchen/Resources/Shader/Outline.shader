Shader "Custom/Outline" {
	Properties
	{
		_MainTex("main tex",2D) = "black"{}
		_RimColor("rim color",Color) = (1,1,1,1)//��Ե��ɫ
		_RimPower("rim power",range(1,10)) = 2//��Եǿ��
	}

		SubShader
		{
			Pass
		{
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include"UnityCG.cginc"

		struct v2f
		{
			float4 vertex:POSITION;
			float4 uv:TEXCOORD0;
			float4 NdotV:COLOR;
		};

		sampler2D _MainTex;
		float4 _RimColor;
		float _RimPower;

		v2f vert(appdata_base v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			float3 V = WorldSpaceViewDir(v.vertex);
			V = mul(unity_WorldToObject,V);//�ӷ�������絽ģ������ϵ��ת��
			o.NdotV.x = saturate(dot(v.normal,normalize(V)));//������ͬһ����ϵ������ȷ���������
			return o;
		}

		half4 frag(v2f IN) :COLOR
		{
			half4 c = tex2D(_MainTex,IN.uv);
			//���ӷ���ͷ��߷�������ˣ�Խ��Ե�ĵط������ߺ��ӷ���Խ�ӽ�90�ȣ����Խ�ӽ�0.
			//�ã�1- �����˵Ľ����*��ɫ������ӳ��Ե��ɫ���
			c.rgb += pow((1 - IN.NdotV.x) ,_RimPower)* _RimColor.rgb;
			return c;
		}
			ENDCG
		}
		}
			FallBack "Diffuse"
}
