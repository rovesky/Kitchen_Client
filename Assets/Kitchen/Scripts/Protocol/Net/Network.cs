using FootStone.Client;
using FootStone.GrainInterfaces;
using FootStone.ProtocolNetty;
using Ice;
using Kitchen.PocoInterfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace SampleClient
{

    internal class RoomPushI : IRoomPushDisp_, IServerPush
    {
        private string account;
        private SessionPushI sessionPushI;

        public override void EnterMessage(List<string> playids, Current current = null)
        {
            Debug.Log(playids.Count + "<<<<<<<<<<");
            DataManager.Instance.RoomDataManager.SetEnterList(playids);
        }

        public string GetFacet()
        {
            return typeof(IRoomPushPrx).Name;
        }

        public override void ReadyMessage(List<string> playids, Current current = null)
        {
            Debug.Log(playids.Count + "<<<<<<<<<<");
            DataManager.Instance.RoomDataManager.SetReadyList(playids);
        }

        public void setAccount(string account)
        {
            this.account = account;
        }

        public void setSessionPushI(SessionPushI sessionPushI)
        {
            this.sessionPushI = sessionPushI;
        }

    }
    public class NetworkNew : BSingleton<NetworkNew>
    {
        public IFSSession Session { get; private set; }

        public async void Init(string ip, int port)
        {
            var client = new FSClientBuilder().IceOptions(iceOptions =>
            {
                iceOptions.EnableDispatcher = false;
                iceOptions.PushObjects = new List<Ice.Object>();
                iceOptions.PushObjects.Add(new RoomPushI());
            }).NettyOptions(nettyOptions =>
            {
                nettyOptions.Port = 8007;
            }).Build();
            await client.StartAsync();

            var sessionId = "session" + 1;
            Session = await client.CreateSession(ip, port, sessionId);

        }

        public async void LoginRequest(string account, string password)
        {
            await LoginTask(account, password);
        }
        private async Task LoginTask(string account, string password)
        {
            try
            {

                var accountPrx = Session.UncheckedCast(IAccountPrxHelper.uncheckedCast);
                try
                {
                    LoginInfo loginInfo = await accountPrx.LoginRequestAsync(account, password);
                    DataManager.Instance.UserData.InitPlayerData(loginInfo);

                }
                catch (Ice.Exception ex)
                {
                    Debug.Log(ex.ToString() + "????????????????????????????????");
                }
            }
            catch (System.Exception e)
            {

            }
        }

        public async void GetRoomInfoListRequest()
        {
            await GetRoomInfoList();
        }
        private async Task GetRoomInfoList()
        {
            try
            {
                var roomPrx = Session.UncheckedCast(IRoomPrxHelper.uncheckedCast);
                List<RoomInfoP> roomInfoList = await roomPrx.GetRoomListAsync(0);
                DataManager.Instance.RoomDataManager.GetRoomInfoList(roomInfoList);
            }
            catch (Ice.Exception ex)
            {
                Debug.Log(ex.ToString() + "????????????????????????????????");
            }
        }



        public async void CreatRoomRequest(string name, string ciph, byte tp)
        {
            await CreatRoom(name, ciph, tp);
        }
        private async Task CreatRoom(string name, string ciph, byte tp)
        {
            try
            {
                var roomPrx = Session.UncheckedCast(IRoomPrxHelper.uncheckedCast);
                RoomInfoP roomInfo = await roomPrx.CreateRoomAsync(name, ciph, tp);
                DataManager.Instance.RoomDataManager.SetOwner(true);
                DataManager.Instance.RoomDataManager.GetCurRoomInfo(roomInfo);
            }
            catch (Ice.Exception ex)
            {

                throw;
            }
        }

        public async void RemoveRoomRequest(string roomId)
        {
            await RemoveRoom(roomId);
        }

        private async Task RemoveRoom(string roomId)
        {
            var roomPrx = Session.UncheckedCast(IRoomPrxHelper.uncheckedCast);
            var isRemove = await roomPrx.RemoveRoomAsync(roomId);
            if (isRemove)
            {
                DataManager.Instance.RoomDataManager.RemoveRoom(roomId);
            }
        }

        public async void EnterRoomRequest(string roomId, string pwd)
        {
            await EnterRoom(roomId, pwd);
        }
        private async Task EnterRoom(string roomId, string pwd)
        {
            var roomPrx = Session.UncheckedCast(IRoomPrxHelper.uncheckedCast);
            var isEnter = await roomPrx.EnterRoomAsync(roomId, pwd);

            Debug.Log(isEnter + "<<<<<<<<<<<<<<<<<<<<<<<<<进入房间");
            if (isEnter)
            {
                DataManager.Instance.RoomDataManager.SetIsReady(false);
                DataManager.Instance.RoomDataManager.SetCurRoomID(roomId);
                PanelManager.Instance.CloseAllWindow();
                Globe.nextSceneName = CommonDef.TempScene;//目标场景名称
                UnityEngine.SceneManagement.SceneManager.LoadScene(CommonDef.Loading);//加载进度条场景
            }
            //await ReadyGame(roomId);
        }

        public async void ReadyGameRequest(string roomId)
        {
            await ReadyGame(roomId);
        }

        private async Task ReadyGame(string roomId)
        {
            var roomPrx = Session.UncheckedCast(IRoomPrxHelper.uncheckedCast);
            var isReady = await roomPrx.ReadyRoomAsync(roomId);
            DataManager.Instance.RoomDataManager.SetIsReady(true);
            Debug.Log("Is Ready <<<<<<<<<<<<<<<<<<<<<" + isReady);
            PanelManager.Instance.CloseAllWindow();
            Globe.nextSceneName = CommonDef.kitchen_01;//目标场景名称
            UnityEngine.SceneManagement.SceneManager.LoadScene(CommonDef.Loading);//加载进度条场景
        }
    }
}
