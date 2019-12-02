using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{ 
    [DisableAutoCreation]
    public class UpdateUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (GetEntityQuery(typeof(ServerSnapshot)).CalculateEntityCount() == 0)
                return;

            var snapshotFromServer = GetSingleton<ServerSnapshot>();
            UIManager.Instance.UpdateRtt(snapshotFromServer.Rtt);
          
        }
    }
}