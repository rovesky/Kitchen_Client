using Unity.Entities;
using UnityEngine;
namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateRTTSystem : ComponentSystem
    {
        private int frameCount;
        private float passedTime;

        protected override void OnUpdate()
        {
            if (GetEntityQuery(typeof(ServerSnapshot)).CalculateEntityCount() <= 0) 
                return;

            var snapshotFromServer = GetSingleton<ServerSnapshot>();
            UIManager.Instance.UpdateRtt(snapshotFromServer.Rtt);

        }
    }
}