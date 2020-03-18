using System;
using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class HandleTimeSystem : ComponentSystem
    {
        private float frameTimeScale = 1.0f;
        private InputSystem inputSystem;
        private double lastFrameTime;
     //   private WorldTimeSystem worldTimeSystem;

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(ClientTickTime));
            SetSingleton(new ClientTickTime
            {
                Predict = new GameTick(30),
                Render = new GameTick(30)
            });

            inputSystem = World.GetOrCreateSystem<InputSystem>();
           // worldTimeSystem = World.GetOrCreateSystem<WorldTimeSystem>();
        }

        protected override void OnUpdate()
        {
            var snapshot = GetSingleton<ServerSnapshot>();
            var serverTick = snapshot.ServerTick;
            var clientTickTime = GetSingleton<ClientTickTime>();
            var worldTime = GetSingleton<WorldTime>();

            var frameDuration = Math.Abs(lastFrameTime) > 0 ? (float) (worldTime.FrameTime - lastFrameTime) : 0;
            lastFrameTime = worldTime.FrameTime;

            inputSystem.SampleInput(clientTickTime.Render.Tick);

            // handle pridect tick
            var prevTick = clientTickTime.Predict.Tick;

            // Increment time
            var deltaPredictedTime = frameDuration * frameTimeScale;
            clientTickTime.Predict.AddDuration(deltaPredictedTime);

            // Adjust time to be synchronized with server
            const int preferredBufferedCommandCount = 2;
         
            var preferredTick = serverTick +
                                (uint) ((snapshot.Rtt + snapshot.TimeSinceSnapshot) / 1000.0f * worldTime.GameTick.TickRate) +
                                preferredBufferedCommandCount;


            var resetTime = false;
            if (clientTickTime.Predict.Tick < preferredTick - 3)
            {
                FSLog.Warning("Client predictTime hard catchup ... ");
                resetTime = true;
            }

            if (!resetTime && clientTickTime.Predict.Tick > preferredTick + 6)
            {
                FSLog.Warning("Client predictTime hard slowdown ... ");
                resetTime = true;
            }

            frameTimeScale = 1.0f;
            if (resetTime)
            {
                FSLog.Warning(string.Format("CATCHUP ({0} -> {1} ：{2})", clientTickTime.Predict.Tick, preferredTick,
                    serverTick));
                // m_NetworkStatistics.notifyHardCatchup = true;
                //  m_GameWorld.nextTickTime = Game.FrameTime;             
                clientTickTime.Predict.SetTick(preferredTick, 0);
            }
            else
            {
                var bufferedCommands = snapshot.LastAcknowledgedTick - (int) serverTick;
                if (bufferedCommands < preferredBufferedCommandCount)
                    frameTimeScale = 1.01f;

                if (bufferedCommands > preferredBufferedCommandCount)
                    frameTimeScale = 0.99f;
            }

           // FSLog.Info($"preferredTick：{preferredTick-serverTick}，frameTimeScale：{frameTimeScale}，frameDuration{frameDuration}");
            //handle render tick
            // Increment interpolation time
            clientTickTime.Render.AddDuration(frameDuration * frameTimeScale);

            // Force interp time to not exeede server time
            if (clientTickTime.Render.Tick >= serverTick)
                //    FSLog.Warning($"Client renderTime hard slowdown ... ");
                clientTickTime.Render.SetTick(serverTick, 0);

            // hard catchup
            if (serverTick > 10 && clientTickTime.Render.Tick < serverTick - 10)
            {
                FSLog.Warning($"Client renderTime hard catchup ... ({clientTickTime.Render.Tick}=>{serverTick - 8}) ");
                clientTickTime.Render.SetTick(serverTick - 8, 0);
            }

            // Throttle up to catch up
            if (clientTickTime.Render.Tick < serverTick - 1)
                clientTickTime.Render.AddDuration(frameDuration * 0.01f);

            // FSLog.Info($"serverTick:{serverTick},predictTick:{clientTickTime.predict.Tick}," +
            //    $"renderTick:{clientTickTime.render.Tick},preferredTick:{preferredTick - serverTick}");

            // If predicted time has entered a new tick the stored commands should be sent to server 
            if (clientTickTime.Predict.Tick > prevTick)
            {
                var oldestCommandToSend = (uint) Mathf.Max(prevTick, clientTickTime.Predict.Tick -
                                                                     NetworkConfig.commandClientBufferSize);
                for (var tick = oldestCommandToSend; tick < clientTickTime.Predict.Tick; tick++)
                {
                    inputSystem.StoreCommand(tick);
                    inputSystem.SendCommand(tick);
                    inputSystem.ResetInput();
                }
        
            }
            // Store command
            inputSystem.StoreCommand(clientTickTime.Predict.Tick);

          
            SetSingleton(clientTickTime);
        }
    }
}