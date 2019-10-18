using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [ExecuteAlways]
    [DisableAutoCreation]
    public class SpawnSystemGroup : NoSortComponentSystemGroup
    {
     
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnEntitiesClientSystem>());
        }
    }

    [ExecuteAlways]
    [DisableAutoCreation]
    public class DespawnSystemGroup : NoSortComponentSystemGroup
    {     
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystem>());
        }
    }


    [ExecuteAlways]
    [DisableAutoCreation]
    public class PresentationSystemGroup : NoSortComponentSystemGroup
    {      
        protected override void OnCreate()
        {            
        //    m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ExlosionSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateHealthUISystem>());
        }
    }
   

    [ExecuteAlways]
    [DisableAutoCreation]
    public class SetRenderTimeSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {

            var worldTime = GetSingleton<WorldTime>();
            worldTime.tick = GetSingleton<ClientTickTime>().render;
            SetSingleton(worldTime);
        }
    }

    [ExecuteAlways]
    [DisableAutoCreation]
    public class SetPredictTimeSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {            
            var worldTime = GetSingleton<WorldTime>();
            worldTime.tick = GetSingleton<ClientTickTime>().predict;
            SetSingleton(worldTime);
        }
    }  


    [ExecuteAlways]
    public class PredictClientSimulationSystemGroup : NoSortComponentSystemGroup
    {       
            
        protected override void OnCreate()
        {
            FSLog.Info("PredictClientSimulationSystemGroup OnCreate");
            GameWorld.Active = new GameWorld();
            m_systemsToUpdate.Add(World.GetOrCreateSystem<NetworkClientSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystemE<HandleTimeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystemE<SetRenderTimeSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<InputSystem>());          
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ReadSnapshotSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnSystemGroup>());

            m_systemsToUpdate.Add(World.GetOrCreateSystemE<SetPredictTimeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystemE<PredictSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<PresentationSystemGroup>());

            m_systemsToUpdate.Add(World.GetOrCreateSystemE<SetRenderTimeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystemGroup>());            
        }         
    } 
}
