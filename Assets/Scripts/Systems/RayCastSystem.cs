using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class RayCastSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entityA,ref PhysicsCollider collider,
                ref Rocket rocket,
                ref Translation position,
                ref Rotation rotation) =>
            {
                ref PhysicsWorld world = ref World.Active.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
                float3 rayStart = position.Value;
                float3 a = Vector3.forward;
                float3 rayEnd = rayStart + (0.3f * a);

                var rayInput = new RaycastInput
                {
                    Start = rayStart,
                    End = rayEnd,
                    Filter = collider.Value.Value.Filter,
                };

                Unity.Physics.RaycastHit rayHit;
                bool hit = (world.CastRay(rayInput, out rayHit)); //&& rayHit.SurfaceNormal.y > 0.5);

                if (hit)
                {
                    Debug.LogWarning($"raycast:{rayHit.RigidBodyIndex},{rayHit.SurfaceNormal}");
                    var entityB = world.Bodies[rayHit.RigidBodyIndex].Entity;

                    if (EntityManager.HasComponent<TriggerDestroy>(entityA))
                    {
                        EntityManager.DestroyEntity(entityA);
                    }
                }
            });
        }
    }
}