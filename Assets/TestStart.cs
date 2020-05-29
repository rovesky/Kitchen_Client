using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FootStone.Kitchen;
public class TestStart : MonoBehaviour
{
    public GameObject Main;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartMain");
    }

    IEnumerator StartMain()
    {
        yield return new WaitForEndOfFrame();
        Main.GetComponent<MainBehaviour>().StartGame();
        GameCommon.Instance.AudioManager.PlayBackground(GameCommon.Instance.AudioManager.ClipArray[2]);
    }
    // Update is called once per frame

}
