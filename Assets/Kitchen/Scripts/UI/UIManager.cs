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
        private Text textTimePlaying;
        private SimpleImgCountDown textTimePreparing;
        private Text textScore;
        private UI_TaskListCtrl taskList;
        private CanvasGroup canvasGroup;
        private GameObject pannelButton;
        private GameObject pannelMain;
        private GameObject pannelStart;
        private GameObject pannelEnd;
        private GameObject joystick;
        private Text m_text_fps;
        private GameObject RightCorner;
        private Text PingTxt;
        public Transform NetStates;
        //private GameObject RightCorner;
        private Dictionary<string,Sprite>  buttonIcons = new Dictionary<string, Sprite>();

        private void Start()
        {
            Debug.Log("GameManager Start!");

            Instance = this;
            pannelButton = m_canvas_main.transform.Find("FightUI/PannelButton").gameObject;
            pannelMain = m_canvas_main.transform.Find("FightUI/PannelMain").gameObject;
            pannelStart = m_canvas_main.transform.Find("FightUI/PannelStart").gameObject;
            pannelEnd = m_canvas_main.transform.Find("FightUI/PannelEnd").gameObject;
            RightCorner = m_canvas_main.transform.Find("FightUI/RightCorner").gameObject;
            if (RightCorner)
            {
                m_text_fps = RightCorner.transform.Find("text_fps").GetComponent<Text>();
                PingTxt = RightCorner.transform.Find("Ping").GetComponent<Text>();
                NetStates = RightCorner.transform.Find("NetState");
            }
            if (pannelButton)
            {
                m_button1 = pannelButton.transform.Find("Button1").GetComponent<Button>();
                m_button2 = pannelButton.transform.Find("Button2").GetComponent<Button>();
                m_button3 = pannelButton.transform.Find("Button3/Btn_Main").GetComponent<Button>();
            }

            if (pannelMain)
            {
                textScore = pannelMain.transform.Find("CurScore_Txt").GetComponent<Text>();
                textTimePlaying = pannelMain.transform.Find("CurTime_Txt").GetComponent<Text>();
            }
            if (pannelStart)
            {
                textTimePreparing = pannelStart.transform.Find("TimeCountDown").GetComponent<SimpleImgCountDown>();

            }          
            taskList = m_canvas_main.transform.Find("FightUI/UI_TaskListCtrl").GetComponent<UI_TaskListCtrl>();
            canvasGroup = m_canvas_main.GetComponent<CanvasGroup>();
            joystick = m_canvas_main.transform.Find("Joystick").gameObject;
            SetListener();
        }


        public void SetListener()
        {
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
        public void StartUI()
        {

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
        private float refreshInterval = 3.0f;
        private float curTime = 3.0f;
        public void UpdateRtt(double rtt)
        {
            if (curTime < refreshInterval)
            {
                curTime += Time.deltaTime;
            }
            else
            {
                curTime = 0.0f;
                PingTxt.text = string.Format("{0}MS", rtt.ToString("#"));
                SetPingImg(rtt);
            }
        }
        NetState netState;
        public void SetPingImg(double rtt)
        {
            if (rtt < 80)
            {
                netState = NetState.veryGood;
            }
            else if (rtt < 250)
            {
                netState = NetState.good;
            }
            else
            {
                netState = NetState.bad;
            }


            if (NetStates.Find("Full_Img"))
            {
                NetStates.Find("Full_Img").gameObject.SetActive(netState == NetState.veryGood);
            }
            if (NetStates.Find("Half_Img"))
            {
                NetStates.Find("Half_Img").gameObject.SetActive(netState == NetState.good);
            }
            if (NetStates.Find("None_Img"))
            {
                NetStates.Find("None_Img").gameObject.SetActive(netState == NetState.bad);
            }
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
                pannelMain.SetActive(true);
                var str = timeSpan.ToString(@"mm\:ss");
                //  FSLog.Info($"UpdateTime:{str}");
                textTimePlaying.text = str;
            }
            else if (state == GameState.Preparing)
            {
                var str = timeSpan.ToString(@"ss");
                // FSLog.Info($"UpdateTime textTimePreparing:{str}");
                textTimePreparing.SetNum(int.Parse(str));
            }
        }

        public void UpdateScore(ushort score)
        {
            if(!pannelMain.activeSelf)
                return;
          
            var str = score.ToString();
            //  FSLog.Info($"UpdateTime:{str}");
            textScore.text = str;
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