using SDAConsole.types.ConfigHub;
using SDAConsole.types;
using SDAConsole.interfaces.API;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SDAConsole.interfaces.ConfigHub
{

	public interface IConfigHubConfig
	{

		string BaseUrl_Prod { get; }
		string SessionUrl { get; }
        string GetBaselineByNameUrl { get; }
        string GetCompatibleHardwareFromBaselineByIDUrl { get; }
        string GetCompatibleSoftwareFromBaselineByIDUrl { get; }
        int TimeoutInSecs { get; }
		string UserName { get; }
		string Password { get; }
        string TargetBaselineName { get; }
        string TargetBaselineID { get; }
        public ConfigHubBaseline BaselineMetaData { get; }

        void SetTargetBaselineID(string baseLine);
        void SetBaselineMetaData(ConfigHubBaseline baseLines);
    }

    public interface IConfigHubClient
	{
		Task SignInAsync(string userName, string password, ServerInstanceOption server, string token);


	}

    public interface ILoginController
    {
        Task<string> Login(string userName, string password, string token);
    }


    public class LoginController : ILoginController
	{
		private readonly IConfigHubConfig configManager;
		private readonly IApiClient apiClient;


		public LoginController(IConfigHubConfig configManager, IApiClient apiClient)
		{
			this.configManager = configManager;
			this.apiClient = apiClient;
		}

		public async Task<string> Login(string userName, string password, string token)
		{
            try
			{
             string authorizationToken = "";

                    if (string.IsNullOrEmpty(token))
					{
                        var credentials = new Dictionary<string, string>
                        {
                            { "userName", userName },
                            { "password", password }
                        };
                        var credsJson = JsonConvert.SerializeObject(credentials);
                        var tokenJson = await apiClient.PostJsonAsync(configManager.SessionUrl, credsJson);
                        // var tokenObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenJson);
                        authorizationToken = tokenJson;


                    }
                    else
					{
                         authorizationToken = token;

                    }

				return authorizationToken;

			}
			catch (Exception ex)
			{
				throw new ConfigHubException("ConfigHub login failure.", ex);
			}
		}
	}
}
