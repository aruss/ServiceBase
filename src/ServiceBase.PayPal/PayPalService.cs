namespace ServiceBase.PayPal
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using ServiceBase.Extensions;
    using ServiceBase.PayPal.V2;

    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PayPalService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            this._httpClient = httpClientFactory.CreateClient();
            this._cache = cache;

            this._jsonSerializerSettings = new JsonSerializerSettings();

            this._jsonSerializerSettings.Converters.Add(
                new JavaScriptDateTimeConverter());

            this._jsonSerializerSettings
                .NullValueHandling = NullValueHandling.Ignore;
        }

        /// <summary>
        /// Creates a <see cref="HttpRequestMessage"/> with default
        /// headers
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private HttpRequestMessage CreateRequestMessage(
            HttpMethod method,
            string requestUri)
        {
            HttpRequestMessage message = new HttpRequestMessage(
                method,
                requestUri
            );

            message.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            message.Headers.AcceptLanguage.Add(
                new StringWithQualityHeaderValue("en_US"));

            return message;
        }

        /// <summary>
        /// Retrieves token from PayPal REST API
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task<V1.GetTokenResult> GetTokenAsync(
            PayPalOptions options)
        {
            HttpRequestMessage request = this.CreateRequestMessage(
                HttpMethod.Post,
                $"{options.BaseUrl.RemoveTrailingSlash()}/v1/oauth2/token"
            );

            request.AddBasicAuthorizationHeader(
                options.ClientId,
                options.Secret);

            request.Content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(
                        "grant_type",
                        "client_credentials"
                    )
                }
            );

            HttpResponseMessage response =
                await this._httpClient.SendAsync(request);

            #region Parse response message

            V1.GetTokenResult result = new V1.GetTokenResult();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Token = await response.Content
                    .ReadAsAsync<V1.TokenResponseBody>();
            }
            else
            {
                result.Error = await response.Content
                    .ReadAsAsync<V1.ErrorResponseBody>();
            }

            #endregion 

            return result;
        }

        /// <summary>
        /// Adds PayPal access token to <see cref="HttpRequestMessage"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task AddAccessTokenAsync(
            HttpRequestMessage message,
            PayPalOptions options)
        {
            V1.GetTokenResult tokenResult = await this.GetTokenAsync(options);

            message.AddBearerAuthorizationHeader(
                tokenResult.Token.AccessToken
            );
        }

        /// <summary>
        /// Creates an PayPal order, needs to be confirmed by user though.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<CreateOrderResult> CreateOrderAsync(
            CreateOrderRequestBody body,
            PayPalOptions options,
            Action<HttpRequestMessage, HttpResponseMessage> logHttp = null)
        {
            #region Create request message

            HttpRequestMessage request = this.CreateRequestMessage(
                HttpMethod.Post,
                $"{options.BaseUrl.RemoveTrailingSlash()}/v2/checkout/orders"
            );

            await this.AddAccessTokenAsync(request, options);
            request.AddJsonBody(body, this._jsonSerializerSettings);

            #endregion

            HttpResponseMessage response =
                await this._httpClient.SendAsync(request);

            logHttp?.Invoke(request, response);

            #region Parse response message

            CreateOrderResult result = new CreateOrderResult();

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                result.Order = await response.Content
                    .ReadAsAsync<CreateOrderResponseBody>();
            }
            else
            {
                result.Error = await response.Content
                    .ReadAsAsync<ErrorResponseBody>();
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Captures payment for an order. To successfully capture payment for
        /// an order, the buyer must first approve the order or a valid
        /// payment_source must be provided in the request.
        /// <see cref="https://developer.paypal.com/docs/api/orders/v2/#orders_capture"/>
        /// </summary>
        /// <param name="orderId">The ID of the order for which to capture a payment.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<CaptureOrderResult> CaptureOrderAsync(
            string orderId,
            PayPalOptions options,
            Action<HttpRequestMessage, HttpResponseMessage> logHttp = null)
        {
            #region Create request message

            HttpRequestMessage request = this.CreateRequestMessage(
                HttpMethod.Post,
                $"{options.BaseUrl.RemoveTrailingSlash()}/v2/checkout/orders/{orderId}/capture"
            );

            // Dummy crap, so the content type is set.
            // Since the PayPal api will answer with
            // Unsuported Media Type error if Content-Type header is not set
            request.Content = new StringContent(
                string.Empty,
                UnicodeEncoding.UTF8,
                "application/json");

            await this.AddAccessTokenAsync(request, options);

            #endregion

            HttpResponseMessage response =
                await this._httpClient.SendAsync(request);

            logHttp?.Invoke(request, response); 

            #region Parse response message

            CaptureOrderResult result = new CaptureOrderResult();

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                result.Order = await response.Content
                    .ReadAsAsync<CaptureOrderResposeBody>();
            }
            else
            {
                result.Error = await response.Content
                    .ReadAsAsync<ErrorResponseBody>();
            }

            #endregion

            return result;
        }
    }
}


