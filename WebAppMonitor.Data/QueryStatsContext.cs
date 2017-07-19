using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Data
{
	internal sealed class Configuration : DbMigrationsConfiguration<QueryStatsContext>
	{
		public Configuration() {
			AutomaticMigrationsEnabled = false;
		}
	}

	public class QueryStatsContext : DbContext
	{

		static QueryStatsContext() {
			Database.SetInitializer<QueryStatsContext>(null);
		}

		public QueryStatsContext(string  connectionString)
			: base(connectionString) {
		}

		public QueryStatsContext() {
			
		}

		public virtual DbSet<Date> Dates { get; set; }
		public virtual DbSet<QueryStatInfo> QueryStatInfo { get; set; }
		public virtual DbSet<Setting> Settings { get; set; }
		public virtual DbSet<ReaderQueryStack> ReaderQueryStack { get; set; }
		public virtual DbSet<ExecutorQueryStack> ExecutorQueryStack { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Configurations.AddFromAssembly(typeof(QueryStatsContext).Assembly);
			modelBuilder.Entity<Setting>().HasKey(s => s.Id);
		}

		public class QueryStatInfoMap: EntityTypeConfiguration<QueryStatInfo>
		{
			public QueryStatInfoMap()
			{
				HasKey(i => i.NormalizedQueryTextId);
				ToTable("QueryStatInfo");
				Property(e => e.QueryText).IsUnicode(false);
			}
		}
		public class ReaderQueryStackMap : EntityTypeConfiguration<ReaderQueryStack>
		{
			public ReaderQueryStackMap()
			{
				HasKey(i => new { i.DateId , i.QueryId, i.StackId });
				ToTable("VwReaderQueryStack");
			}
		}
		public class ExecutorQueryStackMap : EntityTypeConfiguration<ExecutorQueryStack>
		{
			public ExecutorQueryStackMap()
			{
				HasKey(i => new { i.DateId , i.QueryId, i.StackId });
				ToTable("VwExecutorQueryStack");
			}
		}
	}
}
