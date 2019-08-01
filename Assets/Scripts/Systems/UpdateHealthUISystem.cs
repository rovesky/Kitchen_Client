using Unity.Entities;

namespace Assets.Scripts.ECS
{
    public class UpdateHealthUISystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<UpdateHealthUI>().ForEach((Entity entity, ref Health health, ref Score score) =>
            {
                GameManager.Instance.ChangeLife(health.Value);
                GameManager.Instance.UpdateScore(score);

            });
        }
    }
}