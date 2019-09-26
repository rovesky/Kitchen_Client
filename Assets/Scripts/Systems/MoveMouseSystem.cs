using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class MoveMouseSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref MoveMouse moveMouse,ref UserCommand userCommand, ref Translation position) =>
            {
                if (userCommand.buttons.IsSet(UserCommand.Button.PrimaryFire))
                {                  
                    // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
                    Vector3 pos = Vector3.MoveTowards(position.Value, userCommand.targetPos, moveMouse.Speed * Time.deltaTime);
                    // 更新当前位置
                    position.Value = pos;
                }
            });
        }
    }
}
