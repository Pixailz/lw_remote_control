using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsGuiBuilder.Client.Layouts.Helper;
using System.Collections.Generic;
using LogicWorld.Interfaces;
using LogicUI.MenuTypes;
using LogicAPI.Data;

using UnityEngine;
using UnityEngine.UI;


using LogicUI.MenuParts;
using LogicLocalization;
using LICC;
using RemoteControl.Shared.CustomData;
using EccsGuiBuilder.Client.Layouts.Elements;
using LogicWorld.Players;
using LogicWorld.GameStates;

using JetBrains.Annotations;
using EccsGuiBuilder.Client.Wrappers.RootWrappers;
using EccsLogicWorldAPI.Client.UnityHelper;
using EccsGuiBuilder.Client.Wrappers.Specialized;

namespace RemoteControl.Client.Menus
{
	public class RemoteControlMenu :
		ToggleableSingletonMenu<RemoteControlMenu>,
		IAssignMyFields
	{
		public static void init()
		{
			WS.window("RemoteControlMenu")
				.setLocalizedTitle("RemoteControl - Menu")
				.setYPosition(null)
				.configureContent(content => content
					.layoutGrowElementHorizontalInner()
					.add(WS.scrollableVertical
						.fixedSize(1000, 800)
						.addAndConfigure<LayoutElement>(layout => {
							layout.minWidth = 300f;
							layout.preferredWidth = 500f;
							layout.minHeight = 200f;
							layout.preferredHeight = 300f;
						})
							.configureContent(content => content
								.setAlignment(Alignment.Top)
								.addAndConfigure<ContentSizeFitter>(fitter => {
									fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
									fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
								})
									.injectionKey(nameof(scrollContent))
									.layoutGrowGap(
										padding: new RectOffset(15, 15, 10, 10),
										gapIndex: IndexHelper.Last
									)
							)
					)
					.addContainer(
						"RemoteControlMenuDetails", container => container
						.injectionKey(nameof(detailsSection))
						.fixedSize(1000, 800)
						.layoutVertical(
							expandChildThickness: false
						)
					)
				)
				.add<RemoteControlMenu>()
				.build();

			OnMenuShown += Instance.onMenuShown;
			OnMenuHidden += Instance.onMenuHidden;
		}

		[AssignMe]
		private GameObject scrollContent;

		[AssignMe]
		private GameObject detailsSection;

		private static RemoteOutputCard currentCard = null;


		private void onMenuShown()
		{
			detailsSection.SetActive(false);
			RefreshList();
		}

		private void onMenuHidden()
		{
		}

		private void RefreshList()
		{
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
			RemoteOutputClient	comp_client_code = (RemoteOutputClient)Instances.MainWorld.Renderer.Entities.GetClientCode(comp.Key);
			RemoteOutputCard	card = Instantiate(RemoteOutputCard.pattern, scrollContent.gameObject.transform)
				.GetComponent<RemoteOutputCard>();
			card.remoteOutputMeta = new RemoteOutputMeta(
				comp_client_code.Data.Id,
				comp.Key
			);

			HoverButton Focus = card.GetComponent<HoverButton>();
			Focus.OnClickEnd += () =>
			{
				testFocus(card);
			};
		}

		private void testFocus(RemoteOutputCard card)
		{
			currentCard = card;
			detailsSection.SetActive(true);
		}

		public static string GetRemoteTitle(RemoteOutputMeta meta)
		{
			return meta.Id == "" ? meta.Address.ToString() : meta.Id;
		}
	}
}