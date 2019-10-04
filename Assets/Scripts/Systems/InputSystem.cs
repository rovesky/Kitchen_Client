using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class InputSystem : ComponentSystem
    {
        // 鼠标射线碰撞层
        public LayerMask InputMask;
        private EntityQuery playerCommandQuery;

        protected override void OnCreate()
        {
          
            base.OnCreate();
            InputMask = 1 << LayerMask.NameToLayer("plane");

            playerCommandQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerCommand>());
            EntityManager.CreateEntity(typeof(PlayerCommand));
            playerCommandQuery.SetSingleton(new PlayerCommand()
            {
                renderTick = 0,
                targetPos = Vector3.zero
            });
        }

        protected override void OnUpdate()
        {       
            var userCommand = playerCommandQuery.GetSingleton<PlayerCommand>();

            userCommand.Reset();
            userCommand.isBack = true;
            //是否开火
            userCommand.buttons.Or(PlayerCommand.Button.PrimaryFire, Input.GetKey(KeyCode.Space));

            //是否移动
            userCommand.buttons.Or(PlayerCommand.Button.Move, Input.GetMouseButton(0));

            //获取点击位置
            if (userCommand.buttons.IsSet(PlayerCommand.Button.Move))
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

                playerCommandQuery.SetSingleton(userCommand);

                //   FSLog.Info($"targetPos:[{targetPos.x},{targetPos.y},{targetPos.z}]");
            }

         
        }
    }
}
