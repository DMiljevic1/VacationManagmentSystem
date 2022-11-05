﻿using System.Text.Json;
using System.Text;
using DomainModel.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace VacationDaysCalculatorBlazorServer.Services
{
    public class LogInService
    {
        private readonly HttpClient _httpClient;
        private readonly string BaseApiUrl = "https://localhost:7058/api/Login";
        public LogInService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendUserAsync(UserLogin userLogin)
        {
            var httpGetRequest = new HttpRequestMessage(HttpMethod.Get, BaseApiUrl);
            httpGetRequest.Content = new StringContent(JsonSerializer.Serialize(userLogin), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(httpGetRequest);
            if (response.IsSuccessStatusCode)
            {
                string token = await response.Content.ReadAsStringAsync();
                return token;
            }
            return null;
        }
    }
}
