namespace DW {
	public enum ApplicationRole {
        host,
        client,
        local,
	}

    public enum SceneStatus
    {
        initializing,
        postInit,
        ready,
        error,
    }

    public enum ManagerStatus
    {
        waiting,
        initializing,
        ready,
        error,
    }
}
