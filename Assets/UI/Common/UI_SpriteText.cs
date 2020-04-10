using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpriteText : MonoBehaviour
{
	public enum Alignment : byte
	{
		left,
		center,
		right
	}

	[SerializeField] string m_strValue = null;

	[SerializeField] string m_strAsset = null;
	[SerializeField] Image m_preImage = null;

	[SerializeField] Transform m_transRoot = null;
	[SerializeField] float m_nSpaceNormal = 0;	// 普通字符间距;
	[SerializeField] float m_nSpecialOffset = 0;    // 特殊字符修正;


	private List<char> m_listChars
		= new List<char> { ':' };
	private List<char> m_listSpriteKeys
		= new List<char> { 'm' };

	private Dictionary<string, Sprite> m_dicSprites = new Dictionary<string, Sprite>();

	private List<Image> m_listImage = new List<Image>();

    void Start()
    {
		Object[] objs = Resources.LoadAll(m_strAsset,typeof(Sprite));
		for (int i = 0; i < objs.Length; i++)
		{
			Sprite sprite = (Sprite)objs[i];
			m_dicSprites.Add(objs[i].name, sprite);
		}

		if (m_strValue != null)
		{
			SetText(m_strValue);
		}
	}

	private Image GetImg(int nIndex)
	{
		Image result = null;
		if (nIndex < m_listImage.Count)
		{
			result = m_listImage[nIndex];
		}
		else
		{
			GameObject newObj = (GameObject)Instantiate(m_preImage.gameObject, m_transRoot);
			result = newObj.GetComponent<Image>();
			m_listImage.Add(result);
		}
		return result;
	}

	public void SetText(string strText)
	{
		bool isNum = false; // 是否数字;
		bool isSpecial = false;
		int nSpecial = 0; // 是否特殊符号;
		float nUseLen = 0;

		char[] arrChar = strText.ToCharArray();
		for ( int i = 0 ; i < arrChar.Length ; i++ )
		{
			isNum = (arrChar[i] >= 48 && arrChar[i] <= 57);

			nSpecial = m_listChars.IndexOf(arrChar[i]);
			isSpecial = nSpecial != -1;
			if (isNum || isSpecial)
			{
				Image img = GetImg(i);

				string strKey = arrChar[i].ToString();
				if (isSpecial)
				{
					nUseLen += m_nSpecialOffset;
					strKey = m_listSpriteKeys[nSpecial].ToString();
				}
				Debug.Log(strKey);
				Sprite sprite = m_dicSprites[strKey];

				img.sprite = sprite;

				img.transform.localPosition = new Vector3(nUseLen, 0);

				nUseLen += sprite.rect.width;
				nUseLen += m_nSpaceNormal;

				if (isSpecial)
				{
					nUseLen += m_nSpecialOffset;
				}
			}
			else
			{
				Debug.LogError("UI_SpriteText SetText, out of range : " + arrChar[i]);
			}
		}
	}

	void Update()
    {
    }
}
