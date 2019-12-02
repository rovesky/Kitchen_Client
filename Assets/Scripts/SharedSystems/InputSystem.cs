using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class InputSystem : ComponentSystem
    {
        private TickStateDenseBuffer<UserCommand> commandBuffer = new TickStateDenseBuffer<UserCommand>(128);
        private NetworkClientSystem networkClient;
        private UserCommand userCommand = UserCommand.defaultCommand;

        protected override void OnCreate()
        {
            base.OnCreate();

            EntityManager.CreateEntity(typeof(UserCommand));
            SetSingleton(new UserCommand
            {
                checkTick = 0,
                renderTick = 0,
                targetPos = Vector3.zero
            });

            networkClient = World.GetExistingSystem<NetworkClientSystem>();
        }

        protected override void OnUpdate()
        {
        }

        private void InputToCommand()
        {
            var v = ETCInput.GetAxis("Vertical"); /*检测垂直方向键*/
            var h = ETCInput.GetAxis("Horizontal"); /*检测水平方向键*/

            var v3 = new Vector3(h, 0, v);
            if (Vector3.SqrMagnitude(v3) < 0.00001f)
                userCommand.targetPos = Vector3.zero;
            else
                userCommand.targetPos = v3.normalized;

            userCommand.buttons.Set(UserCommand.Button.Pickup, UIInput.GetButtonClick("pickup"));
            userCommand.buttons.Set(UserCommand.Button.Throw, UIInput.GetButtonClick("throw"));

            if (userCommand.buttons.flags > 0)
                FSLog.Info($"is set pick:{userCommand.buttons.IsSet(UserCommand.Button.Pickup)}" +
                           $",is set throw:{userCommand.buttons.IsSet(UserCommand.Button.Throw)}");
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

            if (userCommand.buttons.flags > 0)
                FSLog.Info($"StoreCommand buffer count:{userCommand.checkTick},{userCommand.buttons.flags}");

            var lastBufferTick = commandBuffer.LastTick();
            if (tick != lastBufferTick && tick != lastBufferTick + 1)
            {
                commandBuffer.Clear();
                GameDebug.Log(string.Format("Trying to store tick:{0} but last buffer tick is:{1}. Clearing buffer",
                    tick, lastBufferTick));
            }

            if (tick == lastBufferTick)
                commandBuffer.Set(ref userCommand, (int) tick);
            else
                commandBuffer.Add(ref userCommand, (int) tick);
        }

        public void RetrieveCommand(uint tick)
        {
            var localEntity = GetSingleton<LocalPlayer>().PlayerEntity;
            if (localEntity == Entity.Null)
                return;

            var userCommand = EntityManager.GetComponentData<UserCommand>(localEntity);
            //   FSLog.Info($"current command:{userCommand.checkTick},{userCommand.buttons.flags}");
            var found = commandBuffer.TryGetValue((int) tick, ref userCommand);
            GameDebug.Assert(found, "Failed to find command for tick:{0}", tick);
            //   if(userCommand.buttons.flags> 0)
           // FSLog.Info($"{found},retrieve command:{tick},{userCommand.checkTick}," +
                     //  $"{userCommand.buttons.flags},userCommand.targetPos.x:{userCommand.targetPos.x},userCommand.targetPos.z:{userCommand.targetPos.z}");

            if (found)
                EntityManager.SetComponentData(localEntity, userCommand);
        }

        public void SendCommand(uint tick)
        {
            var command = UserCommand.defaultCommand;
            var commandValid = commandBuffer.TryGetValue((int) tick, ref command);
            if (commandValid)
            {
                if (command.buttons.flags > 0)
                    FSLog.Info(
                        $"send command:{command.renderTick},{command.checkTick},{command.buttons.IsSet(UserCommand.Button.Pickup)}");
                //   +    $",{command.buttons.flags},{command.targetPos.x},{command.targetPos.y},{command.targetPos.z}");
                networkClient.QueueCommand(tick, (ref NetworkWriter writer) => { command.Serialize(ref writer); });
            }
        }

        public void ResetInput()
        {
            userCommand.Reset();

            UIInput.ResetButtonClick();
            InputToCommand();
        }
    }
}