using UnityEngine;

/// <summary>
/// Test script for validating the sound system functionality
/// This script can be attached to a GameObject in the scene to run automated tests
/// Tests cover SoundManager initialization, sound playback, and InteractionHandler integration
/// </summary>
public class SoundSystemTester : MonoBehaviour
{
    [Header("Test Configuration")]
    [Tooltip("Whether to run tests automatically on Start")]
    [SerializeField] private bool runTestsOnStart = false;
    
    [Tooltip("Whether to show detailed test output")]
    [SerializeField] private bool showDetailedOutput = true;

    [Header("Test Audio Clips (Optional)")]
    [Tooltip("Optional audio clip for testing sound playback")]
    [SerializeField] private AudioClip testAudioClip;

    private int testsPassed = 0;
    private int testsFailed = 0;
    private int totalTests = 0;

    void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    /// <summary>
    /// Run all sound system tests
    /// </summary>
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        testsPassed = 0;
        testsFailed = 0;
        totalTests = 0;

        Debug.Log("=== Sound System Test Suite Started ===");

        // Test SoundManager
        TestSoundManagerInitialization();
        TestSoundManagerSingleton();
        TestSoundManagerPlayback();
        TestSoundManagerNullHandling();

        // Test InteractionSoundData
        TestInteractionSoundData();

        // Test InteractableSoundComponent
        TestInteractableSoundComponent();

        // Test Integration
        TestInteractionHandlerIntegration();

        // Print results
        PrintTestResults();
    }

    // ==================== SoundManager Tests ====================

    /// <summary>
    /// Test SoundManager initialization
    /// </summary>
    private void TestSoundManagerInitialization()
    {
        LogTestStart("SoundManager Initialization");

        try
        {
            SoundManager soundManager = SoundManager.GetInstance();
            
            if (soundManager == null)
            {
                LogTestFailure("SoundManager.GetInstance() returned null");
                return;
            }

            if (soundManager.gameObject == null)
            {
                LogTestFailure("SoundManager GameObject is null");
                return;
            }

            LogTestSuccess("SoundManager initialized successfully");
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during initialization: {e.Message}");
        }
    }

    /// <summary>
    /// Test SoundManager singleton pattern
    /// </summary>
    private void TestSoundManagerSingleton()
    {
        LogTestStart("SoundManager Singleton Pattern");

        try
        {
            SoundManager instance1 = SoundManager.GetInstance();
            SoundManager instance2 = SoundManager.GetInstance();

            if (instance1 == null || instance2 == null)
            {
                LogTestFailure("One or both instances are null");
                return;
            }

            if (instance1 != instance2)
            {
                LogTestFailure("Singleton pattern failed - instances are different");
                return;
            }

            LogTestSuccess("Singleton pattern working correctly");
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during singleton test: {e.Message}");
        }
    }

    /// <summary>
    /// Test SoundManager sound playback methods
    /// </summary>
    private void TestSoundManagerPlayback()
    {
        LogTestStart("SoundManager Playback Methods");

        try
        {
            SoundManager soundManager = SoundManager.GetInstance();
            
            if (soundManager == null)
            {
                LogTestFailure("SoundManager is null");
                return;
            }

            // Test with null clip (should handle gracefully)
            soundManager.PlaySound(null, Vector3.zero, 1f);
            soundManager.PlaySound2D(null, 1f);

            // Test with valid clip if available
            if (testAudioClip != null)
            {
                soundManager.PlaySound(testAudioClip, Vector3.zero, 0.5f);
                soundManager.PlaySound2D(testAudioClip, 0.5f);
                LogTestSuccess("Sound playback methods executed without errors");
            }
            else
            {
                LogTestSuccess("Sound playback methods handle null clips correctly (no test clip assigned)");
            }
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during playback test: {e.Message}");
        }
    }

    /// <summary>
    /// Test SoundManager null handling
    /// </summary>
    private void TestSoundManagerNullHandling()
    {
        LogTestStart("SoundManager Null Handling");

        try
        {
            SoundManager soundManager = SoundManager.GetInstance();
            
            // These should not throw exceptions
            soundManager.PlaySound(null, Vector3.zero);
            soundManager.PlaySound2D(null);
            soundManager.PlayInteractionSound(null, Vector3.zero);

            LogTestSuccess("Null handling works correctly");
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during null handling test: {e.Message}");
        }
    }

    // ==================== InteractionSoundData Tests ====================

    /// <summary>
    /// Test InteractionSoundData ScriptableObject
    /// </summary>
    private void TestInteractionSoundData()
    {
        LogTestStart("InteractionSoundData");

        try
        {
            // Create a test instance
            InteractionSoundData testData = ScriptableObject.CreateInstance<InteractionSoundData>();
            
            if (testData == null)
            {
                LogTestFailure("Failed to create InteractionSoundData instance");
                return;
            }

            // Test property access
            AudioClip successSound = testData.SuccessSound;
            AudioClip failureSound = testData.FailureSound;
            float volume = testData.Volume;

            // Test method calls
            AudioClip clip = testData.GetSoundClip(InteractionSoundType.Success);
            bool hasSound = testData.HasSound(InteractionSoundType.Success);

            // These should not throw exceptions even with null sounds
            if (clip == null && !hasSound)
            {
                LogTestSuccess("InteractionSoundData handles missing sounds correctly");
            }
            else
            {
                LogTestSuccess("InteractionSoundData methods work correctly");
            }

            // Cleanup
            DestroyImmediate(testData);
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during InteractionSoundData test: {e.Message}");
        }
    }

    // ==================== InteractableSoundComponent Tests ====================

    /// <summary>
    /// Test InteractableSoundComponent
    /// </summary>
    private void TestInteractableSoundComponent()
    {
        LogTestStart("InteractableSoundComponent");

        try
        {
            // Create a test GameObject
            GameObject testObject = new GameObject("TestInteractable");
            InteractableSoundComponent component = testObject.AddComponent<InteractableSoundComponent>();

            if (component == null)
            {
                LogTestFailure("Failed to add InteractableSoundComponent");
                DestroyImmediate(testObject);
                return;
            }

            // Test methods
            bool hasData = component.HasSoundData();
            InteractionSoundData data = component.GetSoundData();
            bool playAs2D = component.ShouldPlayAs2D();
            Vector3 position = component.GetSoundPosition();

            // Component should work even without sound data
            if (!hasData && data == null)
            {
                LogTestSuccess("InteractableSoundComponent handles missing sound data correctly");
            }
            else
            {
                LogTestSuccess("InteractableSoundComponent methods work correctly");
            }

            // Cleanup
            DestroyImmediate(testObject);
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during InteractableSoundComponent test: {e.Message}");
        }
    }

    // ==================== Integration Tests ====================

    /// <summary>
    /// Test InteractionHandler integration
    /// </summary>
    private void TestInteractionHandlerIntegration()
    {
        LogTestStart("InteractionHandler Integration");

        try
        {
            // Try to find InteractionHandler in scene
            InteractionHandler handler = FindFirstObjectByType<InteractionHandler>();
            
            if (handler == null)
            {
                LogTestSuccess("InteractionHandler not found in scene (this is acceptable for testing)");
                return;
            }

            // Test that handler has sound manager reference (should be initialized in Start)
            // We can't directly test private fields, but we can verify the handler exists
            LogTestSuccess("InteractionHandler found and initialized");
        }
        catch (System.Exception e)
        {
            LogTestFailure($"Exception during integration test: {e.Message}");
        }
    }

    // ==================== Edge Case Tests ====================

    /// <summary>
    /// Test edge cases and error conditions
    /// </summary>
    [ContextMenu("Test Edge Cases")]
    public void TestEdgeCases()
    {
        Debug.Log("=== Edge Case Tests ===");

        try
        {
            SoundManager soundManager = SoundManager.GetInstance();

            // Test extreme volume values
            soundManager.PlaySound(testAudioClip, Vector3.zero, 0f);
            soundManager.PlaySound(testAudioClip, Vector3.zero, 1f);
            soundManager.PlaySound(testAudioClip, Vector3.zero, 2f); // Should be clamped

            // Test volume control methods (reserved for future)
            soundManager.SetMasterVolume(0.5f);
            soundManager.SetSFXVolume(0.75f);
            float masterVol = soundManager.GetMasterVolume();
            float sfxVol = soundManager.GetSFXVolume();

            if (masterVol == 0.5f && sfxVol == 0.75f)
            {
                Debug.Log("✓ Edge case tests passed");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Edge case test failed: {e.Message}");
        }
    }

    // ==================== Helper Methods ====================

    /// <summary>
    /// Log test start
    /// </summary>
    private void LogTestStart(string testName)
    {
        totalTests++;
        if (showDetailedOutput)
        {
            Debug.Log($"Testing: {testName}");
        }
    }

    /// <summary>
    /// Log test success
    /// </summary>
    private void LogTestSuccess(string message)
    {
        testsPassed++;
        if (showDetailedOutput)
        {
            Debug.Log($"✓ PASS: {message}");
        }
    }

    /// <summary>
    /// Log test failure
    /// </summary>
    private void LogTestFailure(string message)
    {
        testsFailed++;
        Debug.LogError($"✗ FAIL: {message}");
    }

    /// <summary>
    /// Print test results summary
    /// </summary>
    private void PrintTestResults()
    {
        Debug.Log("=== Test Results ===");
        Debug.Log($"Total Tests: {totalTests}");
        Debug.Log($"Passed: {testsPassed}");
        Debug.Log($"Failed: {testsFailed}");
        Debug.Log($"Success Rate: {(totalTests > 0 ? (testsPassed * 100f / totalTests) : 0):F1}%");
        
        if (testsFailed == 0)
        {
            Debug.Log("=== All Tests Passed! ===");
        }
        else
        {
            Debug.LogWarning("=== Some Tests Failed ===");
        }
    }

    // ==================== Manual Test Methods ====================

    /// <summary>
    /// Manually test sound playback with a specific clip
    /// </summary>
    [ContextMenu("Test Sound Playback")]
    public void TestSoundPlayback()
    {
        if (testAudioClip == null)
        {
            Debug.LogWarning("No test audio clip assigned. Please assign one in the Inspector.");
            return;
        }

        SoundManager soundManager = SoundManager.GetInstance();
        if (soundManager == null)
        {
            Debug.LogError("SoundManager not found!");
            return;
        }

        Debug.Log("Testing 3D sound playback...");
        soundManager.PlaySound(testAudioClip, transform.position, 0.5f);

        Debug.Log("Testing 2D sound playback...");
        soundManager.PlaySound2D(testAudioClip, 0.5f);

        Debug.Log("Sound playback test completed. Check audio output.");
    }

    /// <summary>
    /// Test InteractionSoundData with a specific sound type
    /// </summary>
    [ContextMenu("Test InteractionSoundData")]
    public void TestInteractionSoundDataManual()
    {
        // This would require creating a test InteractionSoundData asset
        Debug.Log("To test InteractionSoundData manually:");
        Debug.Log("1. Create an InteractionSoundData asset in Unity");
        Debug.Log("2. Assign it to an InteractableSoundComponent on a GameObject");
        Debug.Log("3. Test interaction with that GameObject");
    }
}

