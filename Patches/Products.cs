using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using BulkPurchaseRework;



[HarmonyPatch(typeof(ManagerBlackboard), "CreateUIShopItem")]
public static class BlackboardShopItemPatch
{
    static void Postfix(
        int productIndex,
        ProductListing pListingReference,
        float tinflactionFactor,
        string pricePerUnitLocalized,
        ManagerBlackboard __instance)
    {
        // El objeto se instancia dentro de shopItemsParent, accedemos a su último hijo
        Transform parent = __instance.shopItemsParent.transform;
        Transform newItem = parent.GetChild(parent.childCount - 1);

        if (newItem == null) return;

        // Localiza el AddButton original
        Transform addBtn = newItem.Find("AddButton");
        if (addBtn == null) return;

        // Clonar el botón
        GameObject buyAllBtn = UnityEngine.Object.Instantiate(addBtn.gameObject, addBtn.parent);
        buyAllBtn.name = "BlackListButton";

        var fsmClone = buyAllBtn.GetComponent<PlayMakerFSM>();
        if (fsmClone != null) UnityEngine.Object.Destroy(fsmClone);

        RectTransform rt = addBtn.GetComponent<RectTransform>();
        RectTransform newRt = buyAllBtn.GetComponent<RectTransform>();
        newRt.anchoredPosition = rt.anchoredPosition + new Vector2(rt.sizeDelta.x - 130f, 0f);

        Image[] imagesClone = buyAllBtn.GetComponentsInChildren<Image>(true);
        Image iconImage = imagesClone[0];

        UpdateButtonColor(iconImage, productIndex);

        Button btn = buyAllBtn.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                ToggleBlacklist(productIndex);
                UpdateButtonColor(iconImage, productIndex);
            });
        }
    }

    public static void ToggleBlacklist(int productId)
    {
        var numbers = Plugin.ProductBlacklist.Value?
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
            .Where(n => n.HasValue)
            .Select(n => n.Value)
            .ToList() ?? new List<int>();

        if (numbers.Contains(productId))
            numbers.Remove(productId);
        else
            numbers.Add(productId);

        numbers = numbers.Distinct().OrderBy(n => n).ToList();
        Plugin.ProductBlacklist.Value = string.Join(",", numbers);

        Debug.Log($"Producto {productId} toggled. Nueva blacklist: {Plugin.ProductBlacklist.Value}");
    }

    private static void UpdateButtonColor(Image iconImage, int productIndex)
    {
        if (Plugin.ProductBlacklist.Value.Split(',').Contains(productIndex.ToString()))
            iconImage.color = Color.red;
        else
            iconImage.color = Color.green;
    }
}
