using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class UpdateHealthUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
          //  Debug.Log($"UpdateHealthUISystem OnUpdate");
            Entities.WithAllReadOnly<UpdateUI>().ForEach((Entity entity, ref Health health, ref Score score) =>
            {
                GameManager.Instance.ChangeLife(health.Value);
                GameManager.Instance.UpdateScore(score);

            });
        }
    }
}