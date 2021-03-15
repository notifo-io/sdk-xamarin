using System;

namespace NotifoIO.SDK
{
	public class TokenRefreshEventArgs : EventArgs
	{
		public string Token { get; set; }

		public TokenRefreshEventArgs(string token)
		{
			Token = token;
		}		
	}
}
