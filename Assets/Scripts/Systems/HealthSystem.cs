using Unity.Entities;

namespace Assets.Scripts.ECS
{
    public class HealthSystem : ComponentSystem
    {
        EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

            Entities.ForEach((Entity entity, ref Health health, ref Damage damage) =>
            {
                health.Value -= damage.Value;
                damage.Value = 0;
                if (health.Value <= 0)
                {
                    // Debug.Log($"DestroyEntity:{entity.Index},thread:{Thread.CurrentThread.ManagedThreadId}");
                    CommandBuffer.DestroyEntity(entity);
                }
            });
        }
    }
}
