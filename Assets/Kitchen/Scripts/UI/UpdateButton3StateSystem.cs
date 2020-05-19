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
                    if (rushState.CurCooldownTick == 0) 
                        return;
                    
                   
                    var worldTime = GetSingleton<WorldTime>();
                    var time = rushSetting.CooldownTick * worldTime.GameTick.TickInterval;

                    //FSLog.Info($"UpdateButton3StateSystem,rushState.CurCooldownTick:{rushState.CurCooldownTick},time:{time} ");
                    UIManager.Instance.PlayCD("Button3", time);
                }).Run();
        }

    }
}