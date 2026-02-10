using System;
using System.Collections.Generic;
using HarmonyLib;
using SFS.Builds;
using SFS.UI;
using SFS.UI.ModGUI;
using UnityEngine;
using UnityEngine.UI;
using Button = SFS.UI.ModGUI.Button;
using Object = UnityEngine.Object;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades.Build
{
	// Token: 0x0200004A RID: 74
	[HarmonyPatch(typeof(PickCategoriesMenu))]
	internal static class ReplacePickCategories
	{
		// Token: 0x06000138 RID: 312
		[HarmonyPrefix]
		[HarmonyPatch("Start")]
		private static void Start(PickCategoriesMenu __instance)
		{
			inst = __instance;
		}

		// Token: 0x06000139 RID: 313
		[HarmonyPrefix]
		[HarmonyPatch("SetupElements")]
		private static bool SetupElements(IReadOnlyCollection<PickGridUI.CategoryParts> picklists)
		{
			categories = picklists.Count;
			if (categories <= 24)
			{
				return true;
			}
			if (window != null && window.gameObject != null)
			{
				Object.Destroy(window.gameObject);
			}
			window = null;
			if (inst == null || inst.gameObject == null)
			{
				var currentInstance = Object.FindFirstObjectByType<PickCategoriesMenu>();
				if (!(currentInstance != null))
				{
					return true;
				}
				inst = currentInstance;
			}
			var buttons = new List<Button>();
			GameObject gameObject = inst.gameObject;
			int num = Math.Clamp(picklists.Count, 1, 24);
			window = Builder.CreateWindow(gameObject.transform, Builder.GetRandomID(), 240, 57 * num + 250, -20, 35, false, true, 0f);
			Transform backGame = window.gameObject.transform.Find("Back (Game)");
			Transform backInGame = window.gameObject.transform.Find("Back (InGame)");
			Transform title = window.gameObject.transform.Find("Title");
			if (backGame != null)
			{
				backGame.gameObject.SetActive(false);
			}
			if (backInGame != null)
			{
				backInGame.gameObject.SetActive(false);
			}
			if (title != null)
			{
				title.gameObject.SetActive(false);
			}
			Transform mask = window.gameObject.transform.Find("Mask");
			if (mask != null)
			{
				var rectMask = mask.GetComponent<RectMask2D>();
				if (rectMask != null)
				{
					rectMask.rectTransform.offsetMax += new Vector2(0f, 20f);
				}
			}
			RectTransform windowRect = window.gameObject.Rect();
			if (windowRect != null)
			{
				windowRect.anchoredPosition = new Vector2(-20f, 35f);
				windowRect.anchorMin = new Vector2(0f, 1f);
				windowRect.anchorMax = new Vector2(0f, 1f);
				windowRect.pivot = new Vector2(0f, 1f);
			}
			window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 5f, new RectOffset(5, 5, 5, 5));
			window.EnableScrolling(Type.Vertical);
			using (IEnumerator<PickGridUI.CategoryParts> enumerator = picklists.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PickGridUI.CategoryParts categoryParts = enumerator.Current;
					Button button = Builder.CreateButton(window, 225, 50, 0, 0, null, categoryParts.tag.displayName.Field);
					var textAdapter = button.gameObject.GetComponentInChildren<TextAdapter>();
					if (textAdapter != null)
					{
						textAdapter.transform.localScale = new Vector3(0.75f, 0.75f);
					}

					button.OnClick = (Action)Delegate.Combine(button.OnClick, new Action(delegate
					{
						inst.SelectCategory(categoryParts);
						foreach (Button button2 in buttons)
						{
							button2.gameObject.GetComponent<ButtonPC>().SetSelected(false);
						}
						button.gameObject.GetComponent<ButtonPC>().SetSelected(true);
					}));
					buttons.Add(button);
				}
			}

			if (buttons.Count <= 0) return false;
			var buttonPC = buttons[0].gameObject.GetComponent<ButtonPC>();
			if (buttonPC != null)
			{
				buttonPC.SetSelected(true);
			}
			return false;
		}

		// Token: 0x0600013A RID: 314
		[HarmonyPrefix]
		[HarmonyPatch("SelectCategory")]
		private static bool SelectCategory(PickGridUI.CategoryParts newSelected, ref PickGridUI.CategoryParts ___selected)
		{
			if (categories <= 24)
			{
				return true;
			}
			inst.expandMenu.Close();
			___selected = newSelected;
			BuildManager.main.pickGrid.OpenCategory(newSelected);
			return false;
		}

		// Token: 0x040000D7 RID: 215
		private static Window window;

		// Token: 0x040000D8 RID: 216
		private static PickCategoriesMenu inst;

		// Token: 0x040000D9 RID: 217
		private static int categories;
	}
}
