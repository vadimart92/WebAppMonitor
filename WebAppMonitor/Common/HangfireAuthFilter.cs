using Hangfire.Dashboard;

namespace WebAppMonitor.Common
{
    public class HangfireAuthFilter:IDashboardAuthorizationFilter
    {

	    public bool Authorize(DashboardContext context) {
		    return true;
	    }

    }
}
