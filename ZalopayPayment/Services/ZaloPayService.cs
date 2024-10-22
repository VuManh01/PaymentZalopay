using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;

namespace ZalopayPayment.Services
{
    public class ZaloPayService
    {
        private readonly string appId;
        private readonly string key1;
        private readonly string createOrderUrl;

        public ZaloPayService(IConfiguration configuration)
        {
            appId = configuration["ZaloPay:AppId"];
            key1 = configuration["ZaloPay:Key1"];
            createOrderUrl = configuration["ZaloPay:CreateOrderUrl"];
        }

        public string CreateOrder(decimal amount, string orderDescription)
        {
            string appTransId = DateTime.Now.ToString("yyMMddHHmmss");
            long appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var order = new
            {
                appid = appId,
                apptransid = appTransId,
                appuser = "demo_user",
                amount = amount,
                apptime = appTime,
                embeddata = "{}",
                item = "[]",
                description = orderDescription,
                mac = GenerateMac(appId, amount, appTransId)
            };

            return SendOrderRequest(order);
        }

        private string SendOrderRequest(object order)
        {
            var client = new RestClient(createOrderUrl);
            var request = new RestRequest(createOrderUrl, Method.Post);

            request.AddJsonBody(order);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception($"Error creating order: {response.StatusCode} - {response.Content}");
            }
        }

        private string GenerateMac(string appId, decimal amount, string appTransId)
        {
            string data = $"{appId}|{amount}|{appTransId}";
            return HmacSHA256(data, key1);
        }

        private static string HmacSHA256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
