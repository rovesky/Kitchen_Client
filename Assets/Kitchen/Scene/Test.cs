using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneDataManager.nextSceneName = "kitchen_01";//目标场景名称
            SceneManager.LoadScene("Loading");//加载进度条场景
        }
    }
}
