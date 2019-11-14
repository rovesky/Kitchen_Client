using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Assets.Scripts.ECS
{
	[DisableAutoCreation]
	public class ClientTriggerProcessSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
		}

		protected override void OnUpdate()
		{
			Entities.ForEach((Entity entity, ref OnTriggerEnter enter) =>
			{
				var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
				var newMat = new Material(volumeRenderMesh.material);
				newMat.color = Color.white;
				volumeRenderMesh.material = newMat;

				PostUpdateCommands.SetSharedComponent<RenderMesh>(entity, volumeRenderMesh);

				EntityManager.RemoveComponent<OnTriggerEnter>(entity);
			});

			Entities.ForEach((Entity entity, ref OnTriggerExit enter) =>
			{
				var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
				var newMat = new Material(volumeRenderMesh.material);
				newMat.color = new Color(0.945f, 0.635f, 0.184f);
				volumeRenderMesh.material = newMat;

				PostUpdateCommands.SetSharedComponent<RenderMesh>(entity, volumeRenderMesh);

				EntityManager.RemoveComponent<OnTriggerExit>(entity);
			});
		}
		
	}
}
