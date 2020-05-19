using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateButton3StateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<LocalCharacter>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in RushPredictState rushState,
                    in RushSetting rushSetting) =>
                {
                    if (!rushState.IsRushed) 
                        return;
                    
                 //   FSLog.Info($"UpdateButton3StateSystem,rushState.CurCooldownTick:{rushState.CurCooldownTick} ");
                    var worldTime = GetSingleton<WorldTime>();
                    UIManager.Instance.PlayCD("Button3", rushSetting.CooldownTick * worldTime.TickDuration);
                }).Run();
        }

    }
}