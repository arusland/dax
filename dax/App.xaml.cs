using System.Threading;
using System.Windows;

namespace dax
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
		}
	}
}
