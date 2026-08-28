using UnityEngine;

namespace SmartElectric.AR
{
    /// <summary>Disables AR Mobile template onboarding / object menu when product runtime is active.</summary>
    public sealed class TemplateUiDisabler : MonoBehaviour
    {
        [SerializeField] bool disableUiRoot = true;
        [SerializeField] bool disableObjectSpawner = true;

        void Awake()
        {
            DisableByTypeName("GoalManager");
            DisableByTypeName("ARTemplateMenuManager");
            DisableByTypeName("ARPlaneMeshVisualizerFader");

            if (disableObjectSpawner)
            {
                var spawner = GameObject.Find("Object Spawner");
                if (spawner != null)
                    spawner.SetActive(false);
            }

            if (disableUiRoot)
            {
                var ui = GameObject.Find("UI");
                if (ui != null)
                    ui.SetActive(false);

                var coaching = GameObject.Find("Coaching UI");
                if (coaching != null)
                    coaching.SetActive(false);
            }
        }

        static void DisableByTypeName(string typeNameFragment)
        {
            var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                if (b == null)
                    continue;
                var typeName = b.GetType().Name;
                if (typeName.Contains(typeNameFragment))
                    b.gameObject.SetActive(false);
            }
        }
    }
}
