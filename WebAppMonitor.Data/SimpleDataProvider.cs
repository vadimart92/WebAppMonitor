using System.Collections.Generic;
using System.Data;
using Dapper;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data
{
	public class SimpleDataProvider: ISimpleDataProvider {
		private readonly IDbConnectionProvider _connectionProvider;

		public SimpleDataProvider(IDbConnectionProvider connectionProvider) {
			_connectionProvider = connectionProvider;
		}

		public IEnumerable<T> Enumerate<T>(string query, object parameters = null) {
			using (IDataReader reader = _connectionProvider.GetReader(new CommandDefinition(query, parameters))) {
				var parser = reader.GetRowParser<T>();
				while (reader.Read()) {
					yield return parser.Invoke(reader);
				}
			}
		}
	}
}
