using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCameraSystem : SystemBase
    {
        //private Quaternion targetRotation;
        //private float smoothRotation = 1.5f;
        private Vector3 targetPosition;
        private float smoothPosition = 1.25f;
        protected override void OnUpdate()
        {
            Entities
                .WithAll<LocalCharacter>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in LocalToWorld localToWorld) =>
                {
                    var camera = CameraManager.Instance.CameraMain;
                    // setting the target position to be the correct offset from the 

                    var targetX = 0.0f;
                    if(localToWorld.Position.x > CameraManager.Instance.RightEdge.position.x)
                        targetX =  (localToWorld.Position.x -CameraManager.Instance.RightEdge.position.x)*0.5f ;
                    else if (localToWorld.Position.x < CameraManager.Instance.LeftEdge.position.x)
                        targetX = (localToWorld.Position.x - CameraManager.Instance.LeftEdge.position.x)*0.5f;

                    
                    var targetZ = 0.0f;
                    if(localToWorld.Position.z > CameraManager.Instance.TopEdge.position.z)
                        targetZ =  (localToWorld.Position.z -CameraManager.Instance.RightEdge.position.z)/5 ;
                    else if (localToWorld.Position.z < CameraManager.Instance.BottomEdge.position.z)
                        targetZ = (localToWorld.Position.z - CameraManager.Instance.LeftEdge.position.z)/5;


                    targetPosition = new Vector3(targetX,
                        camera.transform.position.y,
                        targetZ-24.2f);
                    // making a smooth transition between it's current position and the position it wants to be in
                    camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition,
                        UnityEngine.Time.deltaTime * smoothPosition);
                   
                    //targetRotation = Quaternion.Euler(34.3f +  (float)(Mathf.Round(localToWorld.Position.z/4 * 10)) / 10,
                    //    camera.transform.rotation.y,
                    //    camera.transform.rotation.z);

                    //camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, targetRotation,
                    //    UnityEngine.Time.deltaTime * smoothRotation);
                 

                }).Run();
        }
    }
}