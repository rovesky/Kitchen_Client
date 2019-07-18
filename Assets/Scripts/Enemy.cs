using UnityEngine;
using System.Collections;

[AddComponentMenu("MyGame/Enemy")]
public class Enemy : MonoBehaviour {

    public float m_speed = 1;   // 速度
    public float m_life = 10;   // 生命
    protected float m_rotSpeed = 30;    // 旋转速度

    protected Transform m_transform;

    public Transform m_explosionFX;
    public int m_point = 10;

    internal Renderer m_renderer;  // 模型渲染组件
    internal bool m_isActiv = false;  // 是否激活

	// Use this for initialization
	void Start () {
        m_transform = this.transform;
        m_renderer = this.GetComponent<Renderer>(); // 获得模型渲染组件
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

    void OnBecameVisible()  // 当模型进入屏幕
    {
        m_isActiv = true;
    }
	
	// Update is called once per frame
	void Update () {

        UpdateMove();

        if (m_isActiv && !this.m_renderer.isVisible )  // 如果移动到屏幕外
        {
            Destroy(this.gameObject); // 自我销毁
        }
	}

    protected virtual void UpdateMove()
    {
        // 左右移动
        float rx = Mathf.Sin(Time.time) * Time.deltaTime;

        // 前进
        m_transform.Translate(new Vector3(rx, 0, -m_speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("PlayerRocket") == 0) // 如果撞到主角子弹
        {
            Rocket rocket = other.GetComponent<Rocket>();
            if (rocket != null)
            {
                m_life -= rocket.m_power;  // 减少生命

                if (m_life <= 0)
                {
                    GameManager.Instance.AddScore(m_point);  // 更新UI上的分数

                    Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
                    Destroy(this.gameObject);  // 自我销毁
                }
            }
        }
        else if (other.tag.CompareTo("Player") == 0)  // 如果撞到主角
        {
            m_life = 0;
            Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
            Destroy(this.gameObject); // 自我销毁
        }
        
        //if (other.tag.CompareTo("bound") == 0)
        //{
        //    m_life = 0;
        //    Destroy(this.gameObject);
        //}
    }
}
