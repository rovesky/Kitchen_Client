using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class RayCastSystem : ComponentSystem
    {
        private void DoCollide(Entity entityA,ref LocalToWorld localToWorld, ref Translation position, ref PhysicsCollider collider, float distance)
        {
            ref PhysicsWorld world = ref World.Active.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
            float3 rayStart = position.Value;
            float3 rayEnd = rayStart + (float3)distance * localToWorld.Forward;

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
          //  Debug.Log($"raycast:{rayHit.RigidBodyIndex},{Time.time}");
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entityA,
                ref LocalToWorld localToWorld,
                ref Translation position,
                ref PhysicsCollider collider,
                ref Rocket rocket) =>
            {
                float distance = (rocket.Type == RocketType.Player) ? -0.4f : -0.3f;
                DoCollide(entityA,ref localToWorld,ref position,ref collider, distance);
            });


            Entities.ForEach((Entity entityA,
                ref LocalToWorld localToWorld,
                ref Translation position,
                ref PhysicsCollider collider,
                ref Player player) =>
            {
                DoCollide(entityA, ref localToWorld, ref position, ref collider, -0.6f);
            });
        }
    }
}
