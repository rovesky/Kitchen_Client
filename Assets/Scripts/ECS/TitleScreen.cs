using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyGame/TitleScreen")]
public class TitleScreen : MonoBehaviour
{
    // ��Ӧ��Ϸ��ʼ��ť�¼�
    public void OnButtonGameStart()
    {
       SceneManager.LoadScene("level1");

     //   Scene s1 = SceneManager.CreateScene("level1");
     //   SceneManager.LoadScene("level1",LoadSceneMode.Additive);
    }
    
}
