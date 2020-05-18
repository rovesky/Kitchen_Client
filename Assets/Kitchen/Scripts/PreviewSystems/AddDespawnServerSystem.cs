using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class AddDespawnServerSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            
            Entities
                .WithAll<Despawn>()
                .WithStructuralChanges()
                .WithNone<DespawnServer>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.AddComponentData(entity, new DespawnServer());
                }).Run();
        }

      
    }
}