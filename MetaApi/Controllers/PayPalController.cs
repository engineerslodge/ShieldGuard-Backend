using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static MetaApi.Models.VendorInfo;

namespace MetaApi.Controllers
{
    [AllowAnonymous]
    public class PayPalController : ApiController
    {
        private const string clientId = "Ae_ebUu5AWofWnBLfqPEHcTl0SKsyymN62f7YpfHGD78RDIGNtvYv6ter8fq38r4Mob7MBIENHdfOHsf";
        private const string clientSecret = "ELzWzHLDcmY5WvnmwItnIyWMjJFiFHuGj9MZvC5X6prkniBA4iI8h4yayzZFvelF2p0icuL2Iw50ZW09";

        private static string baseUrl = "https://api-m.sandbox.paypal.com"; // Use "https://api-m.paypal.com" for live env. //https://api-m.sandbox.paypal.com


        private static async Task<string> GetAccessToken()
        {
            string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com"); // https://api-m.paypal.com  // https://api-m.sandbox.paypal.com
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                };

                var content = new FormUrlEncodedContent(formData);

                HttpResponseMessage response = await client.PostAsync("/v1/oauth2/token", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                    return jsonResponse.access_token;
                }
                else
                {
                    throw new Exception("Failed to obtain access token. Status code: " + response.StatusCode);
                }
            }
        }

        public class Order
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public List<PurchaseUnit> PurchaseUnits { get; set; }
            public Payer Payera { get; set; }
            public List<Link> Links { get; set; }

            public class PurchaseUnit
            {
                public string ReferenceId { get; set; }
                public Amount Amount { get; set; }
                public Payee Payee { get; set; }
                public Shipping Shipping { get; set; }
                public List<Payment> Payments { get; set; }
            }

            public class Amount
            {
                public string CurrencyCode { get; set; }
                public string Value { get; set; }
            }

            public class Payee
            {
                public string Email { get; set; }
                public string MerchantId { get; set; }
            }

            public class Shipping
            {
                public Address Address { get; set; }
            }

            public class Address
            {
                public string AddressLine1 { get; set; }
                public string AddressLine2 { get; set; }
                public string AdminArea2 { get; set; }
                public string AdminArea1 { get; set; }
                public string PostalCode { get; set; }
                public string CountryCode { get; set; }
            }

            public class Payment
            {
                public List<Capture> Captures { get; set; }
            }

            public class Capture
            {
                public string Id { get; set; }
                public string Status { get; set; }
                public DateTime CreateTime { get; set; }
                public DateTime UpdateTime { get; set; }
                public Amount Amount { get; set; }
                public bool FinalCapture { get; set; }
            }

            public class Payer
            {
                public string EmailAddress { get; set; }
                public string PayerId { get; set; }
                public Name Name { get; set; }
                public Address Address { get; set; }
            }

            public class Name
            {
                public string GivenName { get; set; }
                public string Surname { get; set; }
            }

            public class Link
            {
                public string Href { get; set; }
                public string Rel { get; set; }
                public string Method { get; set; }
            }
        }

        [HttpGet]
        [Route("api/PayPal/CreateOrder")]
        public async Task<Order> CreateOrder(string amount)
        {
            string accessToken = await PayPalController.GetAccessToken();
            string baseUrl = "https://api-m.sandbox.paypal.com"; // Use https://api.paypal.com for live  ///https://api-m.sandbox.paypal.com
            double newamount = Convert.ToDouble(amount);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var orderRequest = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                new
                {
                    amount = new
                    {
                        currency_code = "EUR",
                        value = newamount
                    }
                }
            },
                    application_context = new
                    {
                        return_url = "http://app.shieldguardai.com/return",
                        cancel_url = "http://app.shieldguardai.com/cancel"
                    }
                };

                var jsonRequest = JsonConvert.SerializeObject(orderRequest);
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{baseUrl}/v2/checkout/orders", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Handle error response
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"PayPal Order creation failed: {errorResponse}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(jsonResponse);
                return order;
            }
        }




        [HttpGet]
        [Route("api/PayPal/ExecuteOrder")]
        public async Task<IHttpActionResult> ExecuteOrder(string orderId)
        {
            var accessToken = await PayPalController.GetAccessToken();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Fetch the order details first
                var response = await client.GetAsync($"{baseUrl}/v2/checkout/orders/{orderId}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var orderDetails = JsonConvert.DeserializeObject<OrderDetails>(jsonResponse);

                // Check if the order is approved
                if (orderDetails.Status != "APPROVED" && orderDetails.Status != "COMPLETED")
                {
                    return BadRequest("Order is not approved for capture.");
                }

                // Extract the payer ID from the order details
                var payerId = orderDetails.Payer.PayerId;

                // Prepare the capture request
                var captureRequest = new
                {
                    payer_id = payerId
                };

                var jsonRequest = JsonConvert.SerializeObject(captureRequest);
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                // Capture the order
                response = await client.PostAsync($"{baseUrl}/v2/checkout/orders/{orderId}/capture", content);
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync();
                var captureResult = JsonConvert.DeserializeObject<CaptureResult>(jsonResponse);

                // Check if the payment is successful
                if (captureResult != null && captureResult.Status == "COMPLETED")
                {
                    // Return the entire JSON response for inspection
                    return Ok(captureResult);
                }
                else
                {
                    return BadRequest("Payment was not successful.");
                }
            }
        }

        // Helper classes to deserialize the PayPal response
        public class OrderDetails
        {
            public string Status { get; set; }
            public PayerInfo Payer { get; set; }
        }

        public class PayerInfo
        {
            [JsonProperty("payer_id")]
            public string PayerId { get; set; }
        }

        public class CaptureResult
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("purchase_units")]
            public List<PurchaseUnit> PurchaseUnits { get; set; }

            [JsonProperty("payer")]
            public Payer Payer { get; set; }

            [JsonProperty("create_time")]
            public DateTime CreateTime { get; set; }
        }

        public class PurchaseUnit
        {
            [JsonProperty("amount")]
            public Amount Amount { get; set; }
        }

        public class Amount
        {
            [JsonProperty("currency_code")]
            public string CurrencyCode { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class Payer
        {
            [JsonProperty("payer_id")]
            public string PayerId { get; set; }

            [JsonProperty("email_address")]
            public string EmailAddress { get; set; }
        }

        [HttpGet]
        [Route("api/PayPal/Return")]
        public async Task<IHttpActionResult> Return(string token)
        {
            // Get the access token
            string accessToken = await PayPalController.GetAccessToken();
            string baseUrl = "https://api-m.sandbox.paypal.com"; // Use https://api.paypal.com for live  //"https://api.sandbox.paypal.com

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Retrieve the order details using the token (orderId)
                var orderResponse = await client.GetAsync($"{baseUrl}/v2/checkout/orders/{token}");
                orderResponse.EnsureSuccessStatusCode();

                var jsonResponse = await orderResponse.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                // Check the order status
                var status = order.status;

                if (status == "COMPLETED")
                {
                    // The payment is already completed, no need to capture again
                    return Ok(new { success = true, message = "Payment is already completed", result = order });
                }
                else if (status == "APPROVED")
                {
                    // Capture the payment
                    var captureResponse = await client.PostAsync($"{baseUrl}/v2/checkout/orders/{token}/capture", null);
                    captureResponse.EnsureSuccessStatusCode();

                    var captureJsonResponse = await captureResponse.Content.ReadAsStringAsync();
                    var captureResult = JsonConvert.DeserializeObject<dynamic>(captureJsonResponse);

                    // Check if the payment was successfully captured
                    var captureStatus = captureResult.status;

                    if (captureStatus == "COMPLETED")
                    {
                        // Return success response
                        return Ok(new { success = true, message = "Payment successfully captured", result = captureResult });
                    }
                    else
                    {
                        // Handle the case where the payment capture was not successful
                        return BadRequest("Payment capture not completed");
                    }
                }
                else
                {
                    // Handle the case where the payment is not approved
                    return BadRequest("Payment not approved or incomplete");
                }
            }
        }


       
    }
}
