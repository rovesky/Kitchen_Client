using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateMenuSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<NewEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in Menu menu) =>
                {
                    UIManager.Instance.AddMenu(menu.ProductId,
                        menu.MaterialId1,menu.MaterialId2
                        ,menu.MaterialId3,menu.MaterialId4);

                    EntityManager.RemoveComponent<NewEntity>(entity);
                }).Run();
        }
    }
}