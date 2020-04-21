using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateGameStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    in GameStateComponent gameState) =>
                {
                    if (gameState.State == GameState.Playing)
                    {
                        UIManager.Instance.EnableGame(true);
                        UIManager.Instance.EnablePannelStart(false);
                        UIManager.Instance.EnablePannelEnd(false);
                    }
                    else  if (gameState.State == GameState.Preparing)
                    {
                        UIManager.Instance.EnableGame(false );
                        UIManager.Instance.EnablePannelStart(true);
                        UIManager.Instance.EnablePannelEnd(false);
                    }
                    //else  if (gameState.State == GameState.Ending)
                    //{
                    //    UIManager.Instance.EnableGame(false );
                    //    UIManager.Instance.EnablePannelStart(false );
                    //    UIManager.Instance.EnablePannelEnd(true );
                    //}

                }).Run();
        }
    }
}