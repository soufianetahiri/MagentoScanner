namespace MagentoScanner.Helpers
{
    public enum Importance
    {
        Info,
        Warning,
        Critical,
        Log
    }
    public enum Criticity
    {
        Unknown,
        Low,
        High
    }
    public enum PatchStats
    {
        Unknown,
        NotPatched,
        Patched,
        Installed,
        BackendUrlRequired
    }
}
