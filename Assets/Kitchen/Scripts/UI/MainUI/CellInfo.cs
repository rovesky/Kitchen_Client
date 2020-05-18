using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CellInfo : MonoBehaviour
{

    public GameObject Bg;
    public GameObject RoomLock;
    public GameObject RoomName;
    public GameObject RoomNumber;
    public GameObject RoomType;


    public void InitCell()
    {
        Bg = transform.Find("Bg").gameObject;
        RoomType = transform.Find("RoomType_Text").gameObject;
        RoomNumber = transform.Find("RoomNumber_Text").gameObject;
        RoomName = transform.Find("RoomName_Text").gameObject;
        RoomLock = transform.Find("RoomLock_Img").gameObject;
    }

    public void SetCellInfo(CellMsg msg)
    {
        if (RoomType)
        {

        }

        if (RoomName)
        {
            RoomName.GetComponent<Text>().text = msg.RoomName;
        }

        if (RoomLock)
        {
            RoomLock.SetActive(msg.IsLock);
        }
        if (RoomNumber)
        {
            RoomNumber.GetComponent<Text>().text = msg.RoomNumber.ToString();
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
