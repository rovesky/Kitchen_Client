using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [ExecuteAlways]
    [DisableAutoCreation]
    public class HandleTimeSystem : FSComponentSystem
    {
        private double lastFrameTime = 0;
        private float frameTimeScale = 1.0f;
        private InputSystem inputSystem;

        protected override void OnCreate()
        {          
            EntityManager.CreateEntity(typeof(ClientTickTime));
            SetSingleton(new ClientTickTime()
            {
                predict = new GameTick(30),
                render = new GameTick(30)
            });

            inputSystem = World.GetOrCreateSystem<InputSystem>();
        }

        protected override void OnUpdate()
        {         
            var serverTick = GetSingleton<SnapshotFromServer>().tick;
            var clientTickTime = GetSingleton<ClientTickTime>();
            var worldTime = GetSingleton<WorldTime>();

            float frameDuration = lastFrameTime != 0 ? (float)(worldTime.frameTime - lastFrameTime) : 0;
            lastFrameTime = worldTime.frameTime;

            inputSystem.SampleInput(clientTickTime.render.Tick);
            
            // handle pridect tick
            uint prevTick = clientTickTime.predict.Tick;

            // Increment time
            var deltaPredictedTime = frameDuration * frameTimeScale;
            clientTickTime.predict.AddDuration(deltaPredictedTime);

            // Adjust time to be synchronized with server
            uint preferredBufferedCommandCount = 2;
            uint preferredTick = serverTick + (uint)((120 / 1000.0f) * worldTime.tick.TickRate) + preferredBufferedCommandCount;

            FSLog.Info($"serverTick:{serverTick},predictTick:{clientTickTime.predict.Tick}," +
                $"renderTick:{clientTickTime.render.Tick},preferredTick:{preferredTick}");

            bool resetTime = false;
            if (!resetTime && clientTickTime.predict.Tick < preferredTick - 3)
            {
                FSLog.Info($"Client predictTime hard catchup ... ");
                resetTime = true;
            }

            if (!resetTime && clientTickTime.predict.Tick > preferredTick + 6)
            {
                FSLog.Info($"Client predictTime hard slowdown ... ");
                resetTime = true;
            }

            frameTimeScale = 1.0f;
            if (resetTime)
            {
                FSLog.Info(string.Format("CATCHUP ({0} -> {1})", clientTickTime.predict.Tick, preferredTick));
                // m_NetworkStatistics.notifyHardCatchup = true;
                //  m_GameWorld.nextTickTime = Game.frameTime;             
                clientTickTime.predict.SetTick(preferredTick, 0);
            }
            else
            {
                //int bufferedCommands = m_NetworkClient.lastAcknowlegdedCommandTime - serverTick;
                //if (bufferedCommands < preferredBufferedCommandCount)
                //    frameTimeScale = 1.01f;

                //if (bufferedCommands > preferredBufferedCommandCount)
                //    frameTimeScale = 0.99f;
            }

            //handle render tick
            // Increment interpolation time
            clientTickTime.render.AddDuration(frameDuration * frameTimeScale);

            // Force interp time to not exeede server time
            if (clientTickTime.render.Tick >= serverTick)
            {
                FSLog.Info($"Client renderTime hard slowdown ... ");
                clientTickTime.render.SetTick(serverTick, 0);
            }

            // hard catchup
            if (clientTickTime.render.Tick < serverTick - 10)
            {
                FSLog.Info($"Client renderTime hard catchup ... ");
                clientTickTime.render.SetTick(serverTick - 8, 0);
            }

            // Throttle up to catch up
            if (clientTickTime.render.Tick < serverTick - 1)
            {
                clientTickTime.render.AddDuration(frameDuration * 0.01f);
            }

            // If predicted time has entered a new tick the stored commands should be sent to server 
            if (clientTickTime.predict.Tick > prevTick)
            {
                var oldestCommandToSend = (uint)Mathf.Max(prevTick, clientTickTime.predict.Tick - NetworkConfig.commandClientBufferSize);
                for (uint tick = oldestCommandToSend; tick < clientTickTime.predict.Tick; tick++)
                {
                     inputSystem.StoreCommand(tick);
                     inputSystem.SendCommand(tick);
                }
                inputSystem.ResetInput();               
            }
            // Store command
            inputSystem.StoreCommand(clientTickTime.predict.Tick);

            SetSingleton(clientTickTime);
        }
    }

}
