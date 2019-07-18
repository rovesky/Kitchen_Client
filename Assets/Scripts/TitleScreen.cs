using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyGame/TitleScreen")]
public class TitleScreen : MonoBehaviour
{
    // 响应游戏开始按钮事件
    public void OnButtonGameStart()
    {
        SceneManager.LoadScene("level1");
    }
    
}
