using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public enum NetState
{
    veryGood,
    good,
    bad,

}
public class RightCornerCtr : MonoBehaviour
{

    public GameObject[] NetType = new GameObject[3];
    public Text PingLabel;
    public GameObject CurElc;
    NetState netState;
    StringBuilder sb = new StringBuilder();
    Ping ping;
    bool isNetWorkLose = false;
    void Start()
    {
        SendPing();
        SetNetPicType(99);
    }
    // Update is called once per frame
    void Update()
    {
        if (null != ping && ping.isDone)
        {
            isNetWorkLose = false;
            sb.Remove(0, sb.Length);
            sb.Append(ping.time);
            SetNetPicType(ping.time);
            sb.Append("ms");
            PingLabel.text = sb.ToString();
            ping.DestroyPing();
            ping = null;
            Invoke("SendPing", 1);//每秒Ping一次
        }
    }

    void SendPing()
    {
        ping = new Ping(GameCommon.Instance.CurIp);
    }

    public void SetNetPicType(int curPing)
    {
        if (curPing < 80)
        {
            netState = NetState.veryGood;
        }
        else if (curPing < 250)
        {
            netState = NetState.good;
        }
        else
        {
            netState = NetState.bad;
        }
        if (NetType[0])
        {
            NetType[0].SetActive(netState == NetState.veryGood);
        }
        if (NetType[1])
        {
            NetType[1].SetActive(netState == NetState.good);
        }
        if (NetType[2])
        {
            NetType[2].SetActive(netState == NetState.bad);
        }


    }
}
