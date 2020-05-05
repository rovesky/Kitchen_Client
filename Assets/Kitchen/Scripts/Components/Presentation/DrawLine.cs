using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class DrawLine : MonoBehaviour
    {

        void OnDrawGizmos()
        {

            var phyicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<KitchenBuildPhysicsWorld>();
            if (phyicsSystem == null)
                return;
            ref var physicsWorld = ref phyicsSystem.PhysicsWorld;

            var pos = transform.position + (Vector3) math.forward(transform.rotation) * 1.8f;
            pos.y = 1.0f;
            var input = new RaycastInput
            {
                Start = pos,
                End = pos + (Vector3) math.forward(transform.rotation) * 3,
                Filter = CollisionFilter.Default
            };

            // FSLog.Info($"DrawLine,Start:{input.Start},End:{input.End}");
            var raycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
            if (physicsWorld.CastRay(input, ref raycastHits))
            {
                // FSLog.Info($"TriggerEntitiesSystem success");
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(input.Start, input.End - input.Start);

            foreach (var raycastHit in raycastHits)
            {
                //  FSLog.Info($"hit:{raycastHit.Position} success");
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(input.Start, raycastHit.Position - input.Start);

            }

            raycastHits.Dispose();
        }

    }
}
