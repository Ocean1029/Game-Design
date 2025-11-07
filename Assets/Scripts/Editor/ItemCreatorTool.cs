using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to quickly create collectable items in the scene
/// Right-click in Hierarchy → Inventory → Create Collectable Item
/// </summary>
public class ItemCreatorTool : Editor
{
    [MenuItem("GameObject/Inventory/Create Collectable Item", false, 0)]
    public static void CreateCollectableItem()
    {
        // Create GameObject
        GameObject item = new GameObject("CollectableItem");
        
        // Position at scene view camera or at origin
        if (SceneView.lastActiveSceneView != null)
        {
            item.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
        }

        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;

        // Add Collider2D
        CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        // Add CollectableItem component
        item.AddComponent<CollectableItem>();

        // Register undo and select
        Undo.RegisterCreatedObjectUndo(item, "Create Collectable Item");
        Selection.activeGameObject = item;

        Debug.Log("CollectableItem created! Please set ItemData.");
    }

    [MenuItem("GameObject/Inventory/Create ItemData Asset", false, 1)]
    public static void CreateItemDataAsset()
    {
        ItemDataCreatorWindow.ShowWindow();
    }
}

/// <summary>
/// Window for creating ItemData assets
/// </summary>
public class ItemDataCreatorWindow : EditorWindow
{
    private string itemId = "item_001";
    private string itemName = "New Item";
    private string description = "Item description";
    private Sprite icon;
    private ItemCategory category = ItemCategory.QuestItem;
    private ItemUseType useType = ItemUseType.Manual;
    private bool isConsumable = false;
    private int maxStackSize = 1;
    private string interactableTag = "";

    public static void ShowWindow()
    {
        GetWindow<ItemDataCreatorWindow>("Create ItemData");
    }

    void OnGUI()
    {
        GUILayout.Label("Create New ItemData", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Quickly create ItemData ScriptableObject\n" +
            "Fill in the info below and click 'Create'",
            MessageType.Info);

        EditorGUILayout.Space();

        // Basic Info
        GUILayout.Label("Basic Info", EditorStyles.boldLabel);
        itemId = EditorGUILayout.TextField("Item ID", itemId);
        itemName = EditorGUILayout.TextField("Item Name", itemName);
        description = EditorGUILayout.TextField("Description", description);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);

        EditorGUILayout.Space();

        // Properties
        GUILayout.Label("Properties", EditorStyles.boldLabel);
        category = (ItemCategory)EditorGUILayout.EnumPopup("Category", category);
        useType = (ItemUseType)EditorGUILayout.EnumPopup("Use Type", useType);
        isConsumable = EditorGUILayout.Toggle("Consumable", isConsumable);
        maxStackSize = EditorGUILayout.IntField("Max Stack", maxStackSize);

        EditorGUILayout.Space();

        // Usage
        GUILayout.Label("Usage Settings", EditorStyles.boldLabel);
        interactableTag = EditorGUILayout.TextField("Interactable Tag", interactableTag);

        EditorGUILayout.Space();

        // Quick Templates
        GUILayout.Label("Quick Templates", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Red Key"))
        {
            itemId = "key_red";
            itemName = "Red Key";
            description = "Can open red doors";
            category = ItemCategory.Key;
            useType = ItemUseType.AutoUse;
            isConsumable = true;
            maxStackSize = 1;
            interactableTag = "door_red";
        }

        if (GUILayout.Button("Blue Key"))
        {
            itemId = "key_blue";
            itemName = "Blue Key";
            description = "Can open blue doors";
            category = ItemCategory.Key;
            useType = ItemUseType.AutoUse;
            isConsumable = true;
            maxStackSize = 1;
            interactableTag = "door_blue";
        }

        if (GUILayout.Button("Tool"))
        {
            itemId = "tool_hammer";
            itemName = "Hammer";
            description = "Can be used to repair things";
            category = ItemCategory.Tool;
            useType = ItemUseType.Manual;
            isConsumable = false;
            maxStackSize = 1;
            interactableTag = "repair_spot";
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Create button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create ItemData", GUILayout.Height(40)))
        {
            CreateItemData();
        }
        GUI.backgroundColor = Color.white;
    }

    private void CreateItemData()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            EditorUtility.DisplayDialog("Error", "Item ID cannot be empty!", "OK");
            return;
        }

        string path = "Assets/ScriptableObjects/Items";
        System.IO.Directory.CreateDirectory(path);

        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.itemId = itemId;
        itemData.itemName = itemName;
        itemData.description = description;
        itemData.icon = icon;
        itemData.itemCategory = category;
        itemData.useType = useType;
        itemData.isConsumable = isConsumable;
        itemData.maxStackSize = maxStackSize;
        itemData.interactableTag = interactableTag;
        itemData.useMessage = $"Used {itemName}";
        itemData.canUseHintMessage = $"Press E to use {itemName}";

        string fileName = itemName.Replace(" ", "_");
        string assetPath = $"{path}/{fileName}.asset";
        
        // Check if file exists
        if (System.IO.File.Exists(assetPath))
        {
            if (!EditorUtility.DisplayDialog("Confirm", 
                $"File already exists:\n{assetPath}\nOverwrite?", 
                "Overwrite", "Cancel"))
            {
                return;
            }
        }

        AssetDatabase.CreateAsset(itemData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(itemData);
        Selection.activeObject = itemData;

        EditorUtility.DisplayDialog("Success", 
            $"ItemData created:\n{assetPath}\n\n" +
            "Next steps:\n" +
            "1. Set Icon image (if not set)\n" +
            "2. Add to Player → InventorySystem → All Game Items\n" +
            "3. Create collectable item in scene", 
            "OK");

        // Reset fields for next item
        itemId = "item_" + Random.Range(100, 999);
        itemName = "New Item";
        description = "Item description";
        icon = null;
    }
}

