using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TaskListCtrl : MonoBehaviour
{
	[SerializeField] UI_TaskListItem m_preItem = null;
	[SerializeField] Transform m_rootTrans = null;

	[SerializeField] float m_nPrepareLength = 0;    // 动态效果距离

	[SerializeField] float m_nItemSpace = 0;    // 每一项间隔距离;

	[SerializeField] float m_nAniTimeLen = 0.5f;

	private Queue<UI_TaskListItem> m_listFrees = new Queue<UI_TaskListItem>();

	private List<int> m_listSort = new List<int>();
	private Dictionary<int, UI_TaskListItem> m_dicUsing = new Dictionary<int, UI_TaskListItem>();

	private bool m_isLockAni = false;
	private List<int> m_listAdd = new List<int>();
	private List<int> m_listRemove = new List<int>();	// 移出列表;
	private int m_nRemoveIndex = -1;                    // 是否有重新排动画 or 下标位置 之后的，都要参与动画;
	private List<bool> m_listWaitting = null;
	
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

		return result;
	}

	void Start()
	{
	}

	public void RemoveAt(int nIndex)
	{
		if (m_dicUsing.Count > 0)
		{
			if (m_dicUsing.ContainsKey(nIndex))
			{
				m_listRemove.Add(nIndex);	// 添加一个任务;
			}
			else
			{
				Debug.LogError("UI_TaskListCtrl RemoveAt, m_dicUsing not ContainsKey : " + nIndex);
			}
		}
		else
		{
			Debug.LogError("UI_TaskListCtrl RemoveAt, m_dicUsing's count Error : " + m_dicUsing.Count);
		}
	}

	public void Clear()
	{
		m_listSort.Clear();
		foreach ( KeyValuePair<int, UI_TaskListItem> kv in m_dicUsing)
		{
			kv.Value.Stop();
			kv.Value.gameObject.SetActive(false);
			m_listFrees.Enqueue(kv.Value);
		}
		m_dicUsing.Clear();

		m_isLockAni = false;
		m_listAdd.Clear();
		m_listRemove.Clear();
		m_nRemoveIndex = -1;
		m_listWaitting = null;
	}

	/// <summary>
	/// 添加一个项目
	/// </summary>
	/// <param name="nIndex">唯一ID</param>
    public void InsertTail(int nIndex, 
		int productId, int material1,
        int material2 = 0, int material3 = 0, int material4 = 0)
    {
		if (m_dicUsing.ContainsKey(nIndex))
		{
			Debug.LogError("UI_TaskListCtrl InsertTail, already has key : " + nIndex);
			return;
		}

        UI_TaskListItem item = GetItem();
		m_dicUsing.Add(nIndex, item);
		m_listSort.Add(nIndex);

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
		m_listAdd.Add(nIndex);
    }

	void Update()
	{
		// 向上补空缺的动画部分;
		if (m_nRemoveIndex != -1)
		{
			if (m_listWaitting == null)
			{
				m_listWaitting = new List<bool>();
				for (int i = m_nRemoveIndex; i < m_listSort.Count; i++)
				{
					if (!m_dicUsing.ContainsKey(m_listSort[i]))
					{
						Debug.LogError("UI_TaskListCtrl UpdatePlayAni, Error key : " + m_listSort[i]);
						continue;
					}
					m_listWaitting.Add(false);
					int nIndex = m_listWaitting.Count - 1;
					UI_TaskListItem item = m_dicUsing[m_listSort[i]];
					item.Play((m_nItemSpace + m_preItem.BGHeight), m_nAniTimeLen, () =>
					{
						m_listWaitting[nIndex] = true;
					});
				}
			}
			else
			{
				if (!m_listWaitting.Contains(false))
				{
					m_nRemoveIndex = -1;
					m_listWaitting.Clear();
					m_listWaitting = null;
				}
			}
			return;
		}

		if (m_isLockAni)
		{
			return;
		}

		// 移出一个元素的部分;
		if (m_listRemove.Count > 0)
		{
			m_isLockAni = true;
			//取第一个元素;
			int nIndex = m_listRemove[0];
			m_listRemove.RemoveAt(0);

			// 开始移出操作;
			UI_TaskListItem item = m_dicUsing[nIndex];
			item.Stop();

			m_nRemoveIndex = m_listSort.IndexOf(nIndex);    // 记录下标位置;
			m_listSort.Remove(nIndex);

			item.PlayOut(
				() =>
				{
					item.gameObject.SetActive(false);
					m_dicUsing.Remove(nIndex);
					// 结束后 回收item;
					m_listFrees.Enqueue(item);
					m_isLockAni = false;
				});
			return;
		}

		//加入一个新项目;
		if (m_listAdd.Count > 0)
		{
			m_isLockAni = true;
			//取第一个元素;
			int nIndex = m_listAdd[0];
			m_listAdd.RemoveAt(0);

			Vector3 v3LastPos = Vector3.zero;
			if (m_listSort.Count > 1)
			{
				int nSortIndex = m_listSort.IndexOf(nIndex);
				if (nSortIndex > 0)
				{
					nSortIndex--;
					int nEnd = m_listSort[nSortIndex];
					v3LastPos = m_dicUsing[nEnd].transform.localPosition;
					v3LastPos.y -= (m_nItemSpace + m_preItem.BGHeight);
				}
			}

			// 开始進入操作;
			UI_TaskListItem item = m_dicUsing[nIndex];
			item.gameObject.SetActive(true);

			Vector3 v3Temp = v3LastPos;
			v3Temp.y -= m_nPrepareLength;
			item.transform.localPosition = v3Temp;
			item.Play(v3Temp, v3LastPos, m_nAniTimeLen, () =>
			{
				m_isLockAni = false;
			});
		}
	}
}
