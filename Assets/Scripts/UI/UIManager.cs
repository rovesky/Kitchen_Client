using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.ECS;
using Unity.Entities;

namespace Assets.Scripts.ECS
{

    [AddComponentMenu("MyGame/GameManager")]
    public class UIManager : MonoBehaviour
    {

        public static UIManager Instance;

        public Canvas m_canvas_main;
        private Text m_text_rtt;
        private Button m_button1;
      
        void Start()
        {
            Debug.Log("GameManager Start!");
     
            Instance = this;          
            m_text_rtt = m_canvas_main.transform.Find("text_rtt").GetComponent<Text>();
            m_button1 = m_canvas_main.transform.Find("button1").GetComponent<Button>();

            m_button1.onClick.AddListener(() =>
            {
                FSLog.Info("m_button1.onClick!");
                UIInput.AddButtonClickEvent("button1");

            });
        }

        // 改变RTT UI显示
        public void UpdateRtt(double rtt)
        {
            m_text_rtt.text =  rtt.ToString("#.00");
        }
    }

}