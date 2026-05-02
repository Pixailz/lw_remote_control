
using System.Runtime.CompilerServices;
using EccsGuiBuilder.Client.Components;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsGuiBuilder.Client.Wrappers.RootWrappers;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicAPI.Data;
using LogicInitializable;
using LogicLocalization;
using LogicUI.MenuParts;
using LogicUI.Palettes;
using RemoteControl.Client.Menus;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RemoteControl.Client
{
	public sealed class RemoteOutputMeta
	{
		public string Id {get; }
		public ComponentAddress Address {get; }

		public RemoteOutputMeta(
			string Id,
			ComponentAddress Address
		)
		{
			this.Id = Id;
			this.Address = Address;
		}
	}

	public class RemoteOutputCard :
		UIBehaviour,
		IAssignMyFields
	{
		public static GameObject pattern {private set; get; }

		public static void initialize()
		{
			pattern = VanillaStore.genInnerBox;
			VanillaStore._addInternal(pattern);

			var rectangle = pattern.GetComponent<Rectangle>();
			var outlinePalette = pattern.AddComponent<PaletteRectangleOutline>();
			Fields.getPrivate(outlinePalette.GetType(), "Target").SetValue(outlinePalette, rectangle);
			outlinePalette.SetPaletteColor(PaletteColor.Tertiary);
			rectangle.ShapeProperties.DrawOutline = true;

			new CanvasWrapper(pattern, "RemoteOutputCard")
				.layoutGrowElementHorizontal(elementIndex: IndexHelper.nth(1))
				.add(WS.textLine
					.injectionKey(nameof(text))
					.configureTMP(tmp => {
						tmp.fontSize = 40;
					})
				)
				.addAndConfigure<HoverButton>(hoverButton => {
					// I do not have any wrapper support for default buttons, thus the graphic has to be set manually.
					Fields.getPrivate(hoverButton.GetType(), "TargetGraphic").SetValue(hoverButton, hoverButton.GetComponent<Graphic>());
					hoverButton.SetPaletteColor(PaletteColor.Secondary);
				})
				.add<RemoteOutputCard>()
				.build();

		}


		[AssignMe] [SerializeField]
		private LocalizedTextMesh text;

		private RemoteOutputMeta _meta;

		public RemoteOutputMeta remoteOutputMeta
		{
			get => _meta;
			set
			{
				_meta = value;
				text.SetLocalizationKey(RemoteControlMenu.GetRemoteTitle(_meta), true);
			}
		}

	}
}
