using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace WebAppMonitor.Data
{
	public class QueryStatsContext : DbContext
	{
		public QueryStatsContext(string  connectionString)
			: base(connectionString) {
		}

		public QueryStatsContext() {
			
		}

		public virtual DbSet<Date> Dates { get; set; }
		public virtual DbSet<QueryStatInfo> QueryStatInfo { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Configurations.AddFromAssembly(typeof(QueryStatsContext).Assembly);
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
	}
}
