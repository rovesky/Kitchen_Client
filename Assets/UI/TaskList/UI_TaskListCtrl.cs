using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TaskListCtrl : MonoBehaviour
{
	[SerializeField] UI_TaskListItem m_preItem = null;
	[SerializeField] Transform m_rootTrans = null;

	[SerializeField] float m_nTimeSpace = 0;    // 间隔时间;
	[SerializeField] float m_nPrepareLength = 0;    // 动态效果距离

	[SerializeField] float m_nItemSpace = 0;    // 每一项间隔距离;

	private Queue<UI_TaskListItem> m_listFrees = new Queue<UI_TaskListItem>();
	private List<UI_TaskListItem> m_listUsing = new List<UI_TaskListItem>();

	private float m_nUsingTime = 0; // 计时;

	private float m_nUsingHeight = 0;   // 已经使用的高度;

	private Vector3 m_v3Temp1 = Vector3.zero;
	private Vector3 m_v3Temp2 = Vector3.zero;

	private UI_TaskListItem GetItem()
	{
		UI_TaskListItem result = null;
		if (m_listFrees.Count > 0)
		{
			result = m_listFrees.Dequeue();
		}
		else
		{
			GameObject newObj = (GameObject)Instantiate(m_preItem.gameObject, m_rootTrans);
			UI_TaskListItem item = newObj.GetComponent<UI_TaskListItem>();
			result = item;
		}

		m_listUsing.Add(result);
		return result;
	}

	void Start()
	{
		m_nUsingHeight += m_nItemSpace;
	}

	public void RemoveHead()
	{
		if (m_listUsing.Count > 0)
		{
			UI_TaskListItem item = m_listUsing[0];
			m_listUsing.RemoveAt(0);
			item.Stop();
			item.PlayOut(
				()=> { m_nRemoveCount++; });
			m_listFrees.Enqueue(item);
		}
	}

    public void InsertTail(int productId, int material1,
        int material2 = 0, int material3 = 0, int material4 = 0)
    {
        UI_TaskListItem item = GetItem();

        int[] materials;
        if (material2 == 0)
        {
            materials = new int[] {material1};
        }
        else  if (material3 == 0)
        {
            materials = new int[] {material1,material2};
        }
        else  if (material4 == 0)
        {
            materials = new int[] {material1,material2,material3};
        }
        else 
        {
            materials = new int[] {material1,material2,material3,material4};
        }

        item.InitItem(productId, materials);

        m_v3Temp2 = m_v3Temp1 = new Vector3(0, 0 - m_nUsingHeight, 0);
        m_v3Temp2.y -= m_nPrepareLength;

        item.Play(m_v3Temp2, m_v3Temp1, 1);
     //   m_nUsingTime -= m_nTimeSpace;

        m_nUsingHeight += m_preItem.BGHeight;
        m_nUsingHeight += m_nItemSpace;
    }

    private int m_nRemoveCount = 0;
	private bool m_isPlayRemove = false;
	[SerializeField] float m_nRemoveUseTime = 1;
	private float m_nRemoveRunTime = 0;
	private float m_nTempRemovePct = 0;

	private Vector3 m_v3RootFrom = Vector3.zero;
	private Vector3 m_v3RootTo = Vector3.zero;

	private bool mof = false;
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Z))
		{
			RemoveHead();
		}

		if (!m_isPlayRemove)
		{
			if (m_nRemoveCount > 0)
			{
				m_v3RootTo = m_v3RootFrom = m_rootTrans.localPosition;
				m_v3RootTo.y += (m_nItemSpace + m_preItem.BGHeight);
				m_nRemoveRunTime = 0;
				m_isPlayRemove = true;
			}
		}
		else
		{
			m_nTempRemovePct = m_nRemoveRunTime / m_nRemoveUseTime;
			m_nRemoveRunTime += Time.deltaTime;
			m_rootTrans.localPosition = Vector3.Lerp(m_v3RootFrom, m_v3RootTo, m_nTempRemovePct);
			if (m_nTempRemovePct >= 1)
			{
				m_isPlayRemove = false;
				m_nRemoveCount--;
			}
		}

		////this is a test;
		//if (m_nUsingTime >= m_nTimeSpace)
		//{
		//	UI_TaskListItem item = GetItem();
		//	if (mof)
		//	{
		//		item.InitItem(1, new int[] { 1, 2 });
		//	}
		//	else
		//	{
		//		item.InitItem(1, new int[] { 2, 3 ,1});
		//	}
		//	m_v3Temp2 = m_v3Temp1 = new Vector3(0, 0 - m_nUsingHeight, 0);
		//	m_v3Temp2.y -= m_nPrepareLength;

		//	item.Play(m_v3Temp2, m_v3Temp1, 1);
		//	m_nUsingTime -= m_nTimeSpace;

		//	m_nUsingHeight += m_preItem.BGHeight;
		//	m_nUsingHeight += m_nItemSpace;
		//}
		//m_nUsingTime += Time.deltaTime;
	}
}
