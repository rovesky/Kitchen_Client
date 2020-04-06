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

        private  Slider sliceSlider;

        private void Start()
        {
            Debug.Log("GameManager Start!");

            Instance = this;
            m_text_rtt = m_canvas_main.transform.Find("text_rtt").GetComponent<Text>();
            m_text_fps = m_canvas_main.transform.Find("text_fps").GetComponent<Text>();

            m_button1 = m_canvas_main.transform.Find("button1").GetComponent<Button>();
            m_button2 = m_canvas_main.transform.Find("button2").GetComponent<Button>();
            m_button3 = m_canvas_main.transform.Find("button3").GetComponent<Button>();

            sliceSlider =  m_canvas_main.transform.Find("Slider").GetComponent<Slider>();

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


        public void UpdateSlider(bool isVisible,Vector3 pos, float value)
        {
            sliceSlider.gameObject.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = sliceSlider.gameObject.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            sliceSlider.value = value;
        }
    }
}