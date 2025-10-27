using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to setup FloatingTextManager in the scene
/// </summary>
public class SetupFloatingTextManager : EditorWindow
{
    private Canvas canvas;
    private GameObject floatingTextPrefab;

    [MenuItem("Tools/Setup FloatingTextManager")]
    public static void ShowWindow()
    {
        GetWindow<SetupFloatingTextManager>("Setup FloatingTextManager");
    }

    void OnGUI()
    {
        GUILayout.Label("FloatingTextManager 設置工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Find or set Canvas
        canvas = (Canvas)EditorGUILayout.ObjectField("Canvas", canvas, typeof(Canvas), true);
        
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorGUILayout.HelpBox("場景中沒有 Canvas，請先創建一個 Canvas", MessageType.Warning);
                if (GUILayout.Button("自動創建 Canvas"))
                {
                    CreateCanvas();
                }
            }
        }

        // Find or set FloatingText Prefab
        floatingTextPrefab = (GameObject)EditorGUILayout.ObjectField(
            "FloatingText Prefab", 
            floatingTextPrefab, 
            typeof(GameObject), 
            false);

        if (floatingTextPrefab == null)
        {
            // Try to find the prefab
            string[] guids = AssetDatabase.FindAssets("FloatingText t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                floatingTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                EditorGUILayout.HelpBox($"已找到 FloatingText prefab: {path}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("找不到 FloatingText prefab，需要創建", MessageType.Warning);
            }
        }

        EditorGUILayout.Space();

        // Check if FloatingTextManager already exists
        FloatingTextManager existingManager = FindObjectOfType<FloatingTextManager>();
        if (existingManager != null)
        {
            EditorGUILayout.HelpBox("✓ 場景中已經有 FloatingTextManager", MessageType.Info);
            EditorGUILayout.ObjectField("現有的 FloatingTextManager", existingManager, typeof(FloatingTextManager), true);
        }

        EditorGUILayout.Space();

        // Setup button
        GUI.enabled = canvas != null && floatingTextPrefab != null && existingManager == null;

        if (GUILayout.Button("創建 FloatingTextManager", GUILayout.Height(40)))
        {
            CreateFloatingTextManager();
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        // Quick actions
        if (GUILayout.Button("自動尋找 Canvas 和 Prefab"))
        {
            canvas = FindObjectOfType<Canvas>();
            
            string[] guids = AssetDatabase.FindAssets("FloatingText t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                floatingTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }

            EditorUtility.DisplayDialog("完成", "已自動尋找並設置", "OK");
        }
    }

    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvasComponent = canvasObj.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        canvas = canvasComponent;

        EditorUtility.DisplayDialog("成功", "已創建 Canvas", "OK");
    }

    private void CreateFloatingTextManager()
    {
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("錯誤", "請先設置 Canvas", "OK");
            return;
        }

        if (floatingTextPrefab == null)
        {
            EditorUtility.DisplayDialog("錯誤", "請先設置 FloatingText Prefab", "OK");
            return;
        }

        // Check if it already exists
        FloatingTextManager existingManager = canvas.GetComponentInChildren<FloatingTextManager>();
        if (existingManager != null)
        {
            EditorUtility.DisplayDialog("注意", "場景中已經有 FloatingTextManager", "OK");
            Selection.activeObject = existingManager.gameObject;
            return;
        }

        // Create FloatingTextManager GameObject
        GameObject ftmObj = new GameObject("FloatingTextManager");
        ftmObj.transform.SetParent(canvas.transform, false);

        // Add FloatingTextManager component
        FloatingTextManager ftm = ftmObj.AddComponent<FloatingTextManager>();

        // Set the prefab using serialized object
        SerializedObject serializedObject = new SerializedObject(ftm);
        SerializedProperty prefabProperty = serializedObject.FindProperty("floatingTextPrefab");
        prefabProperty.objectReferenceValue = floatingTextPrefab;
        serializedObject.ApplyModifiedProperties();

        Selection.activeObject = ftmObj;

        EditorUtility.DisplayDialog("成功", "已創建 FloatingTextManager！\n\n現在可以使用 BombEndpoint 了", "OK");

        Debug.Log("FloatingTextManager 已創建完成，可以使用 BombEndpoint 了！");
    }
}
