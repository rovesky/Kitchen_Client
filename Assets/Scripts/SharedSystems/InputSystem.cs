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
        private TickStateDenseBuffer<UserCommand> commandBuffer = new TickStateDenseBuffer<UserCommand>(128);
        private Entity localEntity;
        private NetworkClientNewSystem networkClient;

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

            networkClient = World.GetExistingSystem<NetworkClientNewSystem>();

        }

        protected override void OnUpdate()
        {


        }

        private void InputToCommand()
        {
            /*
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
            }*/

            var v = ETCInput.GetAxis("Vertical"); /*检测垂直方向键*/
            var h  = ETCInput.GetAxis("Horizontal"); /*检测水平方向键*/

            var v3 = new Vector3(h, 0, v);
            if (Vector3.SqrMagnitude(v3) < 0.001f)
                userCommand.targetPos = Vector3.zero;

            userCommand.targetPos = v3.normalized;
        }

        public bool HasCommands(uint firstTick, uint lastTick)
        {
            var hasCommands = commandBuffer.FirstTick() <= firstTick &&
                      commandBuffer.LastTick() >= lastTick;
            return hasCommands;
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
            var query = GetEntityQuery(typeof(LocalPlayer));
            if (query.CalculateEntityCount() == 0)
                return;

            var entities = query.ToEntityArray(Unity.Collections.Allocator.Persistent);
            localEntity = entities[0];
            entities.Dispose();

            var userCommand = EntityManager.GetComponentData<UserCommand>(localEntity);
            var found = commandBuffer.TryGetValue((int)tick, ref userCommand);
            //   GameDebug.Assert(found, "Failed to find command for tick:{0}", tick);

            if (found)
                EntityManager.SetComponentData(localEntity, userCommand);
        }

        public void SendCommand(uint tick)
        {
            var command = UserCommand.defaultCommand;
            var commandValid = commandBuffer.TryGetValue((int)tick, ref command);
            if (commandValid)
            {
              //  FSLog.Info($"send command:{command.renderTick},{command.checkTick}");
                //  +    $",{command.buttons.flags},{command.targetPos.x},{command.targetPos.y},{command.targetPos.z}");
                networkClient.QueueCommand(tick, (ref NetworkWriter writer) =>
                 {
                     command.Serialize(ref writer);
                 });
            }
        }

        public void ResetInput()
        {
            userCommand.Reset();
            InputToCommand();
        }
    }
}