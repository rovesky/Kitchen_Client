using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class UpdateHealthUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
          //  Debug.Log($"UpdateHealthUISystem OnUpdate");
            Entities.WithAllReadOnly<UpdateHealthUI>().ForEach((Entity entity, ref Health health, ref Score score) =>
            {
                GameManager.Instance.ChangeLife(health.Value);
                GameManager.Instance.UpdateScore(score);

            });
        }
    }
}