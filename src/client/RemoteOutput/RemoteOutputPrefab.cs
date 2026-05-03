using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using JimmysUnityUtilities;
using LogicAPI.Data;
using UnityEngine;

namespace RemoteControl.Client
{
	public class RemoteOutputPrefab : DynamicPrefabGenerator<int>
	{
		private readonly Color24 blockColor = Color24.AlienArmpit;

		protected override int GetIdentifierFor(ComponentData componentData)
			=> 0; // No variants

		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (0, 1);

		protected override Prefab GeneratePrefabFor(int identifier)
		{
			return new Prefab
			{
				Blocks = [
					new Block {
						RawColor = blockColor,
						Position = new Vector3(0f, 0f, 0f),
						Scale = new Vector3(1f, 1f, 1f)
					}
				],
				Inputs = [],
				Outputs = [
					new ComponentOutput {
						Position = new Vector3(
							0f,
							0.5f,
							0.5f
						),
						Rotation = new Vector3(90f, 0f, 0f),
					}
				]
			};
		}
	}
}
