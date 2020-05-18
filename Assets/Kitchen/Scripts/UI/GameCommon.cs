using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCommon : MonoBehaviour
{
    public GameObject UIRoot;

    public string HeadIconPath = "UI/HeadIcon";
    public string UIRootPath = "UI/";
    private static GameCommon m_this = null;
    public string[] TestName = new string[6] {"德玛西亚","艾欧尼亚","诺克萨斯","祖安","均衡教派","恕瑞玛"};

    public string RoomSettingDialog = "RoomSettingDialog";
    public string LoginWindow = "LoginWindow";
    public string MainWindow = "MainWindow";
    public static GameCommon Instance
    {
        get
        {
            return m_this;
        }
    }
    void Awake()
    {
        m_this = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();       
    }

    private void Init()
    {

        if (UIRoot == null)
        {
            var uiRoot = Resources.Load<GameObject>("UI/UI_Root");
            if (uiRoot != null)
            {
                var obj = GameObject.Instantiate(uiRoot);
                UIRoot = obj.transform.Find("Canvas").gameObject ;
            }           
        }
        PanelManager.Instance.OpenPanel<LoginWindow>("LoginWindow", null);
    }
}
