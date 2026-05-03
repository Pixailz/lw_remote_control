using UnityEngine;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsGuiBuilder.Client.Layouts.Helper;
using System.Collections.Generic;
using LogicWorld.UI;
using TMPro;
using LogicUI.MenuParts;
using EccsGuiBuilder.Client.Layouts.Elements;

namespace RemoteControl.Client.Menus
{
	public class RemoteOutputMenu : EditComponentMenu, IAssignMyFields
	{
		public static void init()
		{
			WS.window("Remote Output Menu")
				.setLocalizedTitle("RemoteControl - Remote Output")
				.configureContent(content => content
					.layoutVertical()
					.addContainer("TopBox", box => box
						.layoutVertical()
						.add(WS.textLine
							.injectionKey(nameof(currentIdPromptText))
							.setLocalizationKey("RemoteControl.CurrentIdPrompt")
						)
						.add(WS.inputField
							.injectionKey(nameof(idField))
							.setPlaceholderLocalizationKey("RemoteControl.IdFieldHint")
							.fixedSize(1000, 80)
						)
					)
					.addContainer("ButtonBox", box => box
						.layoutHorizontal()
						.add(WS.button.setLocalizationKey("RemoteControl.IdSaveButton")
							.injectionKey(nameof(idSaveButton))
							.add<ButtonLayout>()
						)
						.add(WS.button.setLocalizationKey("RemoteControl.IdResetButton")
							.injectionKey(nameof(idResetButton))
							.add<ButtonLayout>()
						)
					)
					.addContainer("BottomBox", box => box
						.layoutVertical()
						.add(WS.textLine
							.setLocalizationKey("RemoteControl.IdSaved")
							.injectionKey(nameof(savedIdText))
						)
					)
				)
				.add<RemoteOutputMenu>()
				.build();
		}

		[AssignMe]
		public GameObject currentIdPromptText = null;
		[AssignMe]
		public GameObject savedIdText = null;

		[AssignMe]
		public TMP_InputField idField;
		[AssignMe]
		public HoverButton idSaveButton;
		[AssignMe]
		public HoverButton idResetButton;

		protected override void OnStartEditing()
		{
			savedIdText.SetActive(false);

			this.ResetId();
		}

		public override void Initialize()
		{
			base.Initialize();

			currentIdPromptText.SetActive(true);

			idSaveButton.OnClickEnd += this.SaveId;
			idResetButton.OnClickEnd += this.ResetId;

			idField.onValueChanged.AddListener(text => {
				savedIdText.SetActive(false);
			});
		}

		private void ResetId()
		{
			savedIdText.SetActive(false);
			idField.text = (
				FirstComponentBeingEdited.ClientCode as RemoteOutputClient
			).Data.Id;
		}

		private void SaveId()
		{
			(
				FirstComponentBeingEdited.ClientCode as RemoteOutputClient
			).Data.Id = idField.text;
			savedIdText.SetActive(true);
		}

		protected override IEnumerable<string> GetTextIDsOfComponentTypesThatCanBeEdited()
		{
			return [
				"RemoteControl.RemoteOutput",
			];
		}
	}
}
