using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace WebAppMonitor.DataProcessing.Tests {

	[TestFixture]
	public class StackProcessingUtilsTestCase {

		private static IEnumerable<object> TestCaseSources {
			get {
				yield return new TestCaseData(null, null);
				yield return new TestCaseData("()aa", "()aa");
				yield return new TestCaseData(@"   at System.Environment.GetStackTrace(Exception e, Boolean needFileInfo)
   at System.Environment.get_StackTrace()
   at Terrasoft.Core.DB.LoggingDataReader.CreateDestroyedEventArgs() in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\DB\\LoggingDataReader.cs:line 145
   at Terrasoft.Core.DB.LoggingDataReader.OnDestroy() in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\DB\\LoggingDataReader.cs:line 159
   at Terrasoft.Core.DB.LoggingDataReader.Dispose() in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\DB\\LoggingDataReader.cs:line 173
   at Terrasoft.Core.DB.Select.ExecuteReader(ExecuteReaderReadMethod readMethod) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\DB\\Select.cs:line 1163
   at Terrasoft.Core.Packages.PackageDBStorage.LoadPackagesDependencies(IEnumerable`1 packages) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\Packages\\PackageDBStorage.cs:line 2446
   at Terrasoft.Core.Packages.WorkspaceUtilities.GetTopologyPackagePositions(UserConnection userConnection, Guid workspaceId) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\Packages\\WorkspaceUtilities.cs:line 2654
   at Terrasoft.Core.SchemaManager`1.GetTopologyPackagePositions(Guid workspaceId) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\SchemaManager.cs:line 2805
   at Terrasoft.Core.SchemaManager`1.InitializeItems(Guid itemUId) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\SchemaManager.cs:line 2062
   at Terrasoft.Core.SchemaManager`1.InitializeItems() in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\SchemaManager.cs:line 2152
   at Terrasoft.Core.Entities.EntitySchemaManager.Initialize(SchemaManagerProvider provider, SchemaManagerProviderConfigurationElement configuration) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\Entities\\EntitySchemaManager.cs:line 601
   at Terrasoft.Core.SchemaManagerProvider.InitializeSchemaManager(String managerName) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\SchemaManagerProvider.cs:line 175
   at Terrasoft.Core.SchemaManagerProvider.GetManager(String managerName) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\SchemaManagerProvider.cs:line 249
   at Terrasoft.Core.UserConnection.GetSchemaManager(String schemaManagerName) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\UserConnection.cs:line 1620
   at Terrasoft.Core.UserConnection.get_EntitySchemaManager() in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Core\\UserConnection.cs:line 264
   at Terrasoft.Configuration.FeatureUtilities.GetFeatureState(UserConnection source, String code)
   at Terrasoft.Configuration.DelayedNotificationManagement.SetupDelayedNotificationJobs()
   at Terrasoft.Configuration.DelayedNotificationManagement.OnAppStart(AppEventContext context)
   at Terrasoft.Web.Common.AppEventDispatcher.\u003c\u003ec__DisplayClass9_0.\u003cOnAppStart\u003eb__0(IAppEventListener listener) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Web.Common\\AppEventDispatcher.cs:line 91
   at Terrasoft.Web.Common.AppEventDispatcher.OnAppEvent(Action`1 method) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Web.Common\\AppEventDispatcher.cs:line 75
   at Terrasoft.Web.Common.AppEventDispatcher.OnAppStart(AppEventContext context) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.Web.Common\\AppEventDispatcher.cs:line 91
   at Terrasoft.WebApp.Global.Application_Start(Object sender, EventArgs e) in C:\\Projects\\TSBpm\\Src\\Lib\\Terrasoft.WebApp.Loader\\Terrasoft.WebApp\\Global.asax.cs:line 389
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor)
   at System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
   at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
   at System.Web.HttpApplication.InvokeMethodWithAssert(MethodInfo method, Int32 paramCount, Object eventSource, EventArgs eventArgs)
   at System.Web.HttpApplication.ProcessSpecialRequest(HttpContext context, MethodInfo method, Int32 paramCount, Object eventSource, EventArgs eventArgs, HttpSessionState session)
   at System.Web.HttpApplicationFactory.EnsureAppStartCalledForIntegratedMode(HttpContext context, HttpApplication app)
   at System.Web.HttpApplication.RegisterEventSubscriptionsWithIIS(IntPtr appContext, HttpContext context, MethodInfo[] handlers)
   at System.Web.HttpApplication.InitSpecial(HttpApplicationState state, MethodInfo[] handlers, IntPtr appContext, HttpContext context)
   at System.Web.HttpApplicationFactory.GetSpecialApplicationInstance(IntPtr appContext, HttpContext context)
   at System.Web.Hosting.PipelineRuntime.InitializeApplication(IntPtr appContext)", @"at Terrasoft.Core.DB.Select.ExecuteReader(ExecuteReaderReadMethod readMethod)
at Terrasoft.Core.Packages.PackageDBStorage.LoadPackagesDependencies(IEnumerable`1 packages)
at Terrasoft.Core.Packages.WorkspaceUtilities.GetTopologyPackagePositions(UserConnection userConnection, Guid workspaceId)
at Terrasoft.Core.SchemaManager`1.GetTopologyPackagePositions(Guid workspaceId)
at Terrasoft.Core.SchemaManager`1.InitializeItems(Guid itemUId)
at Terrasoft.Core.SchemaManager`1.InitializeItems()
at Terrasoft.Core.Entities.EntitySchemaManager.Initialize(SchemaManagerProvider provider, SchemaManagerProviderConfigurationElement configuration)
at Terrasoft.Core.SchemaManagerProvider.InitializeSchemaManager(String managerName)
at Terrasoft.Core.SchemaManagerProvider.GetManager(String managerName)
at Terrasoft.Core.UserConnection.GetSchemaManager(String schemaManagerName)
at Terrasoft.Core.UserConnection.get_EntitySchemaManager()
at Terrasoft.Configuration.FeatureUtilities.GetFeatureState(UserConnection source, String code)
at Terrasoft.Configuration.DelayedNotificationManagement.SetupDelayedNotificationJobs()
at Terrasoft.Configuration.DelayedNotificationManagement.OnAppStart(AppEventContext context)
at Terrasoft.Web.Common.AppEventDispatcher.\u003c\u003ec__DisplayClass9_0.\u003cOnAppStart\u003eb__0(IAppEventListener listener)
at Terrasoft.Web.Common.AppEventDispatcher.OnAppEvent(Action`1 method)
at Terrasoft.Web.Common.AppEventDispatcher.OnAppStart(AppEventContext context)
at Terrasoft.WebApp.Global.Application_Start(Object sender, EventArgs e)
at System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor)
at System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments)
at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
at System.Web.HttpApplication.InvokeMethodWithAssert(MethodInfo method, Int32 paramCount, Object eventSource, EventArgs eventArgs)
at System.Web.HttpApplication.ProcessSpecialRequest(HttpContext context, MethodInfo method, Int32 paramCount, Object eventSource, EventArgs eventArgs, HttpSessionState session)
at System.Web.HttpApplicationFactory.EnsureAppStartCalledForIntegratedMode(HttpContext context, HttpApplication app)
at System.Web.HttpApplication.RegisterEventSubscriptionsWithIIS(IntPtr appContext, HttpContext context, MethodInfo[] handlers)
at System.Web.HttpApplication.InitSpecial(HttpApplicationState state, MethodInfo[] handlers, IntPtr appContext, HttpContext context)
at System.Web.HttpApplicationFactory.GetSpecialApplicationInstance(IntPtr appContext, HttpContext context)
at System.Web.Hosting.PipelineRuntime.InitializeApplication(IntPtr appContext)");
			}
		}

		[TestCaseSource(nameof(TestCaseSources))]
		public void NormalizeReaderStack(string sourceString, string expectedString) {
			string actual = sourceString.NormalizeReaderStack();
			actual.Should().BeEquivalentTo(expectedString);
		}
	}
}
