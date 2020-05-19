using Assets.Kitchen.Scripts.UI;
using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateButton2StateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<LocalCharacter>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState) =>
                {
                    UIManager.Instance.ButtonEnable("Button2", false);
                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    //非食物返回
                    if (!EntityManager.HasComponent<Food>(pickupedEntity))
                        return;

                    UIManager.Instance.ButtonEnable("Button2", true);

                }).Run();
        }

    }
}