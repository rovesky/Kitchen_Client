using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyBoxAnimSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in Presentation presentation,
                    in BoxOpenRequest request) =>
                {
                    FSLog.Info("animator open!");
                    var animator = EntityManager.GetComponentObject<Animator>(presentation.Value);
                    animator.SetBool("Opening", true);
                    EntityManager.RemoveComponent<BoxOpenRequest>(entity);
                }).Run();

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in Presentation presentation) =>
                {
                    var animator = EntityManager.GetComponentObject<Animator>(presentation.Value);

                    var animatorInfo = animator.GetCurrentAnimatorStateInfo (0);//必须放在update里
                 //   Debug.Log($"animatorInfo.normalizedTime:{animatorInfo.normalizedTime}");
                    if ((animatorInfo.normalizedTime >= 0.7f) && (animatorInfo.IsName("Base Layer.open")))//normalizedTime: 范围0 -- 1,  0是动作开始，1是动作结束
                    {
                        animator.SetBool("Opening",false);
                    }

                }).Run();
        }
    }
}