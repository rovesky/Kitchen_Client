using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateButton2StateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<LocalCharacter>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in TriggerPredictedState triggerState) =>
                {

                    UIManager.Instance.ButtonEnable("Button2", false);
                    UIManager.Instance.UpdateButtonIcon("Button2", "demo_cookzone_btn_throw");

                    var pickupedEntity = slotState.FilledIn;
                    var triggeredEntity = triggerState.TriggeredEntity;

                    //图标变为cut
                    if (pickupedEntity == Entity.Null 
                        && triggeredEntity != Entity.Null 
                        && EntityManager.HasComponent<TableSlice>(triggeredEntity))
                    {

                        var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerState.TriggeredEntity);
                        if (slot.FilledIn != Entity.Null && EntityManager.HasComponent<Unsliced>(slot.FilledIn))
                        {
                            UIManager.Instance.ButtonEnable("Button2", true);
                            UIManager.Instance.UpdateButtonIcon("Button2", "demo_cookzone_btn_cut");
                        }
                    }
                    else if (pickupedEntity == Entity.Null 
                             && triggeredEntity != Entity.Null
                             && EntityManager.HasComponent<TableSink>(triggeredEntity))
                    {
                        var sink = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                        if (!sink.Value.IsEmpty())
                        {
                            UIManager.Instance.ButtonEnable("Button2", true);
                            UIManager.Instance.UpdateButtonIcon("Button2", "demo_cookzone_btn_wash");
                        }
                    }
                    else if (pickupedEntity != Entity.Null && EntityManager.HasComponent<Food>(pickupedEntity))
                    {
                        UIManager.Instance.ButtonEnable("Button2", true);
                        UIManager.Instance.UpdateButtonIcon("Button2", "demo_cookzone_btn_throw");
                    }
                    else if (pickupedEntity != Entity.Null && EntityManager.HasComponent<Extinguisher>(pickupedEntity))
                    {
                        var extinguisherState = EntityManager.GetComponentData<ExtinguisherPredictedState>(pickupedEntity);
                        UIManager.Instance.ButtonEnable("Button2", true);
                        var icon = extinguisherState.Distance == 0
                            ? "demo_cookzone_btn_extinguisher"
                            : "demo_cookzone_btn_unextinguisher";
                        UIManager.Instance.UpdateButtonIcon("Button2", icon);
                    }

                }).Run();
        }

    }
}