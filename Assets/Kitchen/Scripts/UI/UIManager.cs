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
            m_button2 = m_canvas_main.transform.Find("button2").GetComponent<Button>();
            m_button3 = m_canvas_main.transform.Find("button3").GetComponent<Button>();

      

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

       
    }
}