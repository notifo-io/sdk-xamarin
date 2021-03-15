﻿using System;
using System.Net.Http;
using System.Threading;
using Serilog;


namespace NotifoIO.SDK
{
	public partial class Notifo
	{
		public static readonly string Greeting = $"Hello Notifo from {PlatformName}";
				
		private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => SetupNotifoMobilePush(), LazyThreadSafetyMode.PublicationOnly);
		public static INotifoMobilePush Current => Instance.Value;

		private static INotifoMobilePush SetupNotifoMobilePush()
		{
			ConfigureLogger();

			var httpClient = new HttpClient();
			return new NotifoMobilePush(httpClient);
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
