using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class DrawPointDistance : MonoBehaviour
    {

        void OnDrawGizmos()
        {

            //var phyicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<KitchenBuildPhysicsWorld>();
            //if (phyicsSystem == null)
            //    return;
            //ref var physicsWorld = ref phyicsSystem.PhysicsWorld;

            //var input = new PointDistanceInput
            //{
            //    Position = transform.position +
            //               (Vector3) math.mul(transform.rotation,new Vector3(0.87f,0,0.88f)),
            //    MaxDistance = 1.5f,
            //    Filter = CollisionFilter.Default
            //};

            //var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
            //if (!physicsWorld.CalculateDistance(input, ref distanceHits))
            //    return;
            
            var query =  World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Table>());
            var tableEntities = query.ToEntityArray(Allocator.TempJob);

            foreach (var tableEntity in tableEntities)
            {
                var localToWorld =
                    World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalToWorld>(tableEntity);
                var slotSetting =
                    World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<SlotSetting>(tableEntity);

                Gizmos.color = Color.yellow;
                var pos = localToWorld.Position + math.mul(localToWorld.Rotation, slotSetting.Pos);
                Gizmos.DrawSphere(pos, 1.5f);
            }
         

            tableEntities.Dispose();
        }

    }
}
