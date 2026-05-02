using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using LogicAPI.Data;

namespace RemoteControl.Client
{
	public class RemoteOutputPlacingRules : DynamicPlacingRulesGenerator<int>
	{
		protected override int GetIdentifierFor(ComponentData componentData)
			=> 0;

		protected override PlacingRules GeneratePlacingRulesFor(int identifier)
		{
			return new PlacingRules
			{
				AllowFineRotation = true,
			};
		}
	}
}
