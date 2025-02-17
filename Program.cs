using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoPriceChecker
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.Write("Введите код криптовалюты (например, BTC): ");
            string cryptoCode = Console.ReadLine().ToLower();

            try
            {
                decimal price = await GetCryptoPriceAsync(cryptoCode);
                Console.WriteLine($"Текущая цена {cryptoCode.ToUpper()} составляет {price} USD.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static async Task<decimal> GetCryptoPriceAsync(string cryptoCode)
        {
            string url = $"https://api.coingecko.com/api/v3/simple/price?ids={cryptoCode}&vs_currencies=usd";
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось получить данные от API.");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            if (json[cryptoCode] == null || json[cryptoCode]["usd"] == null)
            {
                throw new Exception("Неверный код криптовалюты или данные недоступны.");
            }

            return json[cryptoCode]["usd"].Value<decimal>();
        }
    }
}
