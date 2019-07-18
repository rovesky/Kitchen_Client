using UnityEngine;
using System.Collections;
[AddComponentMenu("MyGame/EnemyRenderer")]
public class EnemyRenderer : MonoBehaviour {

    public Enemy m_enemy;

	// Use this for initialization
	void Start () {
        m_enemy = this.GetComponentInParent<Enemy>();  // 获得Enemy脚本
    }

    void OnBecameVisible()  // 当模型进入屏幕
    {
        m_enemy.m_isActiv = true;  // 更新Enemy脚本状态
        m_enemy.m_renderer = this.GetComponent<Renderer>();  // 使Enemy获得Renderer
    }
}
