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
            Entities.ForEach((ref MoveMouse moveMouse, ref Translation position) =>
            {
                if (Input.GetMouseButton(0))
                {
                    // 获得鼠标屏幕位置
                    Vector3 ms = Input.mousePosition;
                    // 将屏幕位置转为射线
                    Ray ray = Camera.main.ScreenPointToRay(ms);
                    // 用来记录射线碰撞信息
                    RaycastHit hitInfo;
                    // 产生射线
                    //LayerMask mask =new LayerMask();
                    //mask.value = (int)Mathf.Pow(2.0f, (float)LayerMask.NameToLayer("plane"));
                    bool isCast = Physics.Raycast(ray, out hitInfo, 1000, moveMouse.InputMask);

                    var targetPos = Vector3.zero;
                    if (isCast)
                    {
                        // 如果射中目标,记录射线碰撞点
                        targetPos = hitInfo.point;
                    }

                    // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
                    Vector3 pos = Vector3.MoveTowards(position.Value, targetPos, moveMouse.Speed * Time.deltaTime);
                    // 更新当前位置
                    position.Value = pos;
                }
            });
        }
    }
}
