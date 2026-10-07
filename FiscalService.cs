using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PDVCompleto
{
    public class FiscalService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_URL = "https://api.exemplo-fiscal.com/v1/nfe";
        private const string API_TOKEN = "SEU_TOKEN_AQUI";

        public async Task<string> EmitirNFe(string jsonDadosVenda)
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", API_TOKEN);

            var content = new StringContent(jsonDadosVenda, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(API_URL, content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}