using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Editor tool to quickly setup the inventory system
/// Window → Inventory → Setup Tool
/// </summary>
public class InventorySetupTool : EditorWindow
{
    private GameObject player;
    private Canvas canvas;
    private GameObject floatingTextPrefab;
    private GameObject inventorySlotPrefab;

    private Vector2 scrollPosition;

    [MenuItem("Window/Inventory/Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<InventorySetupTool>("Inventory Setup Tool");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Inventory System Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "This tool will help you automatically setup the inventory system.\n" +
            "Please ensure Player and Canvas exist in the scene.",
            MessageType.Info);

        EditorGUILayout.Space();

        // ===== Step 1: Setup Player =====
        DrawSectionHeader("Step 1: Setup Player");
        player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);

        if (player != null)
        {
            InventorySystem invSystem = player.GetComponent<InventorySystem>();
            if (invSystem == null)
            {
                EditorGUILayout.HelpBox("Player doesn't have InventorySystem component", MessageType.Warning);
                if (GUILayout.Button("Add InventorySystem to Player"))
                {
                    Undo.AddComponent<InventorySystem>(player);
                    EditorUtility.DisplayDialog("Success", "Added InventorySystem component to Player", "OK");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Player has InventorySystem", MessageType.Info);
            }
        }

        EditorGUILayout.Space();

        // ===== Step 2: Setup Canvas =====
        DrawSectionHeader("Step 2: Setup Canvas");
        canvas = (Canvas)EditorGUILayout.ObjectField("Canvas", canvas, typeof(Canvas), true);

        if (canvas != null)
        {
            EditorGUILayout.HelpBox("✓ Canvas is set", MessageType.Info);
        }

        EditorGUILayout.Space();

        // ===== Step 3: Create Prefabs =====
        DrawSectionHeader("Step 3: Create UI Prefabs");

        floatingTextPrefab = (GameObject)EditorGUILayout.ObjectField(
            "FloatingText Prefab", 
            floatingTextPrefab, 
            typeof(GameObject), 
            false);

        if (floatingTextPrefab == null && canvas != null)
        {
            if (GUILayout.Button("Create FloatingText Prefab"))
            {
                CreateFloatingTextPrefab();
            }
        }

        EditorGUILayout.Space();

        inventorySlotPrefab = (GameObject)EditorGUILayout.ObjectField(
            "InventorySlot Prefab", 
            inventorySlotPrefab, 
            typeof(GameObject), 
            false);

        if (inventorySlotPrefab == null && canvas != null)
        {
            if (GUILayout.Button("Create InventorySlot Prefab"))
            {
                CreateInventorySlotPrefab();
            }
        }

        EditorGUILayout.Space();

        // ===== Step 4: Setup Scene UI =====
        DrawSectionHeader("Step 4: Setup Scene UI");

        GUI.enabled = canvas != null && floatingTextPrefab != null && inventorySlotPrefab != null;

        if (GUILayout.Button("Create Complete Backpack UI", GUILayout.Height(40)))
        {
            CreateBackpackUI();
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        // ===== Step 5: Create Test Items =====
        DrawSectionHeader("Step 5: Create Test Items (Optional)");

        if (GUILayout.Button("Create Test Red Key ItemData"))
        {
            CreateTestItemData();
        }

        EditorGUILayout.Space();

        // ===== Quick Actions =====
        DrawSectionHeader("Quick Actions");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto Find Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>()?.gameObject;
            }
            if (player == null)
            {
                EditorUtility.DisplayDialog("Notice", "Player not found, please specify manually", "OK");
            }
        }

        if (GUILayout.Button("Auto Find Canvas"))
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Notice", "Canvas not found, please create one manually", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSectionHeader(string title)
    {
        EditorGUILayout.Space();
        GUILayout.Label(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void CreateFloatingTextPrefab()
    {
        // Create FloatingText GameObject
        GameObject floatingText = new GameObject("FloatingText");
        floatingText.transform.SetParent(canvas.transform, false);

        // Add RectTransform
        RectTransform rectTransform = floatingText.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);

        // Add TextMeshProUGUI
        TextMeshProUGUI text = floatingText.AddComponent<TextMeshProUGUI>();
        text.text = "Test Text";
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;

        // Add FloatingText component
        FloatingText floatingTextComponent = floatingText.AddComponent<FloatingText>();

        // Save as prefab
        string prefabPath = "Assets/Prefabs/UI/FloatingText.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(floatingText, prefabPath);
        floatingTextPrefab = prefab;

        // Destroy temporary object
        DestroyImmediate(floatingText);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"FloatingText Prefab created:\n{prefabPath}", "OK");
    }

    private void CreateInventorySlotPrefab()
    {
        // Create InventorySlot GameObject
        GameObject slot = new GameObject("InventorySlot");
        slot.transform.SetParent(canvas.transform, false);

        // Add RectTransform and Image
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(64, 64);
        Image slotImage = slot.AddComponent<Image>();
        slotImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Add Button
        Button button = slot.AddComponent<Button>();

        // Create Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slot.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        // Create Icon
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(slot.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.sizeDelta = new Vector2(-10, -10);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.raycastTarget = false;
        iconImage.preserveAspect = true;

        // Create QuantityText
        GameObject quantityObj = new GameObject("QuantityText");
        quantityObj.transform.SetParent(slot.transform, false);
        RectTransform quantityRect = quantityObj.AddComponent<RectTransform>();
        quantityRect.anchorMin = new Vector2(1, 0);
        quantityRect.anchorMax = new Vector2(1, 0);
        quantityRect.pivot = new Vector2(1, 0);
        quantityRect.anchoredPosition = new Vector2(-5, 5);
        quantityRect.sizeDelta = new Vector2(40, 20);
        TextMeshProUGUI quantityText = quantityObj.AddComponent<TextMeshProUGUI>();
        quantityText.text = "x1";
        quantityText.fontSize = 16;
        quantityText.alignment = TextAlignmentOptions.BottomRight;
        quantityText.color = Color.white;
        quantityText.outlineWidth = 0.2f;
        quantityText.outlineColor = Color.black;

        // Create EmptyOverlay
        GameObject overlay = new GameObject("EmptyOverlay");
        overlay.transform.SetParent(slot.transform, false);
        RectTransform overlayRect = overlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.7f);
        overlayImage.raycastTarget = false;

        // Add InventorySlotUI component
        InventorySlotUI slotUI = slot.AddComponent<InventorySlotUI>();
        
        // Use reflection to set private fields
        var slotUIType = typeof(InventorySlotUI);
        slotUIType.GetField("backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(slotUI, bgImage);
        slotUIType.GetField("iconImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(slotUI, iconImage);
        slotUIType.GetField("quantityText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(slotUI, quantityText);
        slotUIType.GetField("emptyOverlay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(slotUI, overlayImage);

        // Save as prefab
        string prefabPath = "Assets/Prefabs/UI/InventorySlot.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slot, prefabPath);
        inventorySlotPrefab = prefab;

        // Destroy temporary object
        DestroyImmediate(slot);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"InventorySlot Prefab created:\n{prefabPath}", "OK");
    }

    private void CreateBackpackUI()
    {
        // Create FloatingTextManager
        GameObject floatingTextManager = new GameObject("FloatingTextManager");
        floatingTextManager.transform.SetParent(canvas.transform, false);
        FloatingTextManager ftm = floatingTextManager.AddComponent<FloatingTextManager>();
        
        // Set floatingTextPrefab using reflection
        var ftmType = typeof(FloatingTextManager);
        ftmType.GetField("floatingTextPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(ftm, floatingTextPrefab);

        // Create BackpackPanel (positioned at top-right corner)
        GameObject backpackPanel = new GameObject("BackpackPanel");
        backpackPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = backpackPanel.AddComponent<RectTransform>();
        // Top-right corner anchoring
        panelRect.anchorMin = new Vector2(1, 1);  // Top-right
        panelRect.anchorMax = new Vector2(1, 1);  // Top-right
        panelRect.pivot = new Vector2(1, 1);      // Pivot at top-right
        panelRect.anchoredPosition = new Vector2(-20, -20);  // 20px offset from corner
        panelRect.sizeDelta = new Vector2(400, 600);  // Fixed size
        Image panelImage = backpackPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);  // Semi-transparent background

        // Create Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(backpackPanel.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);  // Top-left of panel
        titleRect.anchorMax = new Vector2(1, 1);  // Top-right of panel
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -10);
        titleRect.sizeDelta = new Vector2(0, 40);  // Full width, 40 height
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "Inventory";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // Create SlotContainer
        GameObject slotContainer = new GameObject("SlotContainer");
        slotContainer.transform.SetParent(backpackPanel.transform, false);
        RectTransform containerRect = slotContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);  // Bottom-left
        containerRect.anchorMax = new Vector2(1, 1);  // Top-right (fill)
        containerRect.pivot = new Vector2(0.5f, 1);   // Top center
        containerRect.anchoredPosition = new Vector2(0, -60);  // Below title
        containerRect.sizeDelta = new Vector2(-20, -70);  // Padding
        GridLayoutGroup grid = slotContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(70, 70);
        grid.spacing = new Vector2(10, 10);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;  // 4 columns for compact display

        // Create UseHintPanel
        GameObject useHintPanel = new GameObject("UseHintPanel");
        useHintPanel.transform.SetParent(canvas.transform, false);
        RectTransform hintRect = useHintPanel.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.5f, 0);
        hintRect.anchorMax = new Vector2(0.5f, 0);
        hintRect.pivot = new Vector2(0.5f, 0);
        hintRect.anchoredPosition = new Vector2(0, 100);
        hintRect.sizeDelta = new Vector2(300, 60);
        Image hintImage = useHintPanel.AddComponent<Image>();
        hintImage.color = new Color(0, 0, 0, 0.7f);

        // Create UseHintText
        GameObject hintTextObj = new GameObject("UseHintText");
        hintTextObj.transform.SetParent(useHintPanel.transform, false);
        RectTransform hintTextRect = hintTextObj.AddComponent<RectTransform>();
        hintTextRect.anchorMin = Vector2.zero;
        hintTextRect.anchorMax = Vector2.one;
        hintTextRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI hintText = hintTextObj.AddComponent<TextMeshProUGUI>();
        hintText.text = "Press E to use item";
        hintText.fontSize = 20;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = Color.yellow;

        // Create BackpackUI
        GameObject backpackUI = new GameObject("BackpackUI");
        backpackUI.transform.SetParent(canvas.transform, false);
        BackpackUI bpUI = backpackUI.AddComponent<BackpackUI>();

        // Set references using reflection
        var bpUIType = typeof(BackpackUI);
        bpUIType.GetField("slotContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, slotContainer.transform);
        bpUIType.GetField("slotPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, inventorySlotPrefab);
        bpUIType.GetField("inventoryPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, backpackPanel);
        bpUIType.GetField("useHintText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, hintText);
        bpUIType.GetField("useHintPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, useHintPanel);
        bpUIType.GetField("alwaysVisible", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(bpUI, true);  // Set to always visible by default

        // BackpackPanel starts visible (always shown in top-right)
        backpackPanel.SetActive(true);
        useHintPanel.SetActive(false);

        EditorUtility.SetDirty(canvas.gameObject);

        EditorUtility.DisplayDialog("Success", 
            "Backpack UI created at top-right corner!\n\n" +
            "Created objects:\n" +
            "- FloatingTextManager\n" +
            "- BackpackPanel (always visible at top-right)\n" +
            "- UseHintPanel (shows when items can be used)\n" +
            "- BackpackUI (controller, set to alwaysVisible)\n\n" +
            "Next steps:\n" +
            "1. Ensure Player has InventorySystem component\n" +
            "2. Add ItemData to Player → InventorySystem → All Game Items\n" +
            "3. Adjust BackpackPanel position/size if needed", 
            "OK");
    }

    private void CreateTestItemData()
    {
        string path = "Assets/ScriptableObjects/Items";
        System.IO.Directory.CreateDirectory(path);

        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.itemId = "key_red";
        itemData.itemName = "Red Key";
        itemData.description = "Can open red doors";
        itemData.itemCategory = ItemCategory.Key;
        itemData.useType = ItemUseType.AutoUse;
        itemData.isConsumable = true;
        itemData.maxStackSize = 1;
        itemData.interactableTag = "door_red";
        itemData.useMessage = "Used {itemName}";
        itemData.canUseHintMessage = "Press E to use {itemName}";

        string assetPath = $"{path}/Key_Red.asset";
        AssetDatabase.CreateAsset(itemData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(itemData);
        Selection.activeObject = itemData;

        EditorUtility.DisplayDialog("Success", 
            $"Test Red Key ItemData created:\n{assetPath}\n\n" +
            "Remember to:\n" +
            "1. Set Icon image\n" +
            "2. Add to Player's InventorySystem → All Game Items\n" +
            "3. Create collectable item in scene", 
            "OK");
    }
}

