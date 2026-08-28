using SmartElectric.Domain;
using UnityEngine;

namespace SmartElectric.UI
{
    /// <summary>Temporary Phase 1 debug HUD (OnGUI). Replace with product UI later.</summary>
    public sealed class ProjectDebugHud : MonoBehaviour
    {
        static readonly Rect PanelRect = new Rect(8f, 8f, 168f, 296f);

        [SerializeField] SmartElectric.AR.ProjectSession session;

        void Awake()
        {
            if (session == null)
                session = FindAnyObjectByType<SmartElectric.AR.ProjectSession>();
        }

        public static bool BlocksPlacement(Vector2 screenPositionBottomLeft)
        {
            var guiPoint = new Vector2(screenPositionBottomLeft.x, Screen.height - screenPositionBottomLeft.y);
            return PanelRect.Contains(guiPoint);
        }

        void OnGUI()
        {
            if (session == null)
                return;

            const float w = 160f;
            const float h = 40f;
            float x = 12f;
            float y = 12f;

            GUI.Box(new Rect(x - 4f, y - 4f, w + 8f, h * 6f + 48f), "SmartElectric");

            if (GUI.Button(new Rect(x, y, w, h), "Outlet"))
                session.SetDeviceType(ElectricalDeviceType.Outlet);
            y += h + 4f;

            if (GUI.Button(new Rect(x, y, w, h), "Panel"))
                session.SetDeviceType(ElectricalDeviceType.Panel);
            y += h + 4f;

            if (GUI.Button(new Rect(x, y, w, h), "Save"))
                session.Save();
            y += h + 4f;

            if (GUI.Button(new Rect(x, y, w, h), "Load"))
                session.Load();
            y += h + 4f;

            if (GUI.Button(new Rect(x, y, w, h), "New room"))
                session.NewRoom();
            y += h + 8f;

            GUI.Label(new Rect(x, y, 400f, 60f), session.LastStatus);
        }
    }
}
