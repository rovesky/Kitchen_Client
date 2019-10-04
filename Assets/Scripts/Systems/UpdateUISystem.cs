using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
 
    [DisableAutoCreation]
    public class UpdateHealthUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {      
            Entities.WithAllReadOnly<UpdateUI>().ForEach((Entity entity, ref Health health, ref Score score) =>
            {
                GameManager.Instance.ChangeLife(health.Value);
                GameManager.Instance.UpdateScore(score);
            });
        }
    }
}