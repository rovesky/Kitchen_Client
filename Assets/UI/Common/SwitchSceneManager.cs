using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwitchSceneManager : BSingleton<SwitchSceneManager>
{

    public void SceneSwitchCallBack(Scene scene, LoadSceneMode sceneType)
    {
        if (scene.name == "TempScene")
        {
            PanelManager.Instance.OpenPanel<RoomWindow>("RoomWindow");
        }

        if (scene.name == CommonDef.kitchen_01)
        {

        }



    }
}
