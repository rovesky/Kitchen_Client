using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class AddItemComponentSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            
            Entities
                .WithAll<Item>()
                .WithStructuralChanges()
                .WithNone<UIObject>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.AddComponentData(entity, new UIObject());
                }).Run();
        }

      
    }
}