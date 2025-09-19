using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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
        GameObject buyAllBtn = Object.Instantiate(addBtn.gameObject, addBtn.parent);
        buyAllBtn.name = "BlackListButton";

        
        RectTransform rt = addBtn.GetComponent<RectTransform>();
        RectTransform newRt = buyAllBtn.GetComponent<RectTransform>();
        newRt.anchoredPosition = rt.anchoredPosition + new Vector2(rt.sizeDelta.x - 100f, 0f); 
        // Preparar lógica del nuevo botón
        Button btn = buyAllBtn.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"Añadir a la lista negra {productIndex}");
                // Aquí tu lógica de compra masiva.
                // Por ejemplo:
                // __instance.AddFullBox(productIndex);
            });
        }
    }
}
