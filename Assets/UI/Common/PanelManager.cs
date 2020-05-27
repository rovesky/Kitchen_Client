using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PanelManager : BSingleton<PanelManager>
{

    //public GameObject UIROOT;
    private Dictionary<string, PanelBase> mPanelDic = new Dictionary<string, PanelBase>();
    public void OpenPanel<T>(string panelName, params object[] args) where T : PanelBase
    {

        if (GameCommon.Instance.UIRoot == null)
        {
            GameCommon.Instance.InitUiRoot();
        }
        var name = typeof(T).ToString();
        
        if (mPanelDic.ContainsKey(name))
        {
            return;
        }
        else
        {
           var mWndObject = GameObject.Instantiate(Resources.Load(CommonDef.UIRootPath + panelName)) as GameObject;
            if (mWndObject != null)
            {
                
                mWndObject.transform.SetParent(GameCommon.Instance.UIRoot.transform, false);
                var value = mWndObject.AddComponent<T>();
                mPanelDic.Add(name,value);
                mWndObject.GetComponent<T>().Init(args);
                mWndObject.GetComponent<T>().OnShowed();
            }
            else
            {
                Debug.Log("资源丢了或者参数不对~~~~ ");
            }
     
        }  
    }

    public void MsgRequestRef(string panelName)
    {
        PanelBase panel = (PanelBase)mPanelDic[panelName];
        if (panel == null)
            return;
        panel.Refresh();
    }
    public void ClosePanel(string name)
    {
        PanelBase panel = (PanelBase)mPanelDic[name];
        if (panel == null)
            return;
        panel.OnClosing();
        mPanelDic.Remove(name);
        panel.OnClosed();
        GameObject.Destroy(panel.gameObject);
        Component.Destroy(panel);
    }

    public void CloseAllWindow()
    {
        using (var e = mPanelDic.GetEnumerator())
        {
            while (e.MoveNext())
            {
                e.Current.Value.OnClosed();
                GameObject.Destroy(e.Current.Value.gameObject);
                Component.Destroy(e.Current.Value);
            }
        }
        mPanelDic.Clear();
    }
}

