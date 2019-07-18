using UnityEngine;
using System.Collections;

[AddComponentMenu("MyGame/Rocket")]
public class Rocket : MonoBehaviour {

    public float m_speed = 10;  // 子弹飞行速度
    public float m_liveTime = 1;  // 生存时间
    public float m_power = 1.0f;  // 威力
    protected Transform m_trasform;

	// Use this for initialization
	void Start () {

        m_trasform = this.transform;

        Destroy(this.gameObject, m_liveTime);  // 一定时间后自我销毁
	}
	
	// Update is called once per frame
	void Update () {
        m_trasform.Translate( new Vector3( 0, 0, -m_speed * Time.deltaTime ) );  // 向前移动
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Enemy")!=0)
            return;

        Destroy(this.gameObject);
    }
}
