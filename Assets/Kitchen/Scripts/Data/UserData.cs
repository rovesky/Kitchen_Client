using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string UserName { get; private set; }
    public string Token { get; private set; }

    public void  InitPlayerData(Kitchen.PocoInterfaces.LoginInfo info)
    {
        UserName = info.token;
        Token = info.token;
        Messenger<string>.Broadcast(MessengerEventDef.REFRESH_UI, "LoginWindow");
    }
}
