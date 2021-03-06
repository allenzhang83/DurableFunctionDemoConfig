﻿using DurableFunctionDemoConfig.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionDemoConfig.Services
{
    public class GitHubApiService : IGitHubApiService
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;
        private HttpClient _httpClient = new HttpClient();        

        public GitHubApiService(IOptions<GitHubApiConfig> gitHubApiConfig)
        {
            var configValue = gitHubApiConfig.Value;
            _username = configValue.Username;
            _password = configValue.Password;
            _baseUrl = configValue.BaseUrl;
        }

        public async Task<RepoViewCount> GetRepositoryViewCount(string repoName)
        {
            var result = new RepoViewCount
            {
                RepoName = repoName
            };

            PrepareHttpClient();
            var url = $"{_baseUrl}/repos/{_username}/{repoName}/traffic/views";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            JObject jObject = JsonConvert.DeserializeObject<JObject>(body);
            result.ViewCount = jObject["count"].Value<int>();
            return result;
        }

        public async Task<List<string>> GetUserRepositoryList()
        {
            var result = new List<string>();
            PrepareHttpClient();
            var url = $"{_baseUrl}/users/{_username}/repos";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            List<JObject> jObjects = JsonConvert.DeserializeObject<List<JObject>>(body);
            foreach (var jObject in jObjects)
            {
                string repoName = jObject["name"].Value<string>();
                result.Add(repoName);
            }
            return result;
        }

        private void PrepareHttpClient()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("C# agent");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}")));
        }
    }
}
