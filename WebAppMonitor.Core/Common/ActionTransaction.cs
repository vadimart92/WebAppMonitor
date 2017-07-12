using System;

namespace WebAppMonitor.Core.Common
{
    public class ActionTransaction:ITransaction
    {

	    private readonly Action _endAction;

	    public ActionTransaction(Action beginAction, Action endAction) {
		    _endAction = endAction;
		    beginAction();
	    }

	    public void Dispose() {
		    _endAction();
	    }

    }
}
