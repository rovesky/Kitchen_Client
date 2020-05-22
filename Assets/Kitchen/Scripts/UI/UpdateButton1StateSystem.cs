//using Unity.Entities;


//namespace FootStone.Kitchen
//{
//    [DisableAutoCreation]
//    public class UpdateButton1StateSystem : SystemBase
//    {
//        protected override void OnUpdate()
//        {
//            Entities
//                .WithAll<LocalCharacter>()
//                .WithoutBurst()
//                .ForEach((Entity entity,
//                    in SlotPredictedState slotState,
//                    in TriggerPredictedState triggerState) =>
//                {
//                    UIManager.Instance.UpdateButtonIcon("Button1", "demo_cookzone_btn_act");

//                    var pickupedEntity = slotState.FilledIn;
//                    var triggeredEntity = triggerState.TriggeredEntity;

//                    //图标变为cut
//                    if (pickupedEntity == Entity.Null 
//                        && triggeredEntity != Entity.Null 
//                        && EntityManager.HasComponent<TableSlice>(triggeredEntity))
//                    {

//                        var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerState.TriggeredEntity);
//                        if (slot.FilledIn != Entity.Null && EntityManager.HasComponent<Unsliced>(slot.FilledIn))
//                        {
//                            UIManager.Instance.UpdateButtonIcon("Button1", "demo_cookzone_btn_cut");

//                        }
//                    }
//                    else if (pickupedEntity == Entity.Null 
//                             && triggeredEntity != Entity.Null
//                             && EntityManager.HasComponent<TableSink>(triggeredEntity))
//                    {
//                        var sink = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
//                        if (!sink.Value.IsEmpty())
//                            UIManager.Instance.UpdateButtonIcon("Button1", "demo_cookzone_btn_wash");
//                    }
                   
                    

//                }).Run();
//        }

//    }
//}