using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


namespace BulkPurchaseRework.Patches
{
    [HarmonyPatch(typeof(PlayerNetwork), nameof(PlayerNetwork.OnStartClient))]
    public class AddButton
    {
        public static bool Prefix()
        {
            // Find the Buttons_Bar GameObject
            GameObject buttonsBar = GameObject.Find("Buttons_Bar");

            if (buttonsBar == null)
            {
                return true;
            }

            // Create the "Add All to Cart" button if it doesn't exist
            if (buttonsBar.transform.Find("AddAllToCartButton") == null)
            {
                GameObject addAllButton = CreateButton(buttonsBar, "AddAllToCartButton", -450, 110); // Full width
                AddButtonEvents(addAllButton.GetComponent<Button>(), addAllButton.GetComponent<Image>(), OnAddAllToCartButtonClick);
            }

            // Create the "Change Mode" button if it doesn't exist
            if (buttonsBar.transform.Find("ChangeModeButton") == null)
            {
                GameObject ChangeModeButton = CreateButton(buttonsBar, "ChangeModeButton", 425, 110); // Shifted 800 units to the right
                AddButtonEvents(ChangeModeButton.GetComponent<Button>(), ChangeModeButton.GetComponent<Image>(), OnChangeModeButtonClick);
            }

            // Create the new button if it doesn't exist
            if (buttonsBar.transform.Find("NeedsOnlyButton") == null)
            {
                // Position this button 50px to the right of the "AddAllToCartButton"
                GameObject newButton = CreateButton(buttonsBar, "NeedsOnlyButton", -325, 55); // Half width, 50px right
                AddButtonEvents(newButton.GetComponent<Button>(), newButton.GetComponent<Image>(), OnNeedsOnlyButtonClick);
            }

            return true;
        }

        private static GameObject CreateButton(GameObject parent, string name, float xOffset, float width)
        {
            // Create the button GameObject
            GameObject buttonObject = new GameObject(name);
            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            Button buttonComponent = buttonObject.AddComponent<Button>();
            Image buttonImage = buttonObject.AddComponent<Image>();

            // Set the button's parent to Buttons_Bar
            buttonObject.transform.SetParent(parent.transform, false);

            // Set up RectTransform properties
            rectTransform.sizeDelta = new Vector2(width, 35); // Adjust width here
            rectTransform.anchoredPosition = new Vector2(xOffset, 612);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Create and configure the text component
            GameObject textObject = new GameObject("ButtonText");
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
            Text textComponent = textObject.AddComponent<Text>();

            textRectTransform.sizeDelta = rectTransform.sizeDelta;
            textRectTransform.anchoredPosition = Vector2.zero;

            textComponent.text = name == "AddAllToCartButton" ? "Add All to Cart" :
                                  name == "ChangeModeButton" ? "Mode " + Plugin.CurrentMode.Value : "Needs Only Button";
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = Color.black;
            textComponent.fontStyle = FontStyle.Bold;

            return buttonObject;
        }

        private static void AddButtonEvents(Button button, Image buttonImage, UnityEngine.Events.UnityAction onClickAction)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            // Hover enter event
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnter.callback.AddListener((data) => OnHoverEnter(buttonImage));
            trigger.triggers.Add(pointerEnter);

            // Hover exit event
            EventTrigger.Entry pointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExit.callback.AddListener((data) => OnHoverExit(buttonImage));
            trigger.triggers.Add(pointerExit);

            // Click event (only triggers if hovered)
            button.onClick.AddListener(() =>
            {
                if (buttonImage.color != Color.white)
                {
                    onClickAction.Invoke();
                }
            });
        }

        private static void OnHoverEnter(Image buttonImage)
        {
            buttonImage.color = new Color(5f / 255f, 133f / 255f, 208f / 255f); // Light blue hover color
        }

        private static void OnHoverExit(Image buttonImage)
        {
            buttonImage.color = Color.white; // Revert color when not hovering
        }

        private static void OnAddAllToCartButtonClick()
        {
            ProductListing productListing = GameObject.FindFirstObjectByType<ProductListing>();
            ManagerBlackboard managerBlackboard = GameObject.FindFirstObjectByType<ManagerBlackboard>();

            if (productListing == null || managerBlackboard == null) return;

            foreach (var productPrefab in productListing.productPrefabs)
            {
                var productComponent = productPrefab.GetComponent<Data_Product>();
                if (productComponent != null && productListing.unlockedProductTiers[productComponent.productTier])
                {
                    float boxPrice = productComponent.basePricePerUnit * productComponent.maxItemsPerBox;
                    boxPrice *= productListing.tierInflation[productComponent.productTier];
                    float roundedBoxPrice = Mathf.Round(boxPrice * 100f) / 100f;
                    managerBlackboard.AddShoppingListProduct(productComponent.productID, roundedBoxPrice);
                }
            }
        }

        private static void OnChangeModeButtonClick()
        {
            Plugin.CurrentMode.Value = (Plugin.CurrentMode.Value % 7) + 1;
            Button targetButton = GameObject.Find("ChangeModeButton")?.GetComponent<Button>();
            if (targetButton != null)
            {
                // Aquí puedes acceder y modificar el botón encontrado
                Text textComponent = targetButton.GetComponentInChildren<Text>();  // O TextMeshPro si usas esa
                textComponent.text = "Mode " + Plugin.CurrentMode.Value;
                Canvas canvas = targetButton.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    // Forzar la actualización del Canvas
                    Canvas.ForceUpdateCanvases();

                    // Aseguramos que el layout del botón se actualice
                    LayoutRebuilder.ForceRebuildLayoutImmediate(targetButton.GetComponent<RectTransform>());
                }

            }
        }

        private static void OnNeedsOnlyButtonClick()
        {
            Debug.Log($"Aca1");
            switch (Plugin.CurrentMode.Value)
            {
                case 1:
                    Debug.Log($"Acacase1");
                    somelogic(1);
                    break;
                case 2:
                    Debug.Log($"Acacase2");
                    somelogic(2);
                    break;
                case 3:
                    Debug.Log($"Acacase3");
                    break;
                case 4:
                    Debug.Log($"Acacase4");
                    break;
                case 5:
                    Debug.Log($"Acacase5");
                    break;
                case 6:
                    Debug.Log($"Acacase6");
                    break;
                case 7:
                    Debug.Log($"Acacase7");
                    break;
            }
        }

        private static void somelogic(int mode)
        {
            ProductListing productListing = GameObject.FindFirstObjectByType<ProductListing>();
            ManagerBlackboard managerBlackboard = GameObject.FindFirstObjectByType<ManagerBlackboard>();
            List<int> productIdsList = string.IsNullOrEmpty(Plugin.ProductBlacklist.Value) ? new List<int>() : Plugin.ProductBlacklist.Value.Split(',').Select(str => int.Parse(str)).ToList();

            if (productListing == null || managerBlackboard == null) return;

            int threshold = 0;
            switch (mode)
            {
                case 1:
                    threshold = Plugin.ShelveThreshold.Value;
                    break;
                case 2:
                    threshold = Plugin.StorageThreshold.Value;
                    break;
                case 3:
                    //threshold = Plugin.BoxThreshold.Value;
                    break;
            }
            foreach (var productPrefab in productListing.productPrefabs)
            {
                // GameObject game = productPrefab;
                // Vector3 size = game.GetComponent<BoxCollider>().size;
                var productComponent = productPrefab.GetComponent<Data_Product>();
                // Debug.Log($"Vector: {size}");
                if (productComponent != null && productListing.unlockedProductTiers[productComponent.productTier])
                {
                    int productID = productComponent.productID;
                    if (!productIdsList.Contains(productID))
                    {
                        int[] productExistences = managerBlackboard.GetProductsExistences(productID);
                        if (productExistences[mode-1] < threshold)
                        {
                            float boxPrice = productComponent.basePricePerUnit * productComponent.maxItemsPerBox;
                            boxPrice *= productListing.tierInflation[productComponent.productTier];
                            float roundedBoxPrice = Mathf.Round(boxPrice * 100f) / 100f;
                            managerBlackboard.AddShoppingListProduct(productID, roundedBoxPrice);
                        }
                    }
                }
            }
        }
    }
}