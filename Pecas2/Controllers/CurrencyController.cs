using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pecas2.Services;
using System.Net.Http;

namespace Pecas2.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ApiService _apiService;

        public CurrencyController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDollarRate()
        {
            string url = "https://economia.awesomeapi.com.br/last/USD-BRL,EUR-BRL,BTC-BRL";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var jsonResponse = JObject.Parse(response);

                    
                    var dollarRate = jsonResponse["USDBRL"]?["bid"]?.ToString();

                    if (!string.IsNullOrEmpty(dollarRate))
                    {
                        
                        return Json(new { rate = dollarRate });
                    }
                    return Json(new { message = "Taxa não disponível" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Erro ao obter taxa: {ex.Message}" });
            }
        }

        public async Task<JsonResult> GetRandomCatImage()
        {
            throw new NotImplementedException();
        }
    }
}
