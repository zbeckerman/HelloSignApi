﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace HelloSignApi
{
    /// <summary>
    /// Main client for performaing HelloSign api calls.
    /// </summary>
    public partial class HelloSignClient
    {
        static readonly ConcurrentDictionary<string, HttpClient> __clientCache = new ConcurrentDictionary<string, HttpClient>();


        string _apiKey;
        HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloSignClient" /> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="getHttpClientRoutine">Optional custom routine to provide an <see cref="HttpClient" />.</param>
        /// <exception cref="ArgumentException">Api key is required.;apiKey</exception>
        public HelloSignClient(string apiKey, Func<HttpClient> getHttpClientRoutine = null)
        {
            if (string.IsNullOrEmpty(apiKey)) { throw new ArgumentException("Api key is required.", "apiKey"); }
            _apiKey = apiKey;

            if (getHttpClientRoutine != null) _client = getHttpClientRoutine();

            if (_client == null)
            {
                _client = __clientCache.GetOrAdd(apiKey, key =>
                 {
                     var client = new HttpClient();
                     return client;
                 });
            }

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":")));

        }

        /// <summary>
        /// Parsed the data received from the event callback.
        /// </summary>
        /// <param name="eventData">json data from the callback.</param>
        /// <param name="verify">Validate the event for integrity. If failed the return value is null.</param>
        /// <returns></returns>
        public Event ParseEvent(string eventData, bool verify = true)
        {
            var wrap = JsonConvert.DeserializeObject<EventWrap>(eventData ?? "", HttpResponseExtensions.JsonSettings);
            if (verify && wrap != null && wrap.Event != null)
            {
                using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(_apiKey)))
                {
                    var input = $"{wrap.Event.EventTimeRaw}{wrap.Event.EventType}";
                    var hash = BitConverter.ToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", "");

                    if (!string.Equals(hash, wrap.Event.EventHash, StringComparison.OrdinalIgnoreCase))
                    {
                        wrap = null;
                    }
                }
            }
            return wrap?.Unwrap();
        }
    }
}
