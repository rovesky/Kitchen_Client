using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleImgCountDown : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> NumImgList;
    [SerializeField]
    public Image TargetImg;

    private int curNum = 0;
    public void SetNum(int num)
    {       
        if (curNum == num)
        {
            return;
        }
        curNum = num;
        Debug.Log(num);
        var count = NumImgList.Count;
        if (num > count)
        {
            return;
        }
        if (TargetImg)
        {
            TargetImg.sprite = NumImgList[num - 1];
        }

    }
}
