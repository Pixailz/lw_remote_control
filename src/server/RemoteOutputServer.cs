using LogicWorld.Server.Circuitry;
using RemoteControl.Shared.CustomData;

namespace RemoteControl.Server
{
	public class RemoteOutputServer : LogicComponent<IRemoteOutputData>
	{
		public override bool	HasPersistentValues => true;

		protected override void SetDataDefaultValues()
		{
			this.Data.Initialize();
		}

		protected override void DoLogicUpdate()
		{
			if (this.Data.Action == RemoteOutputAction.None)
			{
				return ;
			}
			switch (this.Data.Action)
			{
				case RemoteOutputAction.Init:
					InitOutput();
				break;
				case RemoteOutputAction.Toggle:
					ToggleOutput();
				break;
				case RemoteOutputAction.Pulse:
					PulseOutput();
				break;
			}
			if (this.Data.Action == RemoteOutputAction.SendStatus)
			{
				this.Data.Status = Outputs[0].On;
				this.Data.Action = RemoteOutputAction.None;
				return ;
			}
		}

		protected override void OnCustomDataUpdated()
		{
			QueueLogicUpdate();
		}

		private void InitOutput()
		{
			Outputs[0].On = this.Data.Status;
			this.Data.Action = RemoteOutputAction.SendStatus;
		}

		private void ToggleOutput()
		{
			Outputs[0].On = !Outputs[0].On;
			this.Data.Action = RemoteOutputAction.SendStatus;
		}

		private void PulseOutput()
		{
			if (Outputs[0].On)
			{
				Outputs[0].On = false;
				this.Data.Action = RemoteOutputAction.SendStatus;
			}
			else
			{
				Outputs[0].On = true;
				QueueLogicUpdate();
			}
		}
	}
}
