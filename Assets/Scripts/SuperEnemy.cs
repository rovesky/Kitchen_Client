//using UnityEngine;
//using System.Collections;

//[AddComponentMenu("MyGame/SuperEnemy")]
//public class SuperEnemy : Enemy {

//    public Transform m_rocket;  // �ӵ�Prefab
//    protected float m_fireTimer = 2;  // �����ʱ��
//    protected Transform m_player;  // ����

//    void Awake()
//    {
//        GameObject obj=GameObject.FindGameObjectWithTag("Player"); // ��������
//        if ( obj!=null )
//        {
//            m_player=obj.transform;
//        }
//    }

//    protected override void UpdateMove()
//    {

//        m_fireTimer -= Time.deltaTime;
//        if (m_fireTimer <= 0)
//        {
//            m_fireTimer = 2;  // ÿ2�����һ��
//            if ( m_player!=null )
//            {
//                Vector3 relativePos = m_transform.position-m_player.position;  // �����ӵ���ʼ����ʹ����������
//                Instantiate( m_rocket, m_transform.position, Quaternion.LookRotation(relativePos) ); // �����ӵ�
//            }
//        }
        
//        // ǰ��
//        m_transform.Translate(new Vector3(0, 0, -m_speed * Time.deltaTime));
//    }
//}
