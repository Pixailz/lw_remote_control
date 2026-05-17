using System.Collections.Generic;

using LogicWorld.Interfaces;
using LogicWorld.Players;
using LogicWorld.GameStates;
using LogicUI.MenuTypes;
using LogicUI.MenuParts;
using LogicAPI.Data;
using LogicLocalization;

using UnityEngine;
using UnityEngine.UI;

using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Layouts.Helper;


using RemoteControl.Shared.CustomData;
using LogicUI.Palettes;

namespace RemoteControl.Client.Menus
{
	public class RemoteControlMenu :
		ToggleableSingletonMenu<RemoteControlMenu>,
		IAssignMyFields
	{
		[AssignMe]
		private GameObject scrollContent;

		[AssignMe]
		private GameObject detailsSection;

		[AssignMe]
		private LocalizedTextMesh detailsTitle;
		[AssignMe]
		private LocalizedTextMesh detailsId;
		[AssignMe]
		private LocalizedTextMesh detailsAddress;

		[AssignMe]
		private HoverButton buttonOnOff;
		[AssignMe]
		private HoverButton buttonPulse;
		[AssignMe]
		private HoverButton buttonTeleportTo;

		private static RemoteOutputCard currentCard = null;
		private static RemoteOutputCard lastCard = null;

		private readonly List<RemoteOutputCard> instantiatedRemoteOutputCards = [];

		public static void init()
		{
			WS.window("RemoteControlMenu")
				.setLocalizedTitle("RemoteControl - Menu")
				.setYPosition(null)
				.doNotBlurBuildingCanvas()
				.configureContent(content => content
					.layoutGrowElementHorizontalInner()
					.add(WS.scrollableVertical
						.fixedSize(800, 800)
						.addAndConfigure<LayoutElement>(layout => {
							layout.minWidth = 300f;
							layout.preferredWidth = 500f;
							layout.minHeight = 200f;
							layout.preferredHeight = 300f;
						})
							.configureContent(content => content
								.injectionKey(nameof(scrollContent))
								.setAlignment(Alignment.Top)
								.addAndConfigure<ContentSizeFitter>(fitter => {
									fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
									fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
								})
									.layoutGrowGap(
										padding: new RectOffset(15, 15, 10, 10),
										gapIndex: IndexHelper.Last
									)
							)
					)
					.addContainer(
						"RemoteControlMenuDetails", container => container
						.injectionKey(nameof(detailsSection))
						.fixedSize(600, 800)
						.layoutVertical(
							expandChildThickness: false
						)
						.add(WS.textLine
							.injectionKey(nameof(detailsTitle))
						)
						.addContainer(
							"detailsLineAddress", container => container
							.layoutHorizontal(expandChildThickness: false)
							.add(WS.textLine
								.setLocalizationKey("RemoteControl.gui.RemoteMenuDetails.Address")
							)
							.add(WS.textLine
								.injectionKey(nameof(detailsAddress))
							)
						)
						.addContainer(
							"detailsLineId", container => container
							.layoutHorizontal(expandChildThickness: false)
							.add(WS.textLine
								.setLocalizationKey("RemoteControl.gui.RemoteMenuDetails.Id")
							)
							.add(WS.textLine
								.injectionKey(nameof(detailsId))
							)
						)
						.addContainer(
							"buttonActions", container => container
							.layoutHorizontal(expandChildThickness: false)
							.add(WS.button
								.injectionKey(nameof(buttonOnOff))
								.setLocalizationKey("⏻")
								.fixedSize(80, 80)
								.add<ButtonLayout>()
							)
							.add(WS.button
								.injectionKey(nameof(buttonPulse))
								.setLocalizationKey("⚡")
								.fixedSize(80, 80)
								.add<ButtonLayout>()
							)
							.add(WS.button
								.injectionKey(nameof(buttonTeleportTo))
								.setLocalizationKey("📍")
								.fixedSize(80, 80)
								.add<ButtonLayout>()
							)
						)
					)
				)
				.add<RemoteControlMenu>()
				.build();
		}

		public static void initOnce()
		{
			OnMenuShown += () => Instance.onMenuShown();
			OnMenuHidden += () => Instance.onMenuHidden();
		}

		private void onMenuShown()
		{
			currentCard = null;
			detailsSection.SetActive(false);
			RefreshScrollArea();
			buttonOnOff.OnClickEnd += toggleOutput;
			buttonPulse.OnClickEnd += pulseOutput;
			buttonTeleportTo.OnClickEnd += teleportToRemoteComponent;
		}

		private void onMenuHidden()
		{
			buttonOnOff.OnClickEnd -= toggleOutput;
			buttonPulse.OnClickEnd -= pulseOutput;
			buttonTeleportTo.OnClickEnd -= teleportToRemoteComponent;
		}

		private void RefreshScrollArea()
		{
			foreach (RemoteOutputCard card in instantiatedRemoteOutputCards)
				DestroyImmediate(card.gameObject);

			instantiatedRemoteOutputCards.Clear();

			ComponentType remoteOutputType = Instances.MainWorld.ComponentTypes.GetComponentType("RemoteControl.RemoteOutput");

			foreach (var comp in Instances.MainWorld.Data.AllComponents)
			{
				if (comp.Value.Data.Type == remoteOutputType)
				{
					AddComponentToScrollArea(comp);
				}
			}
		}

		private void AddComponentToScrollArea(
			KeyValuePair<ComponentAddress, ComponentDataManager> comp
		)
		{
			RemoteOutputClient	comp_client_code = GetClientCode(comp.Key);
			RemoteOutputCard	card = Instantiate(
				RemoteOutputCard.pattern,
				scrollContent.transform
			).GetComponent<RemoteOutputCard>();
			instantiatedRemoteOutputCards.Add(card);

			card.remoteOutputMeta = new RemoteOutputMeta(
				comp_client_code.Data.Id,
				comp.Key
			);
			card.GetComponent<HoverButton>()
				.OnClickEnd += () =>
				{
					SetFocus(card);
				};
			CardSetStatus(card, comp_client_code.GetOutputState(0), false);
		}

		private void SetFocus(RemoteOutputCard card)
		{
			lastCard = currentCard;
			currentCard = card;

			card.GetComponent<HoverButton>()
				.SetPaletteColor(PaletteColor.Accent);

			if (lastCard != currentCard && lastCard != null)
			{
				lastCard.GetComponent<HoverButton>()
					.SetPaletteColor(PaletteColor.Primary);
			}

			detailsSection.SetActive(true);
			detailsTitle.SetLocalizationKey(
				GetRemoteTitle(card.remoteOutputMeta), true
			);
			detailsAddress.SetLocalizationKey(
				card.remoteOutputMeta.Address.ToString(), true
			);
			detailsId.SetLocalizationKey(
				card.remoteOutputMeta.Id, true
			);
		}

		public static string GetRemoteTitle(RemoteOutputMeta meta)
		{
			return meta.Id == "" ? meta.Address.ToString() : meta.Id;
		}

		public static RemoteOutputClient GetClientCode(ComponentAddress addr)
		{
			return (RemoteOutputClient)Instances.MainWorld.Renderer.Entities
				.GetClientCode(addr);
		}

		public static RemoteOutputClient GetCurrentClientCode()
		{
			return GetClientCode(currentCard.remoteOutputMeta.Address);
		}

		public static void toggleOutput()
		{
			GetCurrentClientCode().Data.Action = RemoteOutputAction.Toggle;
		}

		public static void pulseOutput()
		{
			GetCurrentClientCode().Data.Action = RemoteOutputAction.Pulse;
		}

		public static void teleportToRemoteComponent()
		{
			PlayerControllerManager.TeleportSelf(
				GetCurrentClientCode().Component.WorldPosition
			);
			GameStateManager.TransitionBackToBuildingState();
		}

		public static void CardSetStatus(
			RemoteOutputCard card,
			bool status,
			bool inverted = true
		)
		{
			string _status;

			// going for the inverse. don't know why :)
			// if (inverted)
			// {
			// 	if (status)
			// 		_status = "❎️";
			// 	else
			// 		_status = "✅️";
			// }
			// else
			// {
			// 	if (status)
			// 		_status = "✅️";
			// 	else
			// 		_status = "❎️";
			// }

			// Simplier
			if (inverted ^ status)
				_status = "<color=#0a0>✅️</color>";
			else
				_status = "<color=#a00>❎️</color>";

			card.status.SetLocalizationKey(_status, true);
		}

		public static void SetStatus(ComponentAddress address, bool status)
		{
			foreach (RemoteOutputCard card in Instance.instantiatedRemoteOutputCards)
			{
				if (card.remoteOutputMeta.Address == address)
				{
					CardSetStatus(card, status);
				}
			}
		}
	}
}
