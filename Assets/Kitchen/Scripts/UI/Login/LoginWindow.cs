using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginWindow : PanelBase
{

    private Button Login_Btn;
    private Button Notice_Btn;
    private Text Version_Text;

    public override void Init(params object[] args)
    {
        Login_Btn = transform.Find("Main/Login_Btn").GetComponent<Button>();
        Notice_Btn = transform.Find("Main/Notice_Btn").GetComponent<Button>();
        Version_Text = transform.Find("Main/Notice_Btn").GetComponent<Text>();
        SetListener();
    }

    private void SetListener()
    {
        if (Login_Btn)
        {
            EventTriggerListener.Get(Login_Btn.gameObject).onPointerClick = o =>
            {
                Debug.Log("登录");
                LoginSuccess();
            };
        }
        if(Notice_Btn)
        {
            EventTriggerListener.Get(Notice_Btn.gameObject).onPointerClick = o =>
            {
                Debug.Log("公告");
            };
        }
    }


    public override void OnShowing()
    {
        base.OnShowing();
    }

    public override void OnClosed()
    {
        base.OnClosed();
    }

    public void LoginReqest()
    {

    }

    public void LoginSuccess()
    {
        Debug.Log("Success");
   
        this.gameObject.SetActive(false);
        PanelManager.Instance.ClosePanel("LoginWindow");
        PanelManager.Instance.OpenPanel<MainWindow>("MainWindow");
    }

    public void LoginFail()
    {

    }
}
