using UnityEngine;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    public class HealthSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref Health health, ref Damage damage) =>
            {
                health.Value -= damage.Value;
                damage.Value = 0;
                if (health.Value <= 0)
                {
                    if(!EntityManager.HasComponent<Despawn>(entity))
                        EntityManager.AddComponentData(entity, new Despawn() {Frame = 1});
                }
            });

            Entities.WithNone<Player>().ForEach((Entity entity,ref Health health, ref Damage damage) =>
            {
                health.Value -= damage.Value;
                damage.Value = 0;
                if (health.Value <= 0)
                {
                    EntityManager.AddComponentData(entity, new Despawn() { Frame = 0 });
                }
            });
        }
    }
}
