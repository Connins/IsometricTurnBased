public static class GlobalParameters 
{
    private static bool isLocalPlay = false;
    static bool isHost = true;
    private static bool matchHappening = false;
    private static bool youAreGoodGuys = true;
    public static bool IsLocalPlay
    {
        get { return isLocalPlay; }
        set { isLocalPlay = value; }
    }

    public static bool MatchHappening
    {
        get { return matchHappening; }
        set { matchHappening = value; }
    }

    public static bool YouAreGoodGuys
    {
        get { return youAreGoodGuys; }
        set { youAreGoodGuys = value; }
    }

    public static bool IsHost
    {
        get { return isHost; }
        set { isHost = value; }
    }
}
