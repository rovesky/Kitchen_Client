using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void Init(params object[] args)
    {

    }
    //开始面板前
    public virtual void OnShowing()
    {

    }
    //显示面板后
    public virtual void OnShowed()
    {

    }
    //帧更新
    public virtual void Update()
    {

    }
    //关闭前
    public virtual void OnClosing()
    {

    }
    //关闭后
    public virtual void OnClosed()
    {

    }
}
