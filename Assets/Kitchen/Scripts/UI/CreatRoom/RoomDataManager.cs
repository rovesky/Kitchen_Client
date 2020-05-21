using Kitchen.PocoInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataManager
{

    public string CurRoomName { get; private set; }
    public string CurPwd { get; private set; }

    public RoomInfoP CurRoomInfo { get; private set; }


    public void SetCurRoomName(string name)
    {
        CurRoomName = name;
    }

    public void SetCurPwd(string pwd)
    {
        CurPwd = pwd;
    }



    public List<RoomInfoP> RoomInfoList { get; private set; }

    public void GetRoomInfoList(List<RoomInfoP> roomInfoList)
    {
        RoomInfoList = roomInfoList;
        Messenger<List<RoomInfoP>>.Broadcast(MessengerEventDef.ROOM_LIST_REFRESH, RoomInfoList);
    }

    public void GetCurRoomInfo(RoomInfoP roomInfo)
    {
        CurRoomInfo = roomInfo;
        Messenger<RoomInfoP>.Broadcast(MessengerEventDef.CUR_ROOM, CurRoomInfo);
    }

    public void RemoveRoom(string id)
    {
        var count = RoomInfoList.Count;
        for (var i = 0; i < count; i ++)
        {
            if (id == RoomInfoList[i].rmid)
            {
                RoomInfoList.Remove(RoomInfoList[i]);
                Debug.Log(id + "<<<<<<<<<<<<<<<<<<<<<<<<<<");
                break;
            }
        }
    }

    
    
}

