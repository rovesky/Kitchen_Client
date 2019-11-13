using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class PickSystem : ComponentSystem
    {

        protected override void OnCreate()
        {
            base.OnCreate();             

        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref UserCommand command) =>
            {
              //  FSLog.Info("PickSystem Update");
                if (command.buttons.IsSet(UserCommand.Button.Pick))
                {
                    FSLog.Info("Pick up");
                }

            });

        }      
    }
}