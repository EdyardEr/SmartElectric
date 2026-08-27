using System;

namespace SmartElectric.Domain
{
    [Serializable]
    public struct Vec2Data
    {
        public float x;
        public float y;

        public Vec2Data(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
    public struct Vec3Data
    {
        public float x;
        public float y;
        public float z;

        public Vec3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    /// <summary>Room-space pose. Rotation is quaternion x,y,z,w.</summary>
    [Serializable]
    public struct PoseData
    {
        public Vec3Data position;
        public float qx;
        public float qy;
        public float qz;
        public float qw;

        public static PoseData Identity => new PoseData
        {
            position = new Vec3Data(0f, 0f, 0f),
            qx = 0f,
            qy = 0f,
            qz = 0f,
            qw = 1f
        };
    }
}
