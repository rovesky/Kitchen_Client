using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [ExecuteAlways]
    [DisableAutoCreation]
    public class UpdateCameraSystem : SystemBase
    {
        private Vector3 targetPosition;
        private float smoothPosition = 1.5f;
        private EntityQuery gameQuery;
        private EntityQuery flyingQuery;

        protected override void OnCreate()
        {
            gameQuery = GetEntityQuery(typeof(GameStateComponent));
            flyingQuery = GetEntityQuery(typeof(Flying));
            RequireForUpdate(gameQuery);
        }

        protected override void OnUpdate()
        {
            if (flyingQuery.CalculateEntityCount() > 0)
                //跟随主角扔出的道具
                Entities
                    .WithAll<Flying>()
                    .WithStructuralChanges()
                    .ForEach((Entity entity,
                        in OwnerPredictedState ownerState,
                        in LocalToWorld localToWorld) =>
                    {
                       // FSLog.Info($"Flying Camera,entity:{entity}");
                        if(ownerState.PreOwner == Entity.Null)
                            return;

                        if(!HasComponent<LocalCharacter>(ownerState.PreOwner))
                            return;

                        MoveCamera(localToWorld);
                    }).Run();

            else

                //跟随主角
                Entities
                    .WithAll<LocalCharacter>()
                    .WithStructuralChanges()
                    .ForEach((Entity entity,
                        in LocalToWorld localToWorld) =>
                    {

                        var gameStates = gameQuery.ToComponentDataArray<GameStateComponent>(Allocator.TempJob);
                        if (gameStates.Length == 0 || gameStates[0].State != GameState.Playing)
                        {
                            gameStates.Dispose();
                            return;
                        }

                        gameStates.Dispose();

                        MoveCamera(localToWorld);
                    }).Run();
        }

        private void MoveCamera(LocalToWorld localToWorld)
        {
            var camera = CameraManager.Instance.CameraMain;
            // setting the target position to be the correct offset from the 

            var targetX = 0.0f;
            if (localToWorld.Position.x > CameraManager.Instance.RightEdge.position.x)
                targetX = (localToWorld.Position.x - CameraManager.Instance.RightEdge.position.x) * 0.5f;
            else if (localToWorld.Position.x < CameraManager.Instance.LeftEdge.position.x)
                targetX = (localToWorld.Position.x - CameraManager.Instance.LeftEdge.position.x) * 0.5f;


            var targetZ = 0.0f;
            if (localToWorld.Position.z > CameraManager.Instance.TopEdge.position.z)
                targetZ = (localToWorld.Position.z - CameraManager.Instance.RightEdge.position.z) / 5;
            else if (localToWorld.Position.z < CameraManager.Instance.BottomEdge.position.z)
                targetZ = (localToWorld.Position.z - CameraManager.Instance.LeftEdge.position.z) / 5;

            targetPosition = new Vector3(targetX, camera.transform.position.y, targetZ - 24.2f);
            // making a smooth transition between it's current position and the position it wants to be in
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition,
                UnityEngine.Time.deltaTime * smoothPosition);
        }
    }
}