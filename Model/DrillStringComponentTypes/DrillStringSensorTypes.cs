namespace Model.DrillStringSensorTypes
{
    /// <summary>
    /// an enum to describe the types of sensors in the drill string
    /// </summary>
    public enum DrillStringSensorTypes
    {
        Acoustic = 1,        
        AccelerometerX = 2,
        AccelerometerY = 4,
        AccelerometerZ = 8,
        ResistivitySource = 16,
        ResistivityReceiver = 32,
        NeutronDensity = 64,
        NeutronPorosity = 128,
        MagnetometerX = 256,
        MagnetometerY = 512,
        RotationalPitch = 1024,
        RotationalRoll = 2048,
        RotationalYaw = 4096,
        BendingMomentX = 8192,
        BendingMomentY = 16384,
        Pressure = 32768,
        Temperature = 65536,
        GammaRay = 131072,
        Other = 262144,
        Tension = 524288,
        Torque = 1048576        
    }
}
