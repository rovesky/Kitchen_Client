using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateTimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    in GameStateComponent gameState,
                    in  Countdown countdown) =>
                {
                     UIManager.Instance.UpdateTime(gameState.State,countdown.Value);
                
                }).Run();
        }
    }
}