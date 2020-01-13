using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateUISystem : ComponentSystem
    {

        private int frameCount;
        private float passedTime;

        protected override void OnUpdate()
        {
            if (GetEntityQuery(typeof(ServerSnapshot)).CalculateEntityCount() == 0)
                return;

            var snapshotFromServer = GetSingleton<ServerSnapshot>();
            UIManager.Instance.UpdateRtt(snapshotFromServer.Rtt);

            //frameCount++;
            //passedTime += Time.DeltaTime;
            //if (passedTime >= 1.5f)
            //{
            //    UIManager.Instance.UpdateFps(frameCount / passedTime);
            //    frameCount = 0;
            //    passedTime = 0.0f;

            //}
          
        }
    }
}