using System;

namespace WebAppMonitor.Core.Common
{
	public class ResettableLazy<T>
	{
		public T Value => _container.Value;
		public bool IsValueCreated => _container.IsValueCreated;
		public void Reset() => _container = new Lazy<T>(_valueFactory);

		private Lazy<T> _container;
		private readonly Func<T> _valueFactory;

		public ResettableLazy(Func<T> valueFactory) {
			_valueFactory = valueFactory;
			Reset();
		}
	}
}
