﻿//using FootStone.ECS;
//using Unity.Entities;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{
//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class SpawnSystemGroup : NoSortComponentSystemGroup
//    {
     
//        protected override void OnCreate()
//        {
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnEntitiesClientSystem>());
//        }
//    }

//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class DespawnSystemGroup : NoSortComponentSystemGroup
//    {     
//        protected override void OnCreate()
//        {
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystem>());
//        }
//    }


//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class MoveSystemGroup : NoSortComponentSystemGroup
//    {
//        protected override void OnCreate()
//        {
//            //    m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveSinSystem>());
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTargetSystem>());
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveForwardSystem>());
//         //   m_systemsToUpdate.Add(World.GetOrCreateSystem<MoveTranslationSystem>());
//        }
//    }


//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class PresentationSystemGroup : NoSortComponentSystemGroup
//    {      
//        protected override void OnCreate()
//        {            
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPresentationSystem>());
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<ExlosionSystem>());
//            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateHealthUISystem>());
//        }
//    }
   

//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class SetRenderTimeSystem : FSComponentSystem
//    {
//        protected override void OnUpdate()
//        {

//            var worldTime = GetSingleton<WorldTime>();
//            worldTime.gameTick = GetSingleton<ClientTickTime>().render;
//            SetSingleton(worldTime);
//        }
//    }

//    [ExecuteAlways]
//    [DisableAutoCreation]
//    public class SetPredictTimeSystem : FSComponentSystem
//    {
//        protected override void OnUpdate()
//        {            
//            var worldTime = GetSingleton<WorldTime>();
//            worldTime.gameTick = GetSingleton<ClientTickTime>().predict;
//            SetSingleton(worldTime);
//        }
//    }  


//    [ExecuteAlways]
//    public class PredictClientSimulationSystemGroup : NoSortComponentSystemGroup
//    {
//        private NetworkClientNewSystem networkSystem;
//        private HandleTimeSystem handleTimeSystem;
//        private SetRenderTimeSystem setRenderTimeSystem;
//    //    private ReadSnapshotSystem readSnapshotSystem;
//        private SpawnSystemGroup spawnSystemGroup;
//        private SetPredictTimeSystem setPredictTimeSystem;
//        private PredictSystem predictSystem;
//        private PresentationSystemGroup presentationSystemGroup;
//        private DespawnSystemGroup despawnSystemGroup;

//        protected override void OnCreate()
//        {
//            FSLog.Info("PredictClientSimulationSystemGroup OnCreate");
//            ConfigVar.Init();
//            GameWorld.Active = new GameWorld();
          
//            networkSystem = World.GetOrCreateSystem<NetworkClientNewSystem>();
//            m_systemsToUpdate.Add(networkSystem);

//            //readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();
//            //m_systemsToUpdate.Add(readSnapshotSystem);
            
//            handleTimeSystem = World.GetOrCreateSystemE<HandleTimeSystem>();
//            m_systemsToUpdate.Add(handleTimeSystem);         

//            setRenderTimeSystem = World.GetOrCreateSystemE<SetRenderTimeSystem>();
//            m_systemsToUpdate.Add(setRenderTimeSystem);

//            spawnSystemGroup = World.GetOrCreateSystem<SpawnSystemGroup>();
//            m_systemsToUpdate.Add(spawnSystemGroup);

//            setPredictTimeSystem = World.GetOrCreateSystemE<SetPredictTimeSystem>();
//            m_systemsToUpdate.Add(setPredictTimeSystem);

//            predictSystem = World.GetOrCreateSystemE<PredictSystem>();
//            m_systemsToUpdate.Add(predictSystem);

//            presentationSystemGroup = World.GetOrCreateSystem<PresentationSystemGroup>();
//            m_systemsToUpdate.Add(presentationSystemGroup);

//            despawnSystemGroup = World.GetOrCreateSystem<DespawnSystemGroup>();
//            m_systemsToUpdate.Add(despawnSystemGroup);            
//        }

//        protected override void OnUpdate()
//        {
//            networkSystem.Update();

//            if (networkSystem.IsConnected)
//            {
//            //    readSnapshotSystem.Update();

//                handleTimeSystem.Update();
              
//                setRenderTimeSystem.Update();

//                spawnSystemGroup.Update();

//                setPredictTimeSystem.Update();

//                predictSystem.Update();

//                presentationSystemGroup.Update();

//                setRenderTimeSystem.Update();

//                despawnSystemGroup.Update();
//            }

//            networkSystem.SendData();

//        }
//    }
//}
