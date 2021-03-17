using System.Net.Http;
using System.Threading.Tasks;

namespace NotifoIO.SDK
{
	internal interface IHttpService
	{
		Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string apiKey);
	}
}
