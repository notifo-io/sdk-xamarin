using System;
using System.Net.Http;
using System.Threading;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Notifo.SDK.UnitTests")]

namespace NotifoIO.SDK
{
	public partial class Notifo
	{
		public static readonly string Greeting = $"Hello Notifo from {PlatformName}";

		private static readonly Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(() => new HttpClient(), LazyThreadSafetyMode.PublicationOnly);
		private static readonly Lazy<INotifoMobilePush> Instance = new Lazy<INotifoMobilePush>(() => new NotifoMobilePush(HttpClient.Value), LazyThreadSafetyMode.PublicationOnly);

		public static INotifoMobilePush Current => Instance.Value;
	}
}
