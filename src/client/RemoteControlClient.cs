using LogicAPI.Client;
using System;
using EccsLogicWorldAPI.Client.Hooks;
using LogicWorld;
using RemoteControl.Client.Menus;
using FancyInput;
using RemoteControl.Client;
using LogicWorld.GameStates;

namespace RemoteControl
{
	public class RemoteControl : ClientMod
	{
		protected override void Initialize()
		{
			RegisterInput();
			LoadMenus();
			Logger.Info("[✔️] Client: loaded RemoteControl");
		}

		public void RegisterInput()
		{
			CustomInput.Register<RemoteControlContext, RemoteControlTrigger>("RemoteControl");

            FirstPersonInteraction.RegisterBuildingKeybinding(
                RemoteControlTrigger.RemoteControlOpenMenu,
                () => {
                    GameStateManager.TransitionTo(RemoteControlOpenMenuGameState.Id);
                    return true;
                }
            );
		}

		public void LoadMenus()
		{
			WorldHook.worldLoading += () => {
				try
				{
					RemoteOutputCard.initialize();
					RemoteControlMenu.init();
					RemoteOutputMenu.init();
				}
				catch(Exception e)
				{
					Logger.Error("❌ Failed to initialize RemoteControl Menus");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
