using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.ECS;
using Unity.Entities;

namespace Assets.Scripts.ECS
{

    [AddComponentMenu("MyGame/GameManager")]
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;


        public Canvas m_canvas_main;
        private Text m_text_rtt;
      
        void Start()
        {
            Debug.Log("GameManager Start!");
     
            Instance = this;          
            m_text_rtt = m_canvas_main.transform.Find("text_rtt").GetComponent<Text>();         
        
        }     

        // 改变RTT UI显示
        public void UpdateRtt(double rtt)
        {
            m_text_rtt.text =  rtt.ToString("#.00");

        }
    }

}