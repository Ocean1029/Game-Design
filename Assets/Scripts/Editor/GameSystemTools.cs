using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 統一的遊戲系統工具選單
/// 所有功能都集中在 Tools > Game System 下
/// </summary>
public class GameSystemTools
{
    // ==================== 主要設置功能 ====================

    /// <summary>
    /// 完整系統設置 - 創建所有必要的資料庫和 GameManager
    /// </summary>
    [MenuItem("Tools/Game System/🚀 Complete Setup", false, 0)]
    public static void CompleteSystemSetup()
    {
        if (EditorUtility.DisplayDialog(
            "Complete System Setup",
            "這將創建完整的遊戲系統：\n\n" +
            "• SpawnPointDatabase\n" +
            "• TransitionPointDatabase\n" +
            "• GameManager 在場景中\n" +
            "• GameManager Prefab\n\n" +
            "繼續嗎？",
            "開始設置",
            "取消"))
        {
            PerformCompleteSetup();
        }
    }

    /// <summary>
    /// 快速設置 - 僅創建必要的組件
    /// </summary>
    [MenuItem("Tools/Game System/⚡ Quick Setup", false, 1)]
    public static void QuickSystemSetup()
    {
        if (EditorUtility.DisplayDialog(
            "Quick System Setup",
            "這將創建基本的系統組件：\n\n" +
            "• 必要的資料庫\n" +
            "• GameManager 在場景中\n\n" +
            "繼續嗎？",
            "開始設置",
            "取消"))
        {
            PerformQuickSetup();
        }
    }

    // ==================== 資料庫管理 ====================

    /// <summary>
    /// 創建 Spawn Point Database
    /// </summary>
    [MenuItem("Tools/Game System/📊 Create Spawn Point Database", false, 10)]
    public static void CreateSpawnPointDatabase()
    {
        string path = GetSelectedPathOrFallback();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/SpawnPointDatabase.asset");
        
        SpawnPointDatabase asset = ScriptableObject.CreateInstance<SpawnPointDatabase>();
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log($"✓ Created Spawn Point Database: {assetPath}");
        EditorUtility.DisplayDialog("Success", $"Spawn Point Database created:\n{assetPath}", "OK");
    }

    /// <summary>
    /// 創建 Transition Point Database
    /// </summary>
    [MenuItem("Tools/Game System/📊 Create Transition Point Database", false, 11)]
    public static void CreateTransitionPointDatabase()
    {
        string path = GetSelectedPathOrFallback();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/TransitionPointDatabase.asset");
        
        TransitionPointDatabase asset = ScriptableObject.CreateInstance<TransitionPointDatabase>();
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        
        Debug.Log($"✓ Created Transition Point Database: {assetPath}");
        EditorUtility.DisplayDialog("Success", $"Transition Point Database created:\n{assetPath}", "OK");
    }

    // ==================== 診斷和驗證 ====================

    /// <summary>
    /// 驗證所有資料庫
    /// </summary>
    [MenuItem("Tools/Game System/🔍 Validate All Databases", false, 20)]
    public static void ValidateAllDatabases()
    {
        Debug.Log("==================================================");
        Debug.Log("🔍 VALIDATING ALL DATABASES");
        Debug.Log("==================================================");
        
        bool hasErrors = false;
        
        // 檢查 Spawn Point Databases
        string[] spawnDbGuids = AssetDatabase.FindAssets("t:SpawnPointDatabase");
        Debug.Log($"Found {spawnDbGuids.Length} Spawn Point Database(s):");
        
        foreach (string guid in spawnDbGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            SpawnPointDatabase db = AssetDatabase.LoadAssetAtPath<SpawnPointDatabase>(assetPath);
            if (db != null)
            {
                Debug.Log($"  ✓ {assetPath}");
                db.ValidateDatabase(); // 觸發驗證
            }
            else
            {
                Debug.LogError($"  ✗ {assetPath} - Failed to load");
                hasErrors = true;
            }
        }
        
        // 檢查 Transition Point Databases
        string[] transitionDbGuids = AssetDatabase.FindAssets("t:TransitionPointDatabase");
        Debug.Log($"Found {transitionDbGuids.Length} Transition Point Database(s):");
        
        foreach (string guid in transitionDbGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TransitionPointDatabase db = AssetDatabase.LoadAssetAtPath<TransitionPointDatabase>(assetPath);
            if (db != null)
            {
                Debug.Log($"  ✓ {assetPath}");
                db.ValidateDatabase(); // 觸發驗證
            }
            else
            {
                Debug.LogError($"  ✗ {assetPath} - Failed to load");
                hasErrors = true;
            }
        }
        
        Debug.Log("==================================================");
        
        if (hasErrors)
        {
            Debug.LogError("❌ Validation completed with errors!");
            EditorUtility.DisplayDialog("Validation Results", "Validation completed with errors!\n\nCheck the Console for details.", "OK");
        }
        else
        {
            Debug.Log("✅ All databases validated successfully!");
            EditorUtility.DisplayDialog("Validation Results", "All databases validated successfully!\n\nCheck the Console for details.", "OK");
        }
    }

    /// <summary>
    /// 檢查 GameManager 狀態
    /// </summary>
    [MenuItem("Tools/Game System/🎮 Check GameManager Status", false, 21)]
    public static void CheckGameManagerStatus()
    {
        Debug.Log("==================================================");
        Debug.Log("🎮 CHECKING GAMEMANAGER STATUS");
        Debug.Log("==================================================");
        
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        
        if (gameManager != null)
        {
            Debug.Log($"✓ Found GameManager: {gameManager.name}");
            Debug.Log($"  Scene: {gameManager.gameObject.scene.name}");
            
            // 檢查資料庫引用
            SerializedObject serializedManager = new SerializedObject(gameManager);
            var spawnDb = serializedManager.FindProperty("spawnPointDatabase").objectReferenceValue;
            var transitionDb = serializedManager.FindProperty("transitionPointDatabase").objectReferenceValue;
            
            Debug.Log($"  Spawn Point Database: {(spawnDb != null ? $"✓ {spawnDb.name}" : "✗ Not assigned")}");
            Debug.Log($"  Transition Point Database: {(transitionDb != null ? $"✓ {transitionDb.name}" : "✗ Not assigned")}");
            
            // 顯示結果對話框
            string message = $"GameManager Status:\n\n" +
                           $"✓ Found in scene: {gameManager.gameObject.scene.name}\n" +
                           $"Spawn Database: {(spawnDb != null ? "✓ Assigned" : "✗ Not assigned")}\n" +
                           $"Transition Database: {(transitionDb != null ? "✓ Assigned" : "✗ Not assigned")}\n\n";
            
            if (spawnDb != null && transitionDb != null)
            {
                message += "✅ GameManager is properly configured!";
                Debug.Log("✅ GameManager is properly configured!");
            }
            else
            {
                message += "⚠️ GameManager needs database assignment.\nUse 'Complete Setup' to fix.";
                Debug.LogWarning("⚠️ GameManager needs database assignment.");
            }
            
            EditorUtility.DisplayDialog("GameManager Status", message, "OK");
        }
        else
        {
            Debug.LogWarning("✗ No GameManager found in current scene");
            EditorUtility.DisplayDialog("GameManager Status", 
                "No GameManager found in current scene.\n\n" +
                "Use 'Complete Setup' to create one.", "OK");
        }
        
        Debug.Log("==================================================");
    }

    /// <summary>
    /// 完整系統診斷
    /// </summary>
    [MenuItem("Tools/Game System/🩺 Full System Diagnostic", false, 22)]
    public static void FullSystemDiagnostic()
    {
        Debug.Log("==================================================");
        Debug.Log("🩺 FULL SYSTEM DIAGNOSTIC");
        Debug.Log("==================================================");
        
        // 檢查資料庫
        string[] spawnDbs = AssetDatabase.FindAssets("t:SpawnPointDatabase");
        string[] transitionDbs = AssetDatabase.FindAssets("t:TransitionPointDatabase");
        
        Debug.Log($"📊 Database Status:");
        Debug.Log($"  Spawn Point Databases: {spawnDbs.Length}");
        Debug.Log($"  Transition Point Databases: {transitionDbs.Length}");
        
        // 檢查 GameManager
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        Debug.Log($"🎮 GameManager Status: {(gameManager != null ? "✓ Found" : "✗ Not found")}");
        
        // 檢查 SavePoint 組件
        SavePoint[] savePoints = Object.FindObjectsByType<SavePoint>(FindObjectsSortMode.None);
        Debug.Log($"💾 SavePoints in scene: {savePoints.Length}");
        
        // 檢查 TransitionPoint 組件
        TransitionPoint[] transitionPoints = Object.FindObjectsByType<TransitionPoint>(FindObjectsSortMode.None);
        Debug.Log($"🚪 TransitionPoints in scene: {transitionPoints.Length}");
        
        Debug.Log("==================================================");
        
        // 生成診斷報告
        string report = "System Diagnostic Report:\n\n";
        report += $"Spawn Point Databases: {spawnDbs.Length}\n";
        report += $"Transition Point Databases: {transitionDbs.Length}\n";
        report += $"GameManager: {(gameManager != null ? "Found" : "Not found")}\n";
        report += $"SavePoints: {savePoints.Length}\n";
        report += $"TransitionPoints: {transitionPoints.Length}\n\n";
        
        if (spawnDbs.Length > 0 && transitionDbs.Length > 0 && gameManager != null)
        {
            report += "✅ System appears to be properly set up!";
            Debug.Log("✅ System appears to be properly set up!");
        }
        else
        {
            report += "⚠️ System needs setup or has missing components.";
            Debug.LogWarning("⚠️ System needs setup or has missing components.");
        }
        
        EditorUtility.DisplayDialog("System Diagnostic", report, "OK");
        Debug.Log("==================================================");
    }

    // ==================== 實作方法 ====================

    /// <summary>
    /// 執行完整設置
    /// </summary>
    private static void PerformCompleteSetup()
    {
        try
        {
            Debug.Log("==================================================");
            Debug.Log("🚀 STARTING COMPLETE SYSTEM SETUP");
            Debug.Log("==================================================");

            // 創建資料夾
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder("Assets/Prefabs");

            // 創建資料庫
            string spawnDbPath = "Assets/ScriptableObjects/SpawnPointDatabase.asset";
            string transitionDbPath = "Assets/ScriptableObjects/TransitionPointDatabase.asset";

            SpawnPointDatabase spawnDb = CreateOrLoadDatabase<SpawnPointDatabase>(spawnDbPath);
            TransitionPointDatabase transitionDb = CreateOrLoadDatabase<TransitionPointDatabase>(transitionDbPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 設置 GameManager
            SetupGameManager(spawnDb, transitionDb);

            // 創建 Prefab
            CreateGameManagerPrefab();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 標記場景為已修改
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log("==================================================");
            Debug.Log("✅ COMPLETE SETUP FINISHED!");
            Debug.Log("==================================================");
            Debug.Log("Created:");
            Debug.Log($"  ✓ {spawnDbPath}");
            Debug.Log($"  ✓ {transitionDbPath}");
            Debug.Log($"  ✓ GameManager in scene");
            Debug.Log($"  ✓ GameManager.prefab");
            Debug.Log("==================================================");

            EditorUtility.DisplayDialog(
                "Setup Complete!",
                "Complete system setup successful!\n\n" +
                "✓ SpawnPointDatabase created\n" +
                "✓ TransitionPointDatabase created\n" +
                "✓ GameManager configured\n" +
                "✓ GameManager prefab created\n\n" +
                "Don't forget to save your scene!",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Setup failed: {e.Message}");
            EditorUtility.DisplayDialog("Setup Failed", $"Error: {e.Message}\n\nCheck the Console for details.", "OK");
        }
    }

    /// <summary>
    /// 執行快速設置
    /// </summary>
    private static void PerformQuickSetup()
    {
        try
        {
            Debug.Log("==================================================");
            Debug.Log("⚡ STARTING QUICK SYSTEM SETUP");
            Debug.Log("==================================================");

            // 創建資料夾
            EnsureFolder("Assets/ScriptableObjects");

            // 創建資料庫
            string spawnDbPath = "Assets/ScriptableObjects/SpawnPointDatabase.asset";
            string transitionDbPath = "Assets/ScriptableObjects/TransitionPointDatabase.asset";

            SpawnPointDatabase spawnDb = CreateOrLoadDatabase<SpawnPointDatabase>(spawnDbPath);
            TransitionPointDatabase transitionDb = CreateOrLoadDatabase<TransitionPointDatabase>(transitionDbPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 設置 GameManager
            SetupGameManager(spawnDb, transitionDb);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 標記場景為已修改
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log("==================================================");
            Debug.Log("✅ QUICK SETUP FINISHED!");
            Debug.Log("==================================================");

            EditorUtility.DisplayDialog(
                "Quick Setup Complete!",
                "Quick system setup successful!\n\n" +
                "✓ Databases created\n" +
                "✓ GameManager configured\n\n" +
                "Don't forget to save your scene!",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Quick setup failed: {e.Message}");
            EditorUtility.DisplayDialog("Setup Failed", $"Error: {e.Message}\n\nCheck the Console for details.", "OK");
        }
    }

    /// <summary>
    /// 創建或載入資料庫
    /// </summary>
    private static T CreateOrLoadDatabase<T>(string path) where T : ScriptableObject
    {
        if (!File.Exists(path))
        {
            T database = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(database, path);
            Debug.Log($"✓ Created: {path}");
            return database;
        }
        else
        {
            T database = AssetDatabase.LoadAssetAtPath<T>(path);
            Debug.Log($"✓ Using existing: {path}");
            return database;
        }
    }

    /// <summary>
    /// 設置 GameManager
    /// </summary>
    private static void SetupGameManager(SpawnPointDatabase spawnDb, TransitionPointDatabase transitionDb)
    {
        GameManager existingManager = Object.FindFirstObjectByType<GameManager>();

        if (existingManager != null)
        {
            Debug.Log("✓ GameManager already exists. Updating references...");
            
            SerializedObject serializedManager = new SerializedObject(existingManager);
            serializedManager.FindProperty("spawnPointDatabase").objectReferenceValue = spawnDb;
            serializedManager.FindProperty("transitionPointDatabase").objectReferenceValue = transitionDb;
            serializedManager.FindProperty("showDebugInfo").boolValue = true;
            serializedManager.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(existingManager);
            
            Debug.Log("✓ Updated existing GameManager with database references.");
        }
        else
        {
            Debug.Log("✓ Creating GameManager in scene...");
            GameObject gameManagerObj = new GameObject("GameManager");
            GameManager gameManager = gameManagerObj.AddComponent<GameManager>();

            SerializedObject serializedManager = new SerializedObject(gameManager);
            serializedManager.FindProperty("spawnPointDatabase").objectReferenceValue = spawnDb;
            serializedManager.FindProperty("transitionPointDatabase").objectReferenceValue = transitionDb;
            serializedManager.FindProperty("showDebugInfo").boolValue = true;
            serializedManager.ApplyModifiedProperties();

            Selection.activeGameObject = gameManagerObj;
            
            Debug.Log("✓ Created GameManager in scene.");
        }
    }

    /// <summary>
    /// 創建 GameManager Prefab
    /// </summary>
    private static void CreateGameManagerPrefab()
    {
        string prefabPath = "Assets/Prefabs/GameManager.prefab";
        if (!File.Exists(prefabPath))
        {
            GameObject gameManagerObj = GameObject.Find("GameManager");
            if (gameManagerObj != null)
            {
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameManagerObj, prefabPath);
                Debug.Log($"✓ Created prefab: {prefabPath}");
            }
        }
    }

    /// <summary>
    /// 確保資料夾存在
    /// </summary>
    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = Path.GetDirectoryName(path).Replace("\\", "/");
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, folderName);
            Debug.Log($"✓ Created folder: {path}");
        }
    }

    /// <summary>
    /// 取得選中的路徑或預設路徑
    /// </summary>
    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        
        return path;
    }
}
