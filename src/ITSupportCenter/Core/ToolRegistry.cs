using System;
using System.Collections.Generic;
using System.Linq;
using ITSupportCenter.Tools.Network;
using ITSupportCenter.Tools.Printer;
using ITSupportCenter.Tools.RemoteAndLicense;
using ITSupportCenter.Tools.Security;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Core
{
    public static class ToolRegistry
    {
        private static readonly List<IToolCommand> _tools = new();

        static ToolRegistry()
        {
            RegisterDefaults();
        }

        public static void Register(IToolCommand tool)
        {
            if (!_tools.Any(t => t.Id.Equals(tool.Id, StringComparison.OrdinalIgnoreCase)))
            {
                _tools.Add(tool);
            }
        }

        public static IReadOnlyList<IToolCommand> GetAllTools() => _tools.AsReadOnly();

        public static IEnumerable<IToolCommand> Filter(string? category, string? searchQuery)
        {
            var query = _tools.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) && !category.Equals(ToolCategory.All, StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string s = searchQuery.Trim().ToLowerInvariant();
                query = query.Where(t =>
                    t.Title.ToLowerInvariant().Contains(s) ||
                    t.Description.ToLowerInvariant().Contains(s) ||
                    t.Keywords.ToLowerInvariant().Contains(s) ||
                    t.Category.ToLowerInvariant().Contains(s)
                );
            }

            return query;
        }

        private static void RegisterDefaults()
        {
            // --- DIAGNOSIS & HEALTH CHECK ---
            Register(new AutoHealthCheckTool());
            Register(new PcAssetInfoTool());

            // --- PRINTER & SPOOLER ---
            Register(new FixPrinterSharingTool());
            Register(new ClearPrintQueueTool());
            Register(new AuditPrintersAndTestPrintTool());
            Register(new SpoolerFactoryResetTool());
            Register(new OpenDevicesAndPrintersClassicTool());

            // --- NETWORK & SMB FILE SHARING ---
            Register(new FullNetworkRepairTool());
            Register(new FixSmbGuestAuthTool());
            Register(new DiagnoseSmbTargetTool());
            Register(new NetworkQualityTesterTool());
            Register(new AuditIpAndNetstatTool());
            Register(new OpenNetworkConnectionsTool());
            Register(new ResetNetworkCacheTool());
            Register(new ResetNetworkStackTool());
            Register(new ResetProxyAndHostsTool());
            Register(new DnsQuickSwitchTool());
            Register(new WifiSignalAuditTool());
            Register(new SubnetIpScannerTool());
            Register(new DisconnectSharesTool());
            Register(new SyncTimeNtpTool());
            Register(new CredentialManagerTool());
            Register(new RestoreAllSharingTool());
            Register(new ActiveDirectoryDomainAssistantTool());

            // --- SYSTEM MAINTENANCE & REPAIR ---
            Register(new CleanSystemTempTool());
            Register(new RamOptimizationTool());
            Register(new ResetWindowsUpdateTool());
            Register(new SystemFileRepairTool());
            Register(new DiskSmartHealthTool());
            Register(new SafeModeAndBcdRepairTool());
            Register(new WindowsAdminToolsHubTool());
            Register(new BatteryHealthReportTool());
            Register(new StartupManagerTool());
            Register(new RestartExplorerShellTool());
            Register(new ScanMissingDriversTool());
            Register(new BackupDriversTool());
            Register(new RestoreDriversTool());
            Register(new WingetSoftwareInstallerTool());
            Register(new WindowsOfficeDebloaterTool());
            Register(new DisableHibernationStorageTool());
            Register(new RestoreClassicContextMenuTool());
            Register(new EnableDotNet35DismTool());
            Register(new CompactOsCompressionTool());
            Register(new RemoveUwpBloatwareTool());
            Register(new DisableEdgeBackgroundBoostTool());
            Register(new OptimizeVisualEffectsTool());
            Register(new ToggleWindowsUpdateLockTool());
            Register(new CreateSystemRestorePointTool());
            Register(new DismWimSystemBackupTool());
            Register(new LiveRescueToolkitHubTool());
            Register(new DiskCloningDeploymentHubTool());
            Register(new VentoyMultiBootGuideTool());
            Register(new DeepHardwareHealthInspectorTool());

            // --- REMOTE DESKTOP & SUPPORT ---
            Register(new WindowsRdpManagerHubTool());
            Register(new ResetAnyDeskIdTool());
            Register(new ChangeTeamViewerPasswordTool());
            Register(new RustDeskManagerHubTool());

            // --- SECURITY & FIREWALL ---
            Register(new UserPasswordRecoveryTool());
            Register(new BitLockerAuditTool());
            Register(new HardeningAntiRansomwareTool());
            Register(new ToggleFirewallPortsTool());
            Register(new PortScannerTool());
            Register(new ResetWindowsFirewallTool());
            Register(new ToggleIcmpPingResponseTool());
            Register(new EmergencyNetworkQuarantineTool());
            Register(new FirewallPortManagerHubTool());
            Register(new ToggleFirewallProfilesTool());

            // --- LICENSE AUDIT & MANAGEMENT ---
            Register(new AuditWindowsLicenseTool());
            Register(new ExtractOemBiosKeyTool());
            Register(new CleanKmsAndResetTokensTool());
            Register(new DiagnoseOfficeLicenseTool());
            Register(new ToggleOfficeUpdateLockTool());
            Register(new SwitchWindowsEditionTool());
        }
    }
}
