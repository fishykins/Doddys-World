namespace DW.Network {
    public enum UniversalPacketType
    {
        initialize = 0,
        ping,
        pong,
        VehicleDNA,
        RequestVehicle,
        vehicleUpdate,
    }

    //Server -> Client
    public enum ServerPacketType
    {
        initialize = 64,
    }

    // Client -> Server
    public enum ClientPacketType
    {
        initialize = 128,
    }
}
