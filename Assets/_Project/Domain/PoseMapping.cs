using UnityEngine;

namespace SmartElectric.Domain
{
    public static class PoseMapping
    {
        public static PoseData FromTransform(Transform transform)
        {
            if (transform == null)
                return PoseData.Identity;

            var p = transform.position;
            var q = transform.rotation;
            return new PoseData
            {
                position = new Vec3Data(p.x, p.y, p.z),
                qx = q.x,
                qy = q.y,
                qz = q.z,
                qw = q.w
            };
        }

        public static Pose ToUnityPose(PoseData data)
        {
            return new Pose(
                new Vector3(data.position.x, data.position.y, data.position.z),
                new Quaternion(data.qx, data.qy, data.qz, data.qw));
        }
    }
}
