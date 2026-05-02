using LogicAPI.Server;

namespace RemoteControl.Server
{
	public class RemoteControlServer : ServerMod
	{
		protected override void Initialize()
		{
			Logger.Info("[✔️] Server: loaded RemoteControl");
		}
	}
}
