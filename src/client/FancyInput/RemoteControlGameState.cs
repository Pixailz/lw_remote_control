using System.Collections.Generic;
using FancyInput;
using LogicUI;
using LogicWorld.GameStates;
using RemoteControl.Client.Menus;

namespace RemoteControl.Client
{
    public class RemoteControlOpenMenuGameState : GameState
	{
        public const string Id = "RemoteControlOpenMenu";
        public override string TextID => Id;
        public override bool PlayerCanMoveAndLookAround => false;
        // public override bool ShowHotbarWhileStateActive => false;

        public override IEnumerable<InputTrigger> HelpScreenTriggers => [
            UITrigger.Back,
			RemoteControlTrigger.RemoteControlOpenMenu
		];

        public override void OnEnter()
		{
			RemoteControlMenu.ShowMenu();
        }

		public override void OnRun()
		{
			if (CustomInput.DownThisFrame(UITrigger.Back))
			{
				GameStateManager.TransitionBackToBuildingState();
			}
			else if (CustomInput.DownThisFrame(RemoteControlTrigger.RemoteControlOpenMenu))
			{
				GameStateManager.TransitionBackToBuildingState();
			}
		}

		public override void OnExit()
		{
			RemoteControlMenu.HideMenu();
		}
    }
}