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

    public class NetworkNew : BSingleton<NetworkNew>
    {
        public IFSSession Session { get; private set; }

        public async void Init(string ip, int port)
        {
            var client = new FSClientBuilder().IceOptions(iceOptions =>
            {
                iceOptions.EnableDispatcher = false;
                iceOptions.PushObjects = new List<Ice.Object>();
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
        private async Task LoginTask(string  account , string password)
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





    }
}
