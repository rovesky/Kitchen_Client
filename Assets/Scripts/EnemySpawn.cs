//using UnityEngine;
//using System.Collections;
//using Unity.Entities;
//using Unity.Transforms;

//[AddComponentMenu("MyGame/EnemySpawn")]
//public class EnemySpawn : MonoBehaviour
//{
//    public GameObject m_enemy; // ���˵�Prefab
//   // protected Transform m_transform;

//	// Use this for initialization
//	void Start () {
//        //m_transform = this.transform;
//        StartCoroutine(SpawnEnemy());  // ����Э��
      
//    }

//    IEnumerator SpawnEnemy() // ʹ��Э�̴�������
//    {
//        yield return new WaitForSeconds(Random.Range(5, 15));  // ÿN������һ������
//                                                               //   Instantiate(m_enemy, m_transform.position, Quaternion.identity);

//        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_enemy, World.Active);
//        var entityManager = World.Active.EntityManager;


//        // Efficiently instantiate a bunch of entities from the already converted entity prefab
//        var instance = entityManager.Instantiate(prefab);

//        // Place the instantiated entity in a grid with some noise
//      //  var position = transform.TransformPoint(new float3(x - CountX / 2, noise.cnoise(new float2(x, y) * 0.21F) * 10, y - CountY / 2));
//        entityManager.SetComponentData(instance, new Translation() { Value = this.transform.position });
//      //  entityManager.AddComponentData(instance, new MoveUp());
//      //  entityManager.AddComponentData(instance, new MovingCube());



//        StartCoroutine(SpawnEnemy());  // ѭ������
//    }

//    void OnDrawGizmos()
//    {
//        Gizmos.DrawIcon (transform.position, "item.png", true);
//    }

//}
