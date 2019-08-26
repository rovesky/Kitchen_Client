using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
   // [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    [UpdateBefore(typeof(HealthSystem))]
    public class RayCastSystem : ComponentSystem
    {
        private void DoCollide(Entity entityA, ref Translation position, ref PhysicsCollider collider, float distance)
        {
            ref PhysicsWorld world = ref World.Active.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
            float3 rayStart = position.Value;
            float3 rayEnd = rayStart + (float3) distance * Vector3.forward;

            var rayInput = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = collider.Value.Value.Filter,
            };

            Unity.Physics.RaycastHit rayHit;
            bool hit = (world.CastRay(rayInput, out rayHit)); //&& rayHit.SurfaceNormal.y > 0.5);

            if (!hit)
                return;

            Debug.LogWarning($"raycast:{rayHit.RigidBodyIndex},{rayHit.SurfaceNormal}");
            var entityB = world.Bodies[rayHit.RigidBodyIndex].Entity;

          

            bool isBodyAAttacker = EntityManager.HasComponent<Attack>(entityA);
            bool isBodyBAttacker = EntityManager.HasComponent<Attack>(entityB);

            bool isBodyADamage = EntityManager.HasComponent<Damage>(entityA);
            bool isBodyBDamage = EntityManager.HasComponent<Damage>(entityB);


            if (isBodyAAttacker && isBodyBDamage)
            {
                var damageComponent = EntityManager.GetComponentData<Damage>(entityB);
                damageComponent.Value += EntityManager.GetComponentData<Attack>(entityA).Power;
                EntityManager.SetComponentData(entityB, damageComponent);
            }

            if (isBodyBAttacker && isBodyADamage)
            {
                var damageComponent = EntityManager.GetComponentData<Damage>(entityA);
                damageComponent.Value += EntityManager.GetComponentData<Attack>(entityB).Power;
                EntityManager.SetComponentData(entityA, damageComponent);
            }

            //if (EntityManager.HasComponent<TriggerDestroy>(entityA))
            //{
            //    EntityManager.DestroyEntity(entityA);
            //}

            //if (EntityManager.HasComponent<TriggerDestroy>(entityB))
            //{
            //    EntityManager.DestroyEntity(entityB);
            //}
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entityA,
                ref Translation position,
                ref PhysicsCollider collider,
                ref Rocket rocket) =>
            {
                float distance = rocket.Type == RocketType.Player ? 0.3f : -0.2f;
                DoCollide(entityA,ref position,ref collider, distance);
            });


            Entities.ForEach((Entity entityA,
                ref Translation position,
                ref PhysicsCollider collider,
                ref Player player) =>
            {
                DoCollide(entityA, ref position, ref collider, 0.5f);
            });
        }
    }
}
