using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class PickupSystem : ComponentSystem
    {
        private EntityQuery plateQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            plateQuery = GetEntityQuery(typeof(Plate));

        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity,ref UserCommand command,ref LocalToWorld localToWorld,
                ref PickupItem pickupItem,ref Rotation rotation) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (command.buttons.IsSet(UserCommand.Button.Pick) )
                {
                    if (pickupItem.pickupEntity == Entity.Null)
                    {
                        if (plateQuery.CalculateEntityCount() == 0)
                            return;

                        var plates = plateQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
                        pickupItem.pickupEntity = plates[0];
                        plates.Dispose();

                        if (!EntityManager.HasComponent<Parent>(pickupItem.pickupEntity))
                        {
                            EntityManager.AddComponentData(pickupItem.pickupEntity, new Parent() { Value = entity });
                            EntityManager.AddComponentData(pickupItem.pickupEntity, new LocalToParent());
                            EntityManager.SetComponentData(pickupItem.pickupEntity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
                            EntityManager.SetComponentData(pickupItem.pickupEntity, new Rotation() { Value = quaternion.identity });

                        }

                    }
                    else
                    {
                        EntityManager.RemoveComponent<Parent>(pickupItem.pickupEntity);
                        EntityManager.RemoveComponent<LocalToParent>(pickupItem.pickupEntity);

                        var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(pickupItem.pickupEntity);
                        Vector3 linear = localToWorld.Forward;
                        linear.y = 0.2f;
                        linear.Normalize();
                        physicsVelocity.Linear = linear * 10;
                        EntityManager.SetComponentData(pickupItem.pickupEntity,physicsVelocity);

                        EntityManager.SetComponentData(pickupItem.pickupEntity, new Translation() { Value = localToWorld.Position +   math.mul(rotation.Value, new float3(0, 0.2f, 1.2f) )  });
                        pickupItem.pickupEntity = Entity.Null;

                    }
                   
                }

            });

        }      
    }
}