using LogicWorld.Rendering.Components;

using RemoteControl.Shared.CustomData;

namespace RemoteControl.Client
{
	public class RemoteOutputClient : ComponentClientCode<IRemoteOutputData>
	{
		protected override void SetDataDefaultValues()
		{
			this.Data.Initialize();
		}
	}
}