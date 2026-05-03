using LogicWorld.Rendering.Components;
using RemoteControl.Client.Menus;
using RemoteControl.Shared.CustomData;

namespace RemoteControl.Client
{
	public class RemoteOutputClient : ComponentClientCode<IRemoteOutputData>
	{
		protected override void SetDataDefaultValues()
		{
			this.Data.Initialize();
		}

		protected override void DataUpdate()
		{
			if (this.Data.Action == RemoteOutputAction.None ||
				this.Data.Action == RemoteOutputAction.Init)
				return ;
			bool _status = this.Data.Status;

			if (this.Data.Action == RemoteOutputAction.Pulse)
				_status = true;
			RemoteControlMenu.SetStatus(this.Address, _status);
		}
	}
}