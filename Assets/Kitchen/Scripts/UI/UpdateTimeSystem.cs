using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateTimeSystem : SystemBase
    {

        private int frameCount;
        private float passedTime;

        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    in Countdown countddown) =>
                {
                    UIManager.Instance.UpdateTime(countddown.Value);
                }).Run();
        }
    }
}