using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SampleClient;

public class GameCommon : MonoBehaviour
{
    public GameObject UIRoot;
    private string HeadIconPath = "UI/HeadIcon";
    private string UIRootPath = "UI/Prefab";
    private static GameCommon m_this = null;
    public string[] TestName = new string[6] { "德玛西亚", "艾欧尼亚", "诺克萨斯", "祖安", "均衡教派", "恕瑞玛" };
    public AudioManager AudioManager;
    public string RoomSettingDialog = "RoomSettingDialog";
    public string LoginWindow = "LoginWindow";
    public string MainWindow = "MainWindow";
    public string EnterRoomDialog = "EnterRoomDialog";
    public string RoomWindow = "RoomWindow";
    public List<Sprite> CellBgList = new List<Sprite>();
    public bool MusicOn;

    public string CurIp;

    //public string 
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
        Init();
    }
    // Start is called before the first frame update

    private void Init()
    {
        InitPlayerPrefs();
        Messenger<string>.AddListener(MessengerEventDef.REFRESH_UI, PanelManager.Instance.MsgRequestRef);
        NetworkNew.Instance.Init(CurIp, 4061);
        DataManager.Instance.Init();
        InitUiRoot();
        AudioManager.GetComponent<AudioManager>().Init();
        if (PlayerPrefsCtrl.Instance.GetBoolValue(CommonDef.IsQuiet))
        {
            AudioManager.GetComponent<AudioManager>().OnVolumeChange(0.0f);
        }
        else
        {
            AudioManager.GetComponent<AudioManager>().OnVolumeChange(PlayerPrefsCtrl.Instance.GetFloatValue(CommonDef.MusicValue));
        }

        SceneDataManager.Instance.SetDalyTime(0);
        PanelManager.Instance.OpenPanel<LoginWindow>("LoginWindow", null);
    }
    

    public void InitPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(CommonDef.MusicValue))
        {
            PlayerPrefs.SetFloat(CommonDef.MusicValue, 0.5f);
        }
        if (!PlayerPrefs.HasKey(CommonDef.SoundEffectValue))
        {
            PlayerPrefs.SetFloat(CommonDef.SoundEffectValue, 0.5f);
        }
        if (!PlayerPrefs.HasKey(CommonDef.IsQuiet))
        {
            PlayerPrefs.SetInt(CommonDef.SoundEffectValue, 1);
        }
        PlayerPrefs.Save();
    }

    public void InitUiRoot()
    {
        if (UIRoot == null)
        {
            var uiRoot = Resources.Load<GameObject>("UI/Prefab/UI_Root");
            if (uiRoot != null)
            {
                var obj = GameObject.Instantiate(uiRoot);
                UIRoot = obj.transform.Find("Canvas").gameObject;
            }
        }
    }

    private void OnDestroy()
    {
        //Messenger<string>.RemoveListener(MessengerEventDef.REFRESH_UI, PanelManager.Instance.MsgRequestRef);
    }


}
