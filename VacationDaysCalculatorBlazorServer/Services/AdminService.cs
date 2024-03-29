﻿using DomainModel.DtoModels;
using DomainModel.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace VacationDaysCalculatorBlazorServer.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthenticationStateProvider _customAuthenticationStateProvider;
        private readonly string BaseApiUrl = "https://localhost:7058/api/Admin";
        public AdminService(HttpClient httpClient, CustomAuthenticationStateProvider customAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
        }
        public async Task<AdminDetails> GetAdminDetailsAsync()
        {
            try
            {
				await _customAuthenticationStateProvider.DeleteExpiredTokenFromLocalStorage();
				int userId = await _customAuthenticationStateProvider.GetUserId();
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _customAuthenticationStateProvider.GetTokenAsync());
				return await _httpClient.GetFromJsonAsync<AdminDetails>($"{BaseApiUrl}/{userId}");
			}
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public async Task AddUserAsync(UserDetails userDetails)
        {
            try
            {
			    await _customAuthenticationStateProvider.DeleteExpiredTokenFromLocalStorage();
			    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _customAuthenticationStateProvider.GetTokenAsync());
                var httpPostRequest = new HttpRequestMessage(HttpMethod.Post, BaseApiUrl);
                httpPostRequest.Content = new StringContent(JsonSerializer.Serialize(userDetails), Encoding.UTF8, "application/json");
                await _httpClient.SendAsync(httpPostRequest);
            }
            catch (Exception e)
            {
				Console.WriteLine(e);
			}
        }
        public async Task<List<Vacation>> GetApprovedVacations()
        {
            try
            {
			    await _customAuthenticationStateProvider.DeleteExpiredTokenFromLocalStorage();
			    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _customAuthenticationStateProvider.GetTokenAsync());
                return await _httpClient.GetFromJsonAsync<List<Vacation>>($"{BaseApiUrl}/approvedVacations");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
		public async Task<List<string>> GetUsernamesAsync()
		{
			try
			{
				await _customAuthenticationStateProvider.DeleteExpiredTokenFromLocalStorage();
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _customAuthenticationStateProvider.GetTokenAsync());
				return await _httpClient.GetFromJsonAsync<List<string>>($"{BaseApiUrl}/usernames");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}
	}
}
