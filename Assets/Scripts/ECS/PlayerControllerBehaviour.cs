using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

// An example character controller that uses a rigid body to move
// See CharacterControllerComponent for a 'proxy' based character controller also

public struct PlayerController : IComponentData
{
    public float MovementSpeed;
    public LayerMask InputMask; // 鼠标射线碰撞层
}

public class PlayerControllerBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{

    public float MovementSpeed = 5;
    public LayerMask InputMask; // 鼠标射线碰撞层

    void OnEnable() { }

    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            var componentData = new PlayerController
            {
                MovementSpeed = MovementSpeed,
                InputMask = InputMask
            };

            dstManager.AddComponentData(entity, componentData);
        }
    }
}

[UpdateBefore(typeof(BuildPhysicsWorld))]
public class PlayerControllerSystem : ComponentSystem
{
    protected override  void OnUpdate()
    {
        Entities.ForEach(
            (ref PlayerController ccBodyComponentData,         
            ref Translation position) =>
            {

                if (Input.GetMouseButton(0))
                {
                    // 获得鼠标屏幕位置
                    Vector3 ms = Input.mousePosition;
                    // 将屏幕位置转为射线
                    UnityEngine.Ray ray = Camera.main.ScreenPointToRay(ms);
                    // 用来记录射线碰撞信息
                    UnityEngine.RaycastHit hitinfo;
                    // 产生射线
                    //LayerMask mask =new LayerMask();
                    //mask.value = (int)Mathf.Pow(2.0f, (float)LayerMask.NameToLayer("plane"));
                    bool iscast = Physics.Raycast(ray, out hitinfo, 1000, ccBodyComponentData.InputMask);

                    var targetPos = Vector3.zero;
                    if (iscast)
                    {
                        // 如果射中目标,记录射线碰撞点
                        targetPos = hitinfo.point;
                    }

                    // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
                    Vector3 pos = Vector3.MoveTowards(position.Value, targetPos, ccBodyComponentData.MovementSpeed * Time.deltaTime);
                    // 更新当前位置
                    position.Value = pos;
                }

            }); 
    }
}
