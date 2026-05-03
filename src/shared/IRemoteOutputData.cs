namespace RemoteControl.Shared.CustomData
{
	public interface IRemoteOutputData
	{
		bool Status {get; set;}
		RemoteOutputAction Action {get; set; }
		string Id {get; set; }
		int Size {get; set; }
	}

	public enum RemoteOutputAction
	{
		Init = -1,
		None = 0,

		Toggle,
		Pulse,

		SendStatus,
	}

	public static class RemoteOutputDataInit
	{
		public static void Initialize(this IRemoteOutputData data)
		{
			data.Status = false;
			data.Action = RemoteOutputAction.Init;
			data.Id = "";
			data.Size = 1;
		}
	}
}
