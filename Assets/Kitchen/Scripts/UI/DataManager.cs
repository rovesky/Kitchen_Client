using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BSingleton<DataManager>
{
    public LoginDataManager LoginDataManager { get; private set; }
    public MainDataManager MainDataManager { get; private set; }
    public RoomDataManager RoomDataManager { get; private set; }
    public UserData UserData { get; private set; }
    public void Init()
    {
        LoginDataManager = new LoginDataManager();
        MainDataManager = new MainDataManager();
        RoomDataManager = new RoomDataManager();
        UserData = new UserData();
    }
}
