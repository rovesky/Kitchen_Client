using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookUIOnObject : MonoBehaviour
{
	[SerializeField] Transform m_objHook = null;
	[SerializeField] Vector3 m_v3Offset = Vector3.zero;

	private Transform m_transUIObj = null;
	private Canvas m_Canvas = null;
	private Vector3 m_v3OldPos = Vector3.zero;

    void Start()
    {
		m_transUIObj = this.transform;
		m_Canvas = m_transUIObj.transform.GetComponentInParent<Canvas>();

		RefreshPos();
	}

	private void RefreshPos()
	{
		if (m_objHook == null || m_transUIObj == null)
		{
			return;
		}
		m_v3OldPos = m_objHook.position;

		Vector2 pos = WorldToCanvasPoint(m_Canvas, m_transUIObj.parent as RectTransform, m_objHook.position);

		Vector3 uipos = m_transUIObj.localPosition;
		uipos.x = pos.x;
		uipos.y = pos.y;
		uipos += m_v3Offset;
		m_transUIObj.localPosition = uipos;
	}

	private Vector2 WorldToCanvasPoint(Canvas canvas, RectTransform parent, Vector3 worldPos)
	{
		Vector2 pos = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parent,
			Camera.main.WorldToScreenPoint(worldPos), 
			canvas.worldCamera, out pos);
		return pos;
	}

	void Update()
    {
		if (m_v3OldPos.Equals(m_objHook.position))
		{
			return;
		}

		RefreshPos();
	}
}
