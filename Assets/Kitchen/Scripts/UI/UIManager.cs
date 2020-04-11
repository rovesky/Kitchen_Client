using System;
using FootStone.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
  //  [AddComponentMenu("MyGame/GameManager")]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public Canvas m_canvas_main;
      //  public Canvas m_canvas_slider;
      //  public int a;


        private Button m_button1;
        private Button m_button2;
        private Button m_button3;
        private UI_SpriteText textTime;
        private UI_SpriteText textScore;
        private UI_TaskListCtrl taskList;
        private Text m_text_rtt;
        private Text m_text_fps;
      

        private void Start()
        {
            Debug.Log("GameManager Start!");

            Instance = this;
            var pannelInfo = m_canvas_main.transform.Find("PannelInfo");
            m_text_rtt = pannelInfo.Find("text_rtt").GetComponent<Text>();
            m_text_fps = pannelInfo.Find("text_fps").GetComponent<Text>();

            m_button1 = m_canvas_main.transform.Find("button1").GetComponent<Button>();
            m_button2 = m_canvas_main.transform.Find("Btn_Drop/Btn_Main").GetComponent<Button>();
            m_button3 = m_canvas_main.transform.Find("Btn_Pick/Btn_Main").GetComponent<Button>();

       //     var obj = m_canvas_main.transform.Find("PannelMain/TextTime");
         //   FSLog.Info($"TextTime Obj：{obj}");
            textTime = m_canvas_main.transform.Find("PannelMain/TextTime").GetComponent<UI_SpriteText>();
          //  FSLog.Info($"TextTime：{textTime}");
            textScore = m_canvas_main.transform.Find("PannelMain/TextScore").GetComponent<UI_SpriteText>();

            taskList = m_canvas_main.transform.Find("UI_TaskListCtrl").GetComponent<UI_TaskListCtrl>();
        //    taskList.InsertTail(1,1);
         //   taskList.InsertTail(2,1,2);

            m_button1.onClick.AddListener(() =>
            {
                FSLog.Info("pickup.onClick!");
                UIInput.AddButtonClickEvent("pickup");
            });

            m_button2.onClick.AddListener(() =>
            {
             //   FSLog.Info("m_button2.onClick!");
                UIInput.AddButtonClickEvent("throw");
            });

            m_button3.onClick.AddListener(() =>
            {
                FSLog.Info("m_button3.onClick!");
                UIInput.AddButtonClickEvent("rush");
            });
        }

        // 改变RTT UI显示
        public void UpdateRtt(double rtt)
        {
            m_text_rtt.text = rtt.ToString("#.00");
        }

        public void UpdateFps(double fps)
        {
            m_text_fps.text = fps.ToString("#.00");
        }

        public GameObject CreateProgess()
        {
            var slider = Instantiate(Resources.Load("Progress")) as GameObject;
            slider.transform.parent = m_canvas_main.transform;
            slider.transform.localScale = m_button1.transform.localScale;
            return slider;
        }

        public void UpdateTime(ushort timeSecond)
        {
           //ar endTime = DateTime.Now.AddSeconds(timeSecond);
            var timeSpan = new TimeSpan(0, 0, seconds: timeSecond);
            var str = timeSpan.ToString(@"mm\:ss");
          //  FSLog.Info($"UpdateTime:{str}");
            textTime.SetText(str);
        }
       
        public void AddMenu(int productId,int material1,int material2,int material3,int material4)
        {
            taskList.InsertTail(productId,material1,material2,material3,material4);
        }
    }
}