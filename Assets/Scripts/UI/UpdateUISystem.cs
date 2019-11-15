using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{ 
    [DisableAutoCreation]
    public class UpdateUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (GetEntityQuery(typeof(SnapshotFromServer)).CalculateEntityCount() == 0)
                return;
            var snapshotFromServer = GetSingleton<SnapshotFromServer>();
            UIManager.Instance.UpdateRtt(snapshotFromServer.rtt);
            //Entities.WithAllReadOnly<UpdateUI>().ForEach((Entity entity, ref Health health, ref Score score) =>
            //{          

             
            //});
        }
    }
}