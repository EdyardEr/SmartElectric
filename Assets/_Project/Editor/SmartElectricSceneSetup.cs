#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using SmartElectric.Adapters;
using SmartElectric.AR;
using SmartElectric.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace SmartElectric.EditorTools
{
    public static class SmartElectricSceneSetup
    {
        const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
        const string ProductScenePath = "Assets/_Project/Scenes/ARPlacement.unity";
        const string MenuSetupOpen = "SmartElectric/Setup Phase1 On Open Scene";
        const string MenuCreateT2 = "SmartElectric/Create ARPlacement Scene (T2)";

        [MenuItem(MenuSetupOpen)]
        public static void SetupOpenScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[SmartElectric] No active scene.");
                return;
            }

            EnsureRuntimeRoot(scene);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[SmartElectric] Phase 1 runtime added to open scene.");
        }

        [MenuItem(MenuCreateT2)]
        public static void CreateProductScene()
        {
            if (!File.Exists(SampleScenePath))
            {
                Debug.LogError($"[SmartElectric] Missing template scene: {SampleScenePath}");
                return;
            }

            var dir = Path.GetDirectoryName(ProductScenePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(ProductScenePath))
            {
                if (!EditorUtility.DisplayDialog(
                        "SmartElectric",
                        $"{ProductScenePath} already exists. Overwrite from SampleScene?",
                        "Overwrite",
                        "Cancel"))
                    return;
            }

            AssetDatabase.CopyAsset(SampleScenePath, ProductScenePath);
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.OpenScene(ProductScenePath, OpenSceneMode.Single);
            DisableTemplateDemo(scene);
            EnsureRuntimeRoot(scene);
            EditorSceneManager.SaveScene(scene);

            SetProductSceneAsBuildIndex0(ProductScenePath);
            Debug.Log($"[SmartElectric] T2 ready: {ProductScenePath} is Build Settings index 0. Build And Run from this scene.");
        }

        static void EnsureRuntimeRoot(Scene scene)
        {
            var root = GameObject.Find("SmartElectric_Runtime");
            if (root == null)
            {
                root = new GameObject("SmartElectric_Runtime");
                Undo.RegisterCreatedObjectUndo(root, "Create SmartElectric_Runtime");
            }

            var session = root.GetComponent<ProjectSession>() ?? Undo.AddComponent<ProjectSession>(root);
            var placer = root.GetComponent<ArDevicePlacer>() ?? Undo.AddComponent<ArDevicePlacer>(root);
            var hud = root.GetComponent<ProjectDebugHud>() ?? Undo.AddComponent<ProjectDebugHud>(root);
            var disabler = root.GetComponent<TemplateUiDisabler>() ?? Undo.AddComponent<TemplateUiDisabler>(root);
            var planeSync = root.GetComponent<PlaneWallSync>() ?? Undo.AddComponent<PlaneWallSync>(root);

            var raycast = Object.FindAnyObjectByType<ARRaycastManager>();
            var anchor = Object.FindAnyObjectByType<ARAnchorManager>();
            var planeManager = Object.FindAnyObjectByType<ARPlaneManager>();
            if (raycast == null)
                Debug.LogWarning("[SmartElectric] ARRaycastManager not found — ensure XR Origin exists in scene.");

            var placerSo = new SerializedObject(placer);
            placerSo.FindProperty("raycastManager").objectReferenceValue = raycast;
            placerSo.FindProperty("anchorManager").objectReferenceValue = anchor;
            placerSo.FindProperty("session").objectReferenceValue = session;
            placerSo.ApplyModifiedPropertiesWithoutUndo();

            var planeSo = new SerializedObject(planeSync);
            planeSo.FindProperty("planeManager").objectReferenceValue = planeManager;
            planeSo.FindProperty("session").objectReferenceValue = session;
            planeSo.ApplyModifiedPropertiesWithoutUndo();

            var hudSo = new SerializedObject(hud);
            hudSo.FindProperty("session").objectReferenceValue = session;
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            if (!disabler.enabled)
                disabler.enabled = true;
        }

        static void DisableTemplateDemo(Scene scene)
        {
            var roots = scene.GetRootGameObjects();
            var toDisable = new HashSet<GameObject>();

            for (var r = 0; r < roots.Length; r++)
                CollectTemplateObjects(roots[r], toDisable);

            foreach (var go in toDisable)
            {
                if (go != null && go.name != "SmartElectric_Runtime")
                    go.SetActive(false);
            }
        }

        static void CollectTemplateObjects(GameObject go, HashSet<GameObject> toDisable)
        {
            if (go == null)
                return;

            var behaviours = go.GetComponents<MonoBehaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                if (b == null)
                    continue;
                var ns = b.GetType().Namespace ?? string.Empty;
                var name = b.GetType().Name;
                if (ns.Contains("XR.Templates.AR") || name.Contains("GoalManager") || name.Contains("ARTemplateMenuManager"))
                {
                    toDisable.Add(go);
                    break;
                }
            }

            if (go.name == "UI" || go.name == "Coaching UI" || go.name == "Object Spawner" || go.name == "Greeting Prompt")
                toDisable.Add(go);

            for (var c = 0; c < go.transform.childCount; c++)
                CollectTemplateObjects(go.transform.GetChild(c).gameObject, toDisable);
        }

        static void SetProductSceneAsBuildIndex0(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(scenePath, true)
            };

            foreach (var existing in EditorBuildSettings.scenes)
            {
                if (existing.path == scenePath)
                    continue;
                scenes.Add(new EditorBuildSettingsScene(existing.path, false));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
#endif
