namespace WebAppMonitor.Data
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class QueryStats : DbContext
	{
		public QueryStats()
			: base("name=QueryStats") {
		}

		public virtual DbSet<Date> Dates { get; set; }
		public virtual DbSet<NormQueryTextHistory> NormQueryTextHistories { get; set; }
		public virtual DbSet<QueryHistory> QueryHistories { get; set; }
		public virtual DbSet<TodayNormQueryText> TodayNormQueryTexts { get; set; }
		public virtual DbSet<TodayQuery> TodayQueries { get; set; }
		public virtual DbSet<QueryStatInfo> QueryStatInfoes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<NormQueryTextHistory>()
				.Property(e => e.NormalizedQuery)
				.IsUnicode(false);

			modelBuilder.Entity<TodayNormQueryText>()
				.Property(e => e.NormalizedQuery)
				.IsUnicode(false);
			modelBuilder.Entity<QueryStatInfo>()
				.Property(e => e.QueryText)
				.IsUnicode(false);
		}
	}
}
