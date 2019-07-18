using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyGame/GameManager")]
public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Transform m_canvas_main;  // 显示分数的UI界面
    public Transform m_canvas_gameover;  // 游戏失败UI界面
    public Text m_text_score;  // 得分UI文字
    public Text m_text_best;  // 最高分UI文字
    public Text m_text_life;  // 生命UI文字

    protected int m_score = 0; //得分
    public static int m_hiscore = 0;  //最高分
    protected Player m_player; //主角
    
    public AudioClip m_musicClip;  // 背景音乐
    protected AudioSource m_Audio;  // 声音源

    void Start () {

        Instance = this;

        m_Audio = this.gameObject.AddComponent<AudioSource>();  // 使用代码添加音效组件
        m_Audio.clip = m_musicClip;
        m_Audio.loop = true;
        m_Audio.Play();
        
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); // 获取主角

        m_text_score = m_canvas_main.transform.Find("Text_score").GetComponent<Text>();  // 获得Ui控件
        m_text_best = m_canvas_main.transform.Find("Text_best").GetComponent<Text>();
        m_text_life = m_canvas_main.transform.Find("Text_life").GetComponent<Text>();
        m_text_score.text = string.Format("分数  {0}", m_score); // 初始化UI分数
        m_text_best.text = string.Format("最高分 {0}", m_hiscore); // 初始化UI最高分
        m_text_life.text = string.Format("生命 {0}", m_player.m_life); // 初始化UI生命值

        var restart_button = m_canvas_gameover.transform.Find("Button_restart").GetComponent<Button>();  // 获取重新开始游戏按钮
        restart_button.onClick.AddListener(delegate ()  // 按钮事件回调
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // 重新开始当前关卡
        });
        m_canvas_gameover.gameObject.SetActive(false);  // 默认隐藏游戏失败UI
    }
 
    // 增加分数
    public void AddScore( int point )
    {
        m_score += point;

        // 更新高分纪录
        if (m_hiscore < m_score)
            m_hiscore = m_score;
        m_text_score.text = string.Format("分数  {0}", m_score);
        m_text_best.text = string.Format("最高分 {0}", m_hiscore);
    }

    // 改变生命值UI显示
    public void ChangeLife(int life)
    {
        m_text_life.text = string.Format("生命 {0}", life);  // 更新UI
        if ( life<=0)
        {
            m_canvas_gameover.gameObject.SetActive(true); // 如果生命为0，显示游戏失败UI
        }
    }
}
