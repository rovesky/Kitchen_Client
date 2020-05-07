using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PreviewClientSimulationSystemGroup : NoSortComponentSystemGroup
    {
        private GameTick gameTime = GameTick.DefaultGameTick;
        private double nextTickTime;

        private SpawnSystemGroup spawnSystemGroup;
        private PredictUpdateSystemGroup predictUpdateSystem;
        private PredictPresentationSystemGroup predictPresentationSystemGroup;
        private DespawnClientSystemGroup despawnSystemGroup;
        private InputSystem inputSystem;
        private UpdateReplicatedOwnerFlag updateReplicatedOwnerFlag;
        private ServerSystemGroup serverSystemGroup;
        private InitSystemGroup initSystemGroup;
        private SpawnCharactersSystem spawnCharacterSystem;

        protected override void OnCreate()
        {
            FSLog.Info("PreviewClientSimulationSystemGroup OnCreate");
          //  Application.targetFrameRate = 40;
            UnityEngine.Time.fixedDeltaTime = gameTime.TickInterval;
            ConfigVar.Init();
            ItemCreateUtilities.Init();
            ClientCharacterUtilities.Init();

            GameWorld.Active = new GameWorld();

            //  World.DestroySystem(World.GetExistingSystem<ExportPhysicsWorld>());

            inputSystem = World.GetOrCreateSystem<InputSystem>();
            m_systemsToUpdate.Add(inputSystem);

            initSystemGroup = World.GetOrCreateSystem<InitSystemGroup>();
            m_systemsToUpdate.Add(initSystemGroup);

            spawnCharacterSystem = World.GetOrCreateSystem<SpawnCharactersSystem>();
            m_systemsToUpdate.Add(spawnCharacterSystem);

            spawnSystemGroup = World.GetOrCreateSystem<SpawnSystemGroup>();
            m_systemsToUpdate.Add(spawnSystemGroup);

            updateReplicatedOwnerFlag = World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>();
            m_systemsToUpdate.Add(updateReplicatedOwnerFlag);

            predictUpdateSystem = World.GetOrCreateSystem<PredictUpdateSystemGroup>();
            m_systemsToUpdate.Add(predictUpdateSystem);

            serverSystemGroup = World.GetOrCreateSystem<ServerSystemGroup>();
            m_systemsToUpdate.Add(serverSystemGroup);

            predictPresentationSystemGroup = World.GetOrCreateSystem<PredictPresentationSystemGroup>();
            m_systemsToUpdate.Add(predictPresentationSystemGroup);
          
            despawnSystemGroup = World.GetOrCreateSystem<DespawnClientSystemGroup>();
            m_systemsToUpdate.Add(despawnSystemGroup);

            updateReplicatedOwnerFlag.SetLocalPlayerId(0);
        }

        protected override void OnUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            inputSystem.SampleInput(worldTime.Tick);

            var commandWasConsumed = false;
            while (worldTime.FrameTime > nextTickTime)
            {
                gameTime.Tick++;
                gameTime.TickDuration = gameTime.TickInterval;

                commandWasConsumed = true;
                PreviewTickUpdate();
                nextTickTime += worldTime.GameTick.TickInterval;
            }

            if (commandWasConsumed)
                inputSystem.ResetInput();
        }

        private void PreviewTickUpdate()
        {
            var worldTime = GetSingleton<WorldTime>();
            worldTime.GameTick = gameTime;

            inputSystem.StoreCommand(worldTime.Tick);
            inputSystem.ResetInput();

            initSystemGroup.Update();

            spawnCharacterSystem.Update();
            spawnSystemGroup.Update();

        
            updateReplicatedOwnerFlag.Update();

           // for (var i = 0; i < 1; ++i)
            {
                inputSystem.RetrieveCommand(worldTime.Tick);

                predictUpdateSystem.Update();

                serverSystemGroup.Update();
            }


            predictPresentationSystemGroup.Update();

            despawnSystemGroup.Update();

            SetSingleton(worldTime);
        }
    }
}
