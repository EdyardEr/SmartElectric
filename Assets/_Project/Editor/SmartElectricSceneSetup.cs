#if UNITY_EDITOR
using SmartElectric.AR;
using SmartElectric.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace SmartElectric.EditorTools
{
    /// <summary>One-click: add Phase 1 session + placer + HUD to the open AR scene.</summary>
    public static class SmartElectricSceneSetup
    {
        const string MenuPath = "SmartElectric/Setup Phase1 On Open Scene";

        [MenuItem(MenuPath)]
        public static void Setup()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[SmartElectric] No active scene.");
                return;
            }

            var root = GameObject.Find("SmartElectric_Runtime");
            if (root == null)
            {
                root = new GameObject("SmartElectric_Runtime");
                Undo.RegisterCreatedObjectUndo(root, "Create SmartElectric_Runtime");
            }

            var session = root.GetComponent<ProjectSession>();
            if (session == null)
                session = Undo.AddComponent<ProjectSession>(root);

            var placer = root.GetComponent<ArDevicePlacer>();
            if (placer == null)
                placer = Undo.AddComponent<ArDevicePlacer>(root);

            var hud = root.GetComponent<ProjectDebugHud>();
            if (hud == null)
                hud = Undo.AddComponent<ProjectDebugHud>(root);

            var raycast = Object.FindAnyObjectByType<ARRaycastManager>();
            if (raycast == null)
                Debug.LogWarning("[SmartElectric] ARRaycastManager not found — add XR Origin / AR components from the template scene.");

            var so = new SerializedObject(placer);
            so.FindProperty("raycastManager").objectReferenceValue = raycast;
            so.FindProperty("session").objectReferenceValue = session;
            so.ApplyModifiedPropertiesWithoutUndo();

            var hudSo = new SerializedObject(hud);
            hudSo.FindProperty("session").objectReferenceValue = session;
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[SmartElectric] Phase 1 components added to SmartElectric_Runtime. Enter Play / Build And Run, tap a plane, use HUD Save/Load.");
        }
    }
}
#endif
