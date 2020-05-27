using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        yield return new WaitForSeconds(0.5f);
        Main.gameObject.SetActive(true);
    }
    // Update is called once per frame

}
