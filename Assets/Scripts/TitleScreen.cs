using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyGame/TitleScreen")]
public class TitleScreen : MonoBehaviour
{
    // ��Ӧ��Ϸ��ʼ��ť�¼�
    public void OnButtonGameStart()
    {
        SceneManager.LoadScene("level1");
    }
    
}
