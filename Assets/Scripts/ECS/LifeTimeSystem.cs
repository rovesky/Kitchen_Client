using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class LifeTimeSystem : ComponentSystem
    {
        EntityCommandBufferSystem m_Barrier;

        protected override void OnCreate()
        {
            m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = m_Barrier.CreateCommandBuffer();
            // Entities.ForEach processes each set of ComponentData on the main thread. This is not the recommended
            // method for best performance. However, we start with it here to demonstrate the clearer separation
            // between ComponentSystem Update (logic) and ComponentData (data).
            // There is no update logic on the individual ComponentData.
            Entities.ForEach((Entity entity, ref LifeTime lifeTime) =>
            {

                lifeTime.Value -= Time.deltaTime;

                if (lifeTime.Value < 0.0f)
                {
                    //  commandBuffer.DestroyEntity()
                    commandBuffer.DestroyEntity(entity);
                }

            });
        }
    }

    /* public class LifeTimeSystem : JobComponentSystem
     {
         EntityCommandBufferSystem m_Barrier;

         protected override void OnCreate()
         {
             m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
         }

         // Use the [BurstCompile] attribute to compile a job with Burst.
         // You may see significant speed ups, so try it!
         [BurstCompile]
         struct LifeTimeJob : IJobForEachWithEntity<LifeTime>
         {
             public float DeltaTime;

             [WriteOnly]
             public EntityCommandBuffer.Concurrent CommandBuffer;

             public void Execute(Entity entity, int jobIndex, ref LifeTime lifeTime)
             {
                 lifeTime.Value -= DeltaTime;

                 if (lifeTime.Value < 0.0f)
                 {
                     CommandBuffer.DestroyEntity(jobIndex, entity);
                 }
             }
         }

         // OnUpdate runs on the main thread.
         protected override JobHandle OnUpdate(JobHandle inputDependencies)
         {
             var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

             var job = new LifeTimeJob
             {
                 DeltaTime = Time.deltaTime,
                 CommandBuffer = commandBuffer,

             }.Schedule(this, inputDependencies);

             m_Barrier.AddJobHandleForProducer(job);

             return job;
         }

     }*/
}
