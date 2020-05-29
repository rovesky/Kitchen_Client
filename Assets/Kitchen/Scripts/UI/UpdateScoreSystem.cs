using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateScoreSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    in Score score) =>
                {
                    UIManager.Instance.UpdateScore(score.Value);
                }).Run();
        }
    }
}