using Unity.Entities;

namespace FootStone.Kitchen
{
 
    /// <summary>
    /// 删除NewServerEntity标签
    /// </summary>
    [DisableAutoCreation]
    public class RemoveNewServerEntitySystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
              .ForEach((Entity entity,in NewServerEntity newServerEntity) =>
                {

                    if (newServerEntity.Tick == 0)
                        EntityManager.RemoveComponent<NewServerEntity>(entity);

                }).Run();
        }
    }
}
