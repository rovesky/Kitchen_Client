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
        private UserCommand userCommand = UserCommand.DefaultCommand;

        protected override void OnCreate()
        {
            base.OnCreate();

            EntityManager.CreateEntity(typeof(UserCommand));
            SetSingleton(UserCommand.DefaultCommand);

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
            userCommand.TargetDir = Vector3.SqrMagnitude(v3) < 0.00001f ? Vector3.zero : v3.normalized;

            userCommand.Buttons.Set(UserCommand.Button.Pickup, UIInput.GetButtonClick("pickup"));
            userCommand.Buttons.Set(UserCommand.Button.Throw, UIInput.GetButtonClick("throw"));

        //    if (userCommand.buttons.flags > 0)
        //        FSLog.Info($"is set pick:{userCommand.buttons.IsSet(UserCommand.Button.Pickup)}" +
        //                   $",is set throw:{userCommand.buttons.IsSet(UserCommand.Button.Throw)}");
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
            userCommand.RenderTick = renderTick;
        }

        public void StoreCommand(uint tick)
        {
            userCommand.CheckTick = tick;

            var lastBufferTick = commandBuffer.LastTick();
            if (tick != lastBufferTick && tick != lastBufferTick + 1)
            {
                commandBuffer.Clear();
                GameDebug.Log($"Trying to store tick:{tick} but last buffer tick is:{lastBufferTick}. Clearing buffer");
            }

            if (tick == lastBufferTick)
                commandBuffer.Set(ref userCommand, (int) tick);
            else
                commandBuffer.Add(ref userCommand, (int) tick);

            //if (userCommand.buttons.flags > 0)
            //{
            //    FSLog.Info($"StoreCommand buffer count:{userCommand.checkTick},{userCommand.buttons.flags}");
            //    //for (var i = Mathf.Max(commandBuffer.LastTick() - 5, 0); i <= commandBuffer.LastTick(); i++)
            //    //{
            //    //    var command = UserCommand.defaultCommand;
            //    //    commandBuffer.TryGetValue(i, ref command);

            //    //    FSLog.Info($"StoreCommand buffer :{i},{command.checkTick},{command.buttons.flags}");

            //    //}
            //}
        }

        public void RetrieveCommand(uint tick)
        {
            var localEntity = GetSingleton<LocalPlayer>().PlayerEntity;
            if (localEntity == Entity.Null)
                return;

            var command = EntityManager.GetComponentData<UserCommand>(localEntity);
            //   FSLog.Info($"current command:{userCommand.checkTick},{userCommand.buttons.flags}");
            var found = commandBuffer.TryGetValue((int) tick, ref command);
            //if (command.buttons.flags > 0)
            //    FSLog.Info($"{found},retrieve command:{tick},{command.checkTick}," +
            //               $"{command.buttons.flags},userCommand.targetPos.x:{command.targetPos.x},userCommand.targetPos.z:{command.targetPos.z}");

            GameDebug.Assert(found, "Failed to find command for tick:{0}", tick);
            EntityManager.SetComponentData(localEntity, found ? command : UserCommand.DefaultCommand);
        }

        public void SendCommand(uint tick)
        {
            var command = UserCommand.DefaultCommand;
            var commandValid = commandBuffer.TryGetValue((int) tick, ref command);
            if (commandValid)
            {
                if (command.Buttons.Flags > 0)
                    FSLog.Info(
                        $"send command:{command.RenderTick},{command.CheckTick},{command.Buttons.IsSet(UserCommand.Button.Pickup)}");
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