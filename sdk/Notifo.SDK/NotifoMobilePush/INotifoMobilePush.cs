namespace NotifoIO.SDK
{
	public interface INotifoMobilePush
	{
		INotifoMobilePush SetApiKey(string appId);
		INotifoMobilePush SetBaseUrl(string baseUrl);
		INotifoMobilePush SetPushEventsProvider(IPushEventsProvider pushEventsProvider);
	}
}
