//using UnityEngine;
//using System.Collections;

//namespace Assets.Scripts.ECS
//{
//    [AddComponentMenu("MyGame/EnemySpawn")]
//    public class EnemySpawn : MonoBehaviour
//    {
//        public Transform m_enemy; // ���˵�Prefab
//        protected Transform m_transform;

//        // Use this for initialization
//        void Start()
//        {
//            m_transform = this.transform;
//            StartCoroutine(SpawnEnemy());  // ����Э��
//        }

//        IEnumerator SpawnEnemy() // ʹ��Э�̴�������
//        {
//            yield return new WaitForSeconds(Random.Range(5, 15));  // ÿN������һ������
//            Instantiate(m_enemy, m_transform.position, Quaternion.identity);

//            StartCoroutine(SpawnEnemy());  // ѭ������
//        }

//        void OnDrawGizmos()
//        {
//            Gizmos.DrawIcon(transform.position, "item.png", true);
//        }

//    }
//}
