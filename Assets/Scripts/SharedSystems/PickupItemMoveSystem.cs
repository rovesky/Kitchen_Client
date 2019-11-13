using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class PickupItemMoveSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
          
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<PickupItem>().ForEach((ref LocalToWorld localToWorld,ref PickupItem pickupItem) =>
            {
                if (pickupItem.pickupEntity != Entity.Null)
                {
                    var platePos = EntityManager.GetComponentData<Translation>(pickupItem.pickupEntity);
                    platePos.Value = localToWorld.Position + (localToWorld.Forward * 0.9f) + new float3(0f, 0.2f, 0f);
                    EntityManager.SetComponentData(pickupItem.pickupEntity, platePos);

                   
                }

            });

        }      
    }
}