using UnityEngine;
using System.Collections;

[AddComponentMenu("MyGame/EnemySpawn")]
public class EnemySpawn : MonoBehaviour
{
    public Transform m_enemy; // 敌人的Prefab
    protected Transform m_transform;

	// Use this for initialization
	void Start () {
        m_transform = this.transform;
        StartCoroutine(SpawnEnemy());  // 启动协程
	}

    IEnumerator SpawnEnemy() // 使用协程创建敌人
    {
        yield return new WaitForSeconds(Random.Range(5,15));  // 每N秒生成一个敌人
        Instantiate(m_enemy, m_transform.position, Quaternion.identity);

        StartCoroutine(SpawnEnemy());  // 循环创建
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon (transform.position, "item.png", true);
    }

}
