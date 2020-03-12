using FootStone.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [AddComponentMenu("MyGame/GameManager")]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        private Button m_button1;
        private Button m_button2;

        public Canvas m_canvas_main;
        private Text m_text_rtt;
        private Text m_text_fps;

        private void Start()
        {
            Debug.Log("GameManager Start!");

            Instance = this;
            m_text_rtt = m_canvas_main.transform.Find("text_rtt").GetComponent<Text>();
            m_text_fps = m_canvas_main.transform.Find("text_fps").GetComponent<Text>();

            m_button1 = m_canvas_main.transform.Find("button1").GetComponent<Button>();
            m_button2 = m_canvas_main.transform.Find("button2").GetComponent<Button>();

            m_button1.onClick.AddListener(() =>
            {
             //   FSLog.Info("m_button1.onClick!");
                UIInput.AddButtonClickEvent("pickup");
            });

            m_button2.onClick.AddListener(() =>
            {
             //   FSLog.Info("m_button2.onClick!");
                UIInput.AddButtonClickEvent("throw");
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
    }
}