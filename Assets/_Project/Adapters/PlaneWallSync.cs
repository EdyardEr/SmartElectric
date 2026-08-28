using SmartElectric.Domain;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SmartElectric.Adapters
{
    /// <summary>Maps detected vertical AR planes into RoomModel walls (Phase 1 planes scan).</summary>
    public sealed class PlaneWallSync : MonoBehaviour
    {
        [SerializeField] ARPlaneManager planeManager;
        [SerializeField] SmartElectric.AR.ProjectSession session;
        [SerializeField] bool syncVerticalOnly = true;

        void Awake()
        {
            if (planeManager == null)
                planeManager = FindAnyObjectByType<ARPlaneManager>();
            if (session == null)
                session = FindAnyObjectByType<SmartElectric.AR.ProjectSession>();
        }

        void OnEnable()
        {
            if (planeManager != null)
                planeManager.trackablesChanged.AddListener(OnPlanesChanged);
        }

        void OnDisable()
        {
            if (planeManager != null)
                planeManager.trackablesChanged.RemoveListener(OnPlanesChanged);
        }

        void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
        {
            if (session?.Room == null)
                return;

            foreach (var plane in changes.added)
                SyncPlane(plane);
            foreach (var plane in changes.updated)
                SyncPlane(plane);
        }

        void SyncPlane(ARPlane plane)
        {
            if (plane == null || !plane.gameObject.activeInHierarchy)
                return;
            if (syncVerticalOnly && plane.alignment != PlaneAlignment.Vertical)
                return;

            var wall = new WallData
            {
                id = WallIdFromPlane(plane),
                width = plane.size.x,
                height = plane.size.y,
                transform = PoseMapping.FromTransform(plane.transform)
            };
            session.Room.UpsertWall(wall);
            session.NotifyChanged();
        }

        public static string WallIdFromPlane(ARPlane plane)
        {
            return "plane_" + plane.trackableId.subId1 + "_" + plane.trackableId.subId2;
        }
    }
}
