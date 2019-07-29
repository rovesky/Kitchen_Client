//using UnityEngine;
//using System.Collections;

//[AddComponentMenu("MyGame/Player")]
//public class Player : MonoBehaviour {

//    // 速度
//    public float m_speed = 1;
//    // 生命
//    public int m_life = 3;

//    // 子弹prefab
//    public Transform m_rocket;
//    protected Transform m_transform;

//    // 发射子弹计时器
//    float m_rocketTimer = 0;
    
//    public AudioClip m_shootClip;  // 声音
//    protected AudioSource m_audio;  // 声音源
//    public Transform m_explosionFX;  // 爆炸特效

    
//    protected Vector3 m_targetPos; // 目标位置
//    public LayerMask m_inputMask; // 鼠标射线碰撞层

//    // Use this for initialization
//    void Start () {

//        m_transform = this.transform;
//        m_audio = this.GetComponent<AudioSource>();

//        m_targetPos = this.m_transform.position;
//	}
	
//	// Update is called once per frame
//	void Update () {

//        // 纵向移动距离
//        float movev=0;

//        // 水平移动距离
//        float moveh=0;
//        /*
//        // 按上键
//        if ( Input.GetKey( KeyCode.UpArrow ) )
//        {
//            movev -= m_speed * Time.deltaTime;
//        }

//        // 按下键
//        if ( Input.GetKey( KeyCode.DownArrow ) )
//        {
//            movev += m_speed * Time.deltaTime;
//        }

//        // 按左键
//        if ( Input.GetKey( KeyCode.LeftArrow ) )
//        {
//            moveh += m_speed * Time.deltaTime;
//        }

//        // 按右键
//        if ( Input.GetKey( KeyCode.RightArrow ) )
//        {
//            moveh -= m_speed * Time.deltaTime;
//        }
       
//        // 移动
//        this.m_transform.Translate( new Vector3( moveh, 0, movev ) );
//        */
//        MoveTo();
//        // ...
//        m_rocketTimer -= Time.deltaTime;
//        if ( m_rocketTimer <= 0 )
//        {
//            m_rocketTimer = 0.1f;
            
//            // 按空格键或鼠标左键发射子弹
//            if ( Input.GetKey( KeyCode.Space ) || Input.GetMouseButton(0) )
//            {
//                Instantiate( m_rocket, m_transform.position, m_transform.rotation );

//                // 播放射击声音
//                m_audio.PlayOneShot(m_shootClip);
//            }
//        }

//	}

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.tag.CompareTo("PlayerRocket") != 0)  // 如果与主角子弹以外的碰撞体相撞
//        {
//            m_life -= 1;  // 减少生命

//            GameManager.Instance.ChangeLife(m_life);  // 更新UI

//            if (m_life <= 0) 
//            {
//                // 当生命为0后，播放爆炸特效
//                Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);

//                Destroy(this.gameObject);  // 自我销毁
//            }
//        }
//    }

//    void MoveTo()
//    {
//        if (Input.GetMouseButton(0))
//        {
//            // 获得鼠标屏幕位置
//            Vector3 ms = Input.mousePosition;
//            // 将屏幕位置转为射线
//            Ray ray = Camera.main.ScreenPointToRay(ms);
//            // 用来记录射线碰撞信息
//            RaycastHit hitinfo;
//            // 产生射线
//            //LayerMask mask =new LayerMask();
//            //mask.value = (int)Mathf.Pow(2.0f, (float)LayerMask.NameToLayer("plane"));
//            bool iscast = Physics.Raycast(ray, out hitinfo, 1000, m_inputMask);
//            if (iscast)
//            {
//                // 如果射中目标,记录射线碰撞点
//                m_targetPos = hitinfo.point;
//            }  
//        }

//        // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
//        Vector3 pos = Vector3.MoveTowards(this.m_transform.position, m_targetPos, m_speed * Time.deltaTime);
//        // 更新当前位置
//        this.m_transform.position = pos;
//    }


//}