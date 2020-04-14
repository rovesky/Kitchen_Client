using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSceneIcon : MonoBehaviour
{
	[SerializeField] Image m_preImage = null;

	private List<Image> m_listImage = new List<Image>();

	private Image GetImg(int nIndex)
	{
		Image result = null;
		if (nIndex < m_listImage.Count)
		{
			result = m_listImage[nIndex];
		}
		else
		{
			GameObject newObj = (GameObject)Instantiate(m_preImage.gameObject, this.transform);
			result = newObj.GetComponent<Image>();
			m_listImage.Add(result);
		}
		return result;
	}

	public void SetData(int[] arrIDs)
	{
		float nTemp = 0;
		for (int i = 0; i < arrIDs.Length; i++)
		{
			Image img = GetImg(i);
			img.sprite = Resources.Load("demo_icon_food_dishe" + arrIDs[i], typeof(Sprite)) as Sprite;
			img.SetNativeSize();

			nTemp += img.sprite.rect.width;
		}

		nTemp = 0 - nTemp / 2;

		for (int i = 0; i < arrIDs.Length; i++)
		{
			m_listImage[i].transform.localPosition = new Vector3(nTemp, 0, 0);
			nTemp += m_listImage[i].sprite.rect.width;
			m_listImage[i].gameObject.SetActive(true);
		}

		for (int i = arrIDs.Length; i < m_listImage.Count; i++)
		{
			m_listImage[i].gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			SetData(new int[] { 1, 2 });
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			SetData(new int[] { 1 });
		}
	}
}
