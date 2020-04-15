using System.Collections.Generic;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateMenuSystem : SystemBase
    {
        private Dictionary<EntityType,int>  icons = new Dictionary<EntityType, int>();
      
        protected override void OnCreate()
        {
            icons[EntityType.ShrimpSlice] = 5;
            icons[EntityType.CucumberSlice] = 6;
            icons[EntityType.KelpSlice] = 7;
            icons[EntityType.RiceCooked] = 1;
        }

        private int TypeToIcon(int type)
        {
            if (!icons.ContainsKey((EntityType)type))
                return 0;
            return icons[(EntityType) type];
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<NewEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in Menu menu) =>
                {
                    UIManager.Instance.AddMenu(menu.ProductId,
                        TypeToIcon(menu.MaterialId1),
                        TypeToIcon(menu.MaterialId2),
                        TypeToIcon(menu.MaterialId3),
                        TypeToIcon(menu.MaterialId4));

                    EntityManager.RemoveComponent<NewEntity>(entity);
                }).Run();
        }
    }
}