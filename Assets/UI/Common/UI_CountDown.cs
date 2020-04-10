using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CountDown : MonoBehaviour
{
	[SerializeField] Image m_picFrontGround = null;

	private Button m_btnCtrl = null;

	private float m_nRunTime = 0;
	private float m_nMaxTime = 0;
	
	private bool m_isPlaying = false;

	private float m_nTemp = 0;

	public void PlayCDView(float nCDTime)
	{
		m_btnCtrl.enabled = false;

		m_nMaxTime = nCDTime;
		m_nRunTime = 0;
		m_isPlaying = true;
	}

	void Start()
	{
		m_btnCtrl = m_picFrontGround.GetComponent<Button>();

		PlayCDView(5);
	}

	void Update()
    {
		if (!m_isPlaying)
		{
			return;
		}
		m_nTemp = m_nRunTime / m_nMaxTime;
		m_nRunTime += Time.deltaTime;

		if (m_nTemp >= 1)
		{
			m_isPlaying = false;
			m_nTemp = 1;
			m_btnCtrl.enabled = true;
		}
		m_picFrontGround.fillAmount = m_nTemp;

	}
}
