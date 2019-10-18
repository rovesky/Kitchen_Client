using System;
using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class InputSystem : FSComponentSystem
    {
        // 鼠标射线碰撞层
        private LayerMask InputMask;
  
        private UserCommand userCommand = UserCommand.defaultCommand;
        private TickStateDenseBuffer<UserCommand> commandBuffer = new TickStateDenseBuffer<UserCommand>(32);
        private Entity localEntity;
        private NetworkClientSystem networkClient;

        protected override void OnCreate()
        {          
            base.OnCreate();
            InputMask = 1 << LayerMask.NameToLayer("plane");
  
            EntityManager.CreateEntity(typeof(UserCommand));
            SetSingleton(new UserCommand()
            {
                checkTick = 0,
                renderTick = 0,
                targetPos = Vector3.zero
            });

            networkClient = World.GetExistingSystem<NetworkClientSystem>();
            
        }

        protected override void OnUpdate()
        {
            /**
            var userCommand = GetSingleton<UserCommand>();

            userCommand.Reset();
            //  userCommand.isBack = true;
            //是否开火
            userCommand.buttons.Or(UserCommand.Button.PrimaryFire, Input.GetKey(KeyCode.Space));

            //是否移动
            userCommand.buttons.Or(UserCommand.Button.Move, Input.GetMouseButton(0));

            //获取点击位置
            if (userCommand.buttons.IsSet(UserCommand.Button.Move))
            {
                // 获得鼠标屏幕位置
                Vector3 ms = Input.mousePosition;
                // 将屏幕位置转为射线
                Ray ray = Camera.main.ScreenPointToRay(ms);
                // 用来记录射线碰撞信息
                RaycastHit hitInfo;

                // 产生射线                 
                bool isCast = Physics.Raycast(ray, out hitInfo, 1000, InputMask);

                var targetPos = Vector3.zero;
                if (isCast)
                {
                    // 如果射中目标,记录射线碰撞点
                    targetPos = hitInfo.point;
                }
                userCommand.targetPos = targetPos;

                //   FSLog.Info($"targetPos:[{targetPos.x},{targetPos.y},{targetPos.z}]");
            }

            SetSingleton(userCommand);
            **/

            var query = GetEntityQuery(typeof(LocalPlayer));
            if (query.CalculateEntityCount() > 0) {
                var entities = query.ToEntityArray(Unity.Collections.Allocator.Persistent);
                localEntity = entities[0];
                entities.Dispose();
            }
        }

        private void InputToCommand()
        {
            //是否开火
            userCommand.buttons.Or(UserCommand.Button.PrimaryFire, Input.GetKey(KeyCode.Space));

            //是否移动
            userCommand.buttons.Or(UserCommand.Button.Move, Input.GetMouseButton(0));

            //获取点击位置
            if (userCommand.buttons.IsSet(UserCommand.Button.Move))
            {
                // 获得鼠标屏幕位置
                Vector3 ms = Input.mousePosition;
                // 将屏幕位置转为射线
                Ray ray = Camera.main.ScreenPointToRay(ms);
                // 用来记录射线碰撞信息
                RaycastHit hitInfo;
                // 产生射线                 
                bool isCast = Physics.Raycast(ray, out hitInfo, 1000, InputMask);

                var targetPos = Vector3.zero;
                if (isCast)
                {
                    // 如果射中目标,记录射线碰撞点
                    targetPos = hitInfo.point;
                }
                userCommand.targetPos = targetPos;
                //   FSLog.Info($"targetPos:[{targetPos.x},{targetPos.y},{targetPos.z}]");
            }
        }
    

        public void SampleInput(uint renderTick)
        {
            InputToCommand();
            userCommand.renderTick = renderTick;
        }

        public void StoreCommand(uint tick)
        {
            userCommand.checkTick = tick;

            var lastBufferTick = commandBuffer.LastTick();
            if (tick != lastBufferTick && tick != lastBufferTick + 1)
            {
                commandBuffer.Clear();
                GameDebug.Log(string.Format("Trying to store tick:{0} but last buffer tick is:{1}. Clearing buffer", tick, lastBufferTick));
            }

            if (tick == lastBufferTick)
                commandBuffer.Set(ref userCommand, (int)tick);
            else
                commandBuffer.Add(ref userCommand, (int)tick);

        }

        public void RetrieveCommand(uint tick)
        {
            if (localEntity == default)
                return;

            var userCommand = EntityManager.GetComponentData<UserCommand>(localEntity);

            var command = UserCommand.defaultCommand;
            var found = commandBuffer.TryGetValue((int)tick, ref userCommand);
            GameDebug.Assert(found, "Failed to find command for tick:{0}", tick);

            EntityManager.SetComponentData(localEntity, userCommand);
        }

        public  void SendCommand(uint tick)
        {
            var command = UserCommand.defaultCommand;
            var commandValid = commandBuffer.TryGetValue((int)tick, ref command);
            if (commandValid)
            {
                networkClient.SendCommand(command.ToData());
            }
        }

        public  void ResetInput()
        {
            userCommand.Reset();
            InputToCommand();
        }
    }
}
