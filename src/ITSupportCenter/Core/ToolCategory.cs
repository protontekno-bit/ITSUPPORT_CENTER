namespace ITSupportCenter.Core
{
    public static class ToolCategory
    {
        public const string All = "⭐ Semua Kategori";
        public const string Printer = "🖨️ Printer Sharing";
        public const string Network = "🌐 Jaringan & File Sharing";
        public const string System = "🛠️ Perbaikan Sistem";
        public const string Remote = "🖥️ Remote Desktop";
        public const string Security = "🛡️ Keamanan & Firewall";
        public const string Scanner = "📡 Audit & Port Scanner";
        public const string License = "📜 Lisensi Windows";

        public static string[] GetAllCategories() => new[]
        {
            All,
            Printer,
            Network,
            System,
            Remote,
            Security,
            Scanner,
            License
        };
    }
}
