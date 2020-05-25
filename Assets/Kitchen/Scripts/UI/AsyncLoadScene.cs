using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Globe
{
    public static string nextSceneName;
}

public class AsyncLoadScene : MonoBehaviour
{
    public Text loadingText;
    public Image progressBar;

    private float curProgressValue = 0;

    private AsyncOperation operation;

    // Use this for initialization
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            //启动协程
            StartCoroutine(AsyncLoading());
            SceneManager.sceneLoaded += CallBack;
        }
    }

    IEnumerator AsyncLoading()
    {
        operation = SceneManager.LoadSceneAsync(Globe.nextSceneName);
        //阻止当加载完成自动切换
        //operation.allowSceneActivation = false;
        while (operation.isDone == false)
        {
            Debug.Log(operation.progress + "<<<<<<<<<<<<<<<<<<<");
            curProgressValue = Mathf.Clamp01(operation.progress / 0.9f );
            progressBar.fillAmount = curProgressValue;
            loadingText.text = curProgressValue * 100 + "%";
            if (curProgressValue * 100 == 100)
            {
                operation.allowSceneActivation = true;//启用自动加载场景  
                loadingText.text = "OK";//文本显示完成OK  

            }
            yield return new WaitForSeconds(0.2f);
        }    
    }

    public void CallBack(Scene scene, LoadSceneMode sceneType)
    {
        Debug.Log(scene.name + "is load complete!");

        if (scene.name == "TempScene")
        {
            PanelManager.Instance.OpenPanel<RoomWindow>("RoomWindow");
        }
    }

}