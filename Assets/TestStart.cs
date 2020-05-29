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
    }
    // Update is called once per frame

}
