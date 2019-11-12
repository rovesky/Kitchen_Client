using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
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


    //[ExecuteAlways]
    //[DisableAutoCreation]
    //public class MoveSystemGroup : NoSortComponentSystemGroup
    //{
    //    protected override void OnCreate()
    //    {
    //        //    m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveSinSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTargetSystem>());
    //        m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveForwardSystem>());
    //     //   m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTranslationSystem>());
    //    }
    //}


    [ExecuteAlways]
    [DisableAutoCreation]
    public class PresentationSystemGroup : NoSortComponentSystemGroup
    {      
        protected override void OnCreate()
        {            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
        //    m_systemsToUpdate.Add(World.GetOrCreateSystem<ExlosionSystem>());
        //    m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateHealthUISystem>());
        }
    }
   

    [ExecuteAlways]
    [DisableAutoCreation]
    public class SetRenderTimeSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {

            var worldTime = GetSingleton<WorldTime>();
            worldTime.gameTick = GetSingleton<ClientTickTime>().render;
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
            worldTime.gameTick = GetSingleton<ClientTickTime>().predict;
            SetSingleton(worldTime);
        }
    }  


    [ExecuteAlways]
	[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
	public class KitchenClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private NetworkClientNewSystem networkSystem;
        private HandleTimeSystem handleTimeSystem;
        private SetRenderTimeSystem setRenderTimeSystem; 
        private SpawnSystemGroup spawnSystemGroup;
        private SetPredictTimeSystem setPredictTimeSystem;
        private PredictSystem predictSystem;
        private PresentationSystemGroup presentationSystemGroup;
        private DespawnSystemGroup despawnSystemGroup;
        private InterpolatedSystem interpolatedSystem;

        protected override void OnCreate()
        {
            FSLog.Info("KitchenClientSimulationSystemGroup OnCreate");
            Application.targetFrameRate = 30;
            ConfigVar.Init();
            GameWorld.Active = new GameWorld();
          
            networkSystem = World.GetOrCreateSystem<NetworkClientNewSystem>();
            m_systemsToUpdate.Add(networkSystem);
    
            
            handleTimeSystem = World.GetOrCreateSystemE<HandleTimeSystem>();
            m_systemsToUpdate.Add(handleTimeSystem);
       

            setRenderTimeSystem = World.GetOrCreateSystemE<SetRenderTimeSystem>();
            m_systemsToUpdate.Add(setRenderTimeSystem);      

            spawnSystemGroup = World.GetOrCreateSystem<SpawnSystemGroup>();
            m_systemsToUpdate.Add(spawnSystemGroup);

            interpolatedSystem = World.GetOrCreateSystemE<InterpolatedSystem>();
            m_systemsToUpdate.Add(interpolatedSystem);

            setPredictTimeSystem = World.GetOrCreateSystemE<SetPredictTimeSystem>();
            m_systemsToUpdate.Add(setPredictTimeSystem);

            predictSystem = World.GetOrCreateSystemE<PredictSystem>();
            m_systemsToUpdate.Add(predictSystem);

            presentationSystemGroup = World.GetOrCreateSystem<PresentationSystemGroup>();
            m_systemsToUpdate.Add(presentationSystemGroup);

            despawnSystemGroup = World.GetOrCreateSystem<DespawnSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);            
        }

        protected override void OnUpdate()
        {
            networkSystem.Update();

            if (networkSystem.IsConnected)
            {       
                handleTimeSystem.Update();
              
                setRenderTimeSystem.Update();

                spawnSystemGroup.Update();

                interpolatedSystem.Update();

                setPredictTimeSystem.Update();

                predictSystem.Update();

                presentationSystemGroup.Update();

                setRenderTimeSystem.Update();

                despawnSystemGroup.Update();

                networkSystem.SendData();
            }


        }
    }
}
