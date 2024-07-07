using PayPal.Api;

namespace EBMS.Infrastructure.Models.Payment
{
    public static class PaypalConfiguration
    {
        public static APIContext GetAPIContext(string clientId, string clientSecret, string mode)
        {
            APIContext apiContext = new APIContext(GetAccessToken(clientId, clientSecret, mode));

            apiContext.Config = GetConfig(mode);

            return apiContext;
        }

        private static Dictionary<string, string> GetConfig(string mode)
        {
            return new Dictionary<string, string>()
            {
                { "mode", mode }
            };
        }

        private static string GetAccessToken(string clientId, string clientSecret, string mode)
        {
            Dictionary<string, string> modeDic = new Dictionary<string, string>();
            modeDic.Add("mode", mode);

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, modeDic).GetAccessToken();

            return accessToken;
        }
    }
}
