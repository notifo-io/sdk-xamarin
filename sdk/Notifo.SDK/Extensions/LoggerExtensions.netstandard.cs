using Serilog;
using Serilog.Configuration;

namespace NotifoIO.SDK
{
	internal static class LoggerExtensions
	{
		public static LoggerConfiguration PlatformSink(this LoggerSinkConfiguration configuration) =>
			configuration.Console();
	}
}
