using UnityEngine;
using System.Collections;

[AddComponentMenu("MyGame/Enemy")]
public class Enemy : MonoBehaviour {

    public float m_speed = 1;   // �ٶ�
    public float m_life = 10;   // ����
    protected float m_rotSpeed = 30;    // ��ת�ٶ�

    protected Transform m_transform;

    public Transform m_explosionFX;
    public int m_point = 10;

    internal Renderer m_renderer;  // ģ����Ⱦ���
    internal bool m_isActiv = false;  // �Ƿ񼤻�

	// Use this for initialization
	void Start () {
        m_transform = this.transform;
        m_renderer = this.GetComponent<Renderer>(); // ���ģ����Ⱦ���
        if ( m_renderer==null )
        {
            var rs = this.GetComponentsInChildren<Renderer>();
            foreach ( var render in rs)
            {
                if (!render.name.StartsWith("col"))
                {
                    m_renderer = render;
                    break;
                }
            }
        }

    }

    void OnBecameVisible()  // ��ģ�ͽ�����Ļ
    {
        m_isActiv = true;
    }
	
	// Update is called once per frame
	void Update () {

        UpdateMove();

        if (m_isActiv && !this.m_renderer.isVisible )  // ����ƶ�����Ļ��
        {
            Destroy(this.gameObject); // ��������
        }
	}

    protected virtual void UpdateMove()
    {
        // �����ƶ�
        float rx = Mathf.Sin(Time.time) * Time.deltaTime;

        // ǰ��
        m_transform.Translate(new Vector3(rx, 0, -m_speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("PlayerRocket") == 0) // ���ײ�������ӵ�
        {
            Rocket rocket = other.GetComponent<Rocket>();
            if (rocket != null)
            {
                m_life -= rocket.m_power;  // ��������

                if (m_life <= 0)
                {
                    GameManager.Instance.AddScore(m_point);  // ����UI�ϵķ���

                    Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
                    Destroy(this.gameObject);  // ��������
                }
            }
        }
        else if (other.tag.CompareTo("Player") == 0)  // ���ײ������
        {
            m_life = 0;
            Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
            Destroy(this.gameObject); // ��������
        }
        
        //if (other.tag.CompareTo("bound") == 0)
        //{
        //    m_life = 0;
        //    Destroy(this.gameObject);
        //}
    }
}
