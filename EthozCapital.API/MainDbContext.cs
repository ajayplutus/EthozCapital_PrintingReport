using System.Data.Entity;
using EthozCapital.API.Models.Tables;
using log4net;

namespace EthozCapital.API
{
	public class MainDbContext : DbContext
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(MainDbContext));

        public MainDbContext()
            : base("name=DefaultConnection")
        {
            glog.Debug("MainDbContext");
        }

        #region Sys
        public DbSet<Sys_Gentb_Mails> Sys_Gentb_Mails { get; set; }
		#endregion

		#region Approval
		public DbSet<Approval_Process> Approval_Process { get; set; }
		public DbSet<Approval_ProcessDetail> Approval_ProcessDetail { get; set; }
		public DbSet<Sys_ApprovalDetail> Sys_ApprovalDetail { get; set; }
		public DbSet<Sys_Approval> Sys_Approval { get; set; }

		#endregion

	}
}