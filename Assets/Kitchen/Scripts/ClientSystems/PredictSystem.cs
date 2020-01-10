using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PredictSystem : ComponentSystem
    {
    
        private InputSystem inputSystem;
        private PredictUpdateSystemGroup predictUpdateSystemGroup;
        private ReplicateEntitySystemGroup replicateEntitySystemGroup;
        private PredictRollbackStateSystemGroup predictRollbackStateSystemGroup;

        protected override void OnCreate()
        {
            predictUpdateSystemGroup = World.GetOrCreateSystem<PredictUpdateSystemGroup>();
            replicateEntitySystemGroup = World.GetOrCreateSystem<ReplicateEntitySystemGroup>();
            predictRollbackStateSystemGroup = World.GetOrCreateSystem<PredictRollbackStateSystemGroup>();

            inputSystem = World.GetOrCreateSystem<InputSystem>();
        }

        protected override void OnUpdate()
        {
            var serverTick = GetSingleton<ServerSnapshot>().Tick;
            var clientTick = GetSingleton<ClientTickTime>();
            var worldTime = GetSingleton<WorldTime>();
            var predictTime = clientTick.Predict;

            if (!IsPredictionAllowed(predictTime, serverTick)) 
                return;
            // ROLLBACK. All predicted entities (with the ServerEntity component) are rolled back to last server state 
            worldTime.SetTick(serverTick, predictTime.TickInterval);
            SetSingleton(worldTime);

            PredictionRollback();

            // PREDICT PREVIOUS TICKS. Replay every tick *after* the last tick we have from server up to the last stored command we have
            for (var tick = serverTick + 1; tick < predictTime.Tick; tick++)
            {
                worldTime.SetTick(tick, predictTime.TickInterval);
                SetSingleton(worldTime);

                inputSystem.RetrieveCommand(worldTime.Tick);
                PredictionUpdate();
            }

            // PREDICT CURRENT TICK. Update current tick using duration of current tick
            worldTime.GameTick = predictTime;
            SetSingleton(worldTime);
            inputSystem.RetrieveCommand(worldTime.Tick);
            // Dont update systems with close to zero time. 
            if (worldTime.TickDuration > 0.008f)
                PredictionUpdate();
        }

        private void PredictionUpdate()
        {
        //    var worldTime = GetSingleton<WorldTime>();
        //    FSLog.Info($"PredictionUpdate:{worldTime.Tick}");
            predictUpdateSystemGroup.Update();
        }

        private void PredictionRollback()
        {
            var worldTime = GetSingleton<WorldTime>();
          //  FSLog.Info($"PredictionRollback:{worldTime.Tick}");
            replicateEntitySystemGroup.Rollback();
            predictRollbackStateSystemGroup.Update();
        }


        private bool IsPredictionAllowed(GameTick predictTime, uint serverTick)
        {
            if (predictTime.Tick <= serverTick)
            {
                FSLog.Warning("No predict! Predict time not ahead of server tick! ");
                return false;
            }

            if (!inputSystem.HasCommands(serverTick + 1, predictTime.Tick))
            {
                FSLog.Warning("No predict! No commands available. ");
                return false;
            }

            return true;
        }
    }
}