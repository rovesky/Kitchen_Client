using System.Threading;
using Unity.Entities;
using UnityEngine;

public struct Life : IComponentData
{
    public int lifeValue;
}

public class LifeBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public int LifeValue;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Life() { lifeValue = LifeValue });
    }
}

public class LifeSystem : ComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity entity, ref Life life , ref Damage damage) =>
        {
            life.lifeValue -= damage.damage;
            damage.damage = 0;
            if (life.lifeValue <= 0)
            {
                Debug.Log($"DestroyEntity:{entity.Index},thread:{Thread.CurrentThread.ManagedThreadId}");
                CommandBuffer.DestroyEntity(entity);
            }
        });
    }
}
