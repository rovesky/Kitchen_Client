using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TaskListItem : MonoBehaviour
{
	[SerializeField] Image m_picBackGround = null;	// 背景图;
	[SerializeField] Image m_picResult = null;  // 目标结果

	[SerializeField] GridLayoutGroup m_grid = null;	// 排序控件;
	[SerializeField] Image m_preItem = null;    // 合成素材;

	private List<Image> m_listItems = new List<Image>();

	private Vector2 m_nDefaultSize = Vector2.zero;    // 背景原始尺寸;

	public float BGHeight
	{
		get
		{
			return m_picBackGround.rectTransform.sizeDelta.y;
		}
	}

	private void Awake()
	{
		m_nDefaultSize = m_picBackGround.rectTransform.sizeDelta;
	}

	/// <summary>
	/// 初始化;
	/// </summary>
	/// <param name="nResultID">结果</param>
	/// <param name="arrItems">素材</param>
	public void InitItem(int nResultID, int[] arrItems)
	{
		m_picResult.sprite = Resources.Load("demo_icon_food_dishe" + nResultID, typeof(Sprite)) as Sprite;

		for (int i = 0; i < arrItems.Length; i++)
		{
			Image img = null;
			if (i < m_listItems.Count)
			{
				img = m_listItems[i];
			}
			else
			{
				GameObject newObj = (GameObject)Instantiate(m_preItem.gameObject, m_grid.transform);
				img = newObj.GetComponent<Image>();
				m_listItems.Add(img);
			}

			img.sprite = Resources.Load("demo_icon_food_Ingredients" + arrItems[i], typeof(Sprite)) as Sprite; ;
			img.gameObject.SetActive(true);
		}

		for (int i = arrItems.Length; i < m_listItems.Count; i++)
		{
			m_listItems[i].gameObject.SetActive(true);
		}

		ResizeBackGround(arrItems.Length);
	}

	/// <summary>
	/// 重置背景宽度;
	/// </summary>
	private void ResizeBackGround(int nCount)
	{
		RectTransform rcTrans = m_picBackGround.rectTransform;
		Vector2 size = rcTrans.sizeDelta;
		size.x = m_nDefaultSize.x + // 原始宽度
			nCount * m_preItem.rectTransform.sizeDelta.x + // 素材占用宽度
			(nCount - 1) * m_grid.spacing.x;	// 间隔;
		rcTrans.sizeDelta = size;
	}

	private float m_nTimeLen = 0;
	private Vector3 m_v3From = Vector3.zero;
	private Vector3 m_v3To = Vector3.zero;

	private float m_nPctTemp = 0;

	private bool m_isPlaying = false;
	private float m_nRunTime = 0;

	private System.Action m_cbOutFinish = null;

	public void Play(Vector3 v2From, Vector3 v2To, float nTimeLen)
	{
		m_cbOutFinish = null;

		m_nTimeLen = nTimeLen;
		m_v3From = v2From;
		m_v3To = v2To;

		m_nRunTime = 0;
		m_isPlaying = true;
	}

	public void PlayOut(System.Action callBack)
	{
		m_cbOutFinish = callBack;

		m_nTimeLen = 2;
		m_v3To = m_v3From = this.transform.localPosition;
		m_v3To.x -= 150;

		m_nRunTime = 0;
		m_isPlaying = true;
	}

	public void Stop()
	{
		m_isPlaying = false;
	}

	void Update()
	{
		if (!m_isPlaying)
		{
			return;
		}
		if (m_nPctTemp >= 1)
		{
			m_isPlaying = false;
			if (m_cbOutFinish != null)
			{
				m_cbOutFinish();
			}
		}
		m_nPctTemp = m_nRunTime / m_nTimeLen;
		this.transform.localPosition = Vector3.Lerp(m_v3From, m_v3To, m_nPctTemp);
		m_nRunTime += Time.deltaTime;
	}
}
