namespace SmartElectric.Domain
{
    public enum ScanMode
    {
        Lidar = 0,
        Planes = 1,
        Manual = 2
    }

    public enum Confidence
    {
        High = 0,
        Medium = 1,
        Low = 2
    }

    public enum LengthUnits
    {
        Meters = 0
    }

    public enum DeviceType
    {
        Outlet = 0,
        Panel = 1,
        Switch = 2,
        Other = 3
    }

    public enum OpeningType
    {
        Door = 0,
        Window = 1
    }

    public enum RouteChannel
    {
        Wall = 0,
        Ceiling = 1,
        Floor = 2,
        Conduit = 3
    }
}
