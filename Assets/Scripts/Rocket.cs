//using UnityEngine;
//using System.Collections;

//[AddComponentMenu("MyGame/Rocket")]
//public class Rocket : MonoBehaviour {

//    public float m_speed = 10;  // �ӵ������ٶ�
//    public float m_liveTime = 1;  // ����ʱ��
//    public float m_power = 1.0f;  // ����
//    protected Transform m_trasform;

//	// Use this for initialization
//	void Start () {

//        m_trasform = this.transform;

//        Destroy(this.gameObject, m_liveTime);  // һ��ʱ�����������
//	}
	
//	// Update is called once per frame
//	void Update () {
//        m_trasform.Translate( new Vector3( 0, 0, -m_speed * Time.deltaTime ) );  // ��ǰ�ƶ�
//	}

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.tag.CompareTo("Enemy")!=0)
//            return;

//        Destroy(this.gameObject);
//    }
//}
