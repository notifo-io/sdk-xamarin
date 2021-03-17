using System;
using System.Threading;
using Serilog;


namespace NotifoIO.SDK
{
	public partial class Notifo
	{
		private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);
		public static INotifoMobilePush Current => Instance.Value;

		private static INotifoMobilePush SetupNotifoMobilePush()
		{
			ConfigureLogger();

			var httpService = new HttpService();
			return new NotifoMobilePush(httpService);
		}

		private static void ConfigureLogger()
		{
			Log.Logger = new LoggerConfiguration()
#if DEBUG
				.MinimumLevel.Debug()
#else
				.MinimumLevel.Information()
#endif
				.WriteTo.PlatformSink()
				.CreateLogger();
		}
	}
}
