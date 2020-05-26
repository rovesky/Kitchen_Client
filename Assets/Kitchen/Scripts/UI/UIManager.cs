using System;
using System.Collections.Generic;
using FootStone.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public Canvas m_canvas_main;

        private Button m_button1;
        private Button m_button2;
        private Button m_button3;
        private UI_SpriteText textTimePlaying;
        private UI_SpriteText textTimePreparing;
        private UI_SpriteText textScore;
        private UI_TaskListCtrl taskList;
        private CanvasGroup canvasGroup;
        private GameObject pannelButton;
        private GameObject pannelMain;
        private GameObject pannelStart;
        private GameObject pannelEnd;
        private GameObject joystick;
        private Text m_text_rtt;
        private Text m_text_fps;

        private Dictionary<string,Sprite>  buttonIcons = new Dictionary<string, Sprite>();

        private void Start()
        {
            Debug.Log("GameManager Start!");

            Instance = this;
            var pannelInfo = m_canvas_main.transform.Find("PannelInfo");
            m_text_rtt = pannelInfo.Find("text_rtt").GetComponent<Text>();
            m_text_fps = pannelInfo.Find("text_fps").GetComponent<Text>();

            m_button1 = m_canvas_main.transform.Find("PannelButton/Button1").GetComponent<Button>();
            m_button2 = m_canvas_main.transform.Find("PannelButton/Button2").GetComponent<Button>();
         //   m_button2.interactable = false;
            m_button3 = m_canvas_main.transform.Find("PannelButton/Button3/Btn_Main").GetComponent<Button>();


            textTimePlaying = m_canvas_main.transform.Find("PannelMain/TextTime").GetComponent<UI_SpriteText>();

            textTimePreparing = m_canvas_main.transform.Find("PannelStart/TextTime").GetComponent<UI_SpriteText>();

            textScore = m_canvas_main.transform.Find("PannelMain/TextScore").GetComponent<UI_SpriteText>();

            taskList = m_canvas_main.transform.Find("UI_TaskListCtrl").GetComponent<UI_TaskListCtrl>();

            canvasGroup = m_canvas_main.GetComponent<CanvasGroup>();

            pannelButton = m_canvas_main.transform.Find("PannelButton").gameObject;
            pannelMain = m_canvas_main.transform.Find("PannelMain").gameObject;
            pannelStart = m_canvas_main.transform.Find("PannelStart").gameObject;
            pannelEnd = m_canvas_main.transform.Find("PannelEnd").gameObject;

            joystick = m_canvas_main.transform.Find("Joystick").gameObject;

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

        public void UpdateButtonIcon(string buttonName, string icon)
        {
            if (buttonName == "Button2")
            {
               var image =  m_button2.transform.GetComponent<Image>();
               var key = "UI/" + icon;
               if(!buttonIcons.ContainsKey(key))
                   buttonIcons.Add(key,Resources.Load<Sprite>(key));
               image.sprite = buttonIcons[key];
            }
        }

        public void ButtonEnable(string buttonName, bool enable)
        {
            if (buttonName == "Button1")
                m_button1.interactable = enable;
            else if (buttonName == "Button2")
                m_button2.interactable = enable;
            else if (buttonName == "Button3")
                m_button3.interactable = enable;
        }

        public void PlayCD(string buttonName, float time)
        {
            if (buttonName == "Button3")
            {
               var countDown =  m_button3.transform.parent.GetComponent<UI_CountDown>();
               countDown.PlayCDView(time);
            }
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


        public GameObject CreateUIFromPrefabs(string name)
        {
            var obj = Instantiate(Resources.Load("UI/Prefab/"+name)) as GameObject;
            obj.transform.SetParent(m_canvas_main.transform);
            obj.transform.localScale = new Vector3(1,1,1);
            obj.transform.SetAsFirstSibling();
            obj.SetActive(false);
            return obj;
        }


        public void UpdateTime(GameState state, ushort timeSecond)
        {
            var timeSpan = new TimeSpan(0, 0, seconds: timeSecond);

            if (state == GameState.Playing)
            {
                var str = timeSpan.ToString(@"mm\:ss");
                //  FSLog.Info($"UpdateTime:{str}");
                textTimePlaying.SetText(str);
            }
            else if (state == GameState.Preparing)
            {
                var str = timeSpan.ToString(@"ss");
              // FSLog.Info($"UpdateTime textTimePreparing:{str}");
                textTimePreparing.SetText(str);
            }
        }

        public void UpdateScore(ushort score)
        {

            var str = score.ToString();
            //  FSLog.Info($"UpdateTime:{str}");
            textScore.SetText(str);
        }

        public void AddMenu(int index, int productId, int material1, int material2, int material3, int material4)
        {
            taskList.InsertTail(index, productId, material1, material2, material3, material4);
        }

        public void RemoveMenu(int index)
        {
            taskList.RemoveAt(index);
        }

        public void ClearMenu()
        {
            taskList.Clear();
        }

        public void EnableGame(bool enable)
        {
            pannelButton.SetActive(enable);
            pannelMain.SetActive(enable);
            joystick.SetActive(enable);
            taskList.gameObject.SetActive(enable);
            canvasGroup.blocksRaycasts = enable;
           
        }

        public void EnablePannelStart(bool enable)
        {
            pannelStart.SetActive(enable);
        }

        public void EnablePannelEnd(bool enable)
        {
            pannelEnd.SetActive(enable);
        }
    }
}