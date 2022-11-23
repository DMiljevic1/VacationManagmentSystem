﻿using DomainModel.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using VacationDaysCalculatorBlazorServer.Services;

namespace VacationDaysCalculatorBlazorServer.Pages.RazorPageBases
{
    public class LoginPageBase : ComponentBase
    {
        public UserLogin userLogin { get; set; }
        protected string message { get; set; }

        protected List<User> Users { get; set; }

        [Inject]
        protected LogInService _logInService { get; set; }

        [Inject]
        protected NavigationManager _navigationManager { get; set; }

        [Inject]
        protected CustomAuthenticationStateProvider _customAuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            userLogin = new UserLogin();
            message = "";
        }

        protected async void Login()
        {
            await _logInService.SendUserAsync(userLogin);
            var token = await _customAuthenticationStateProvider.GetTokenAsync();
            if (token != null)
                _navigationManager.NavigateTo("/");
            else
            {
                message = "Incorect username or password";
                _navigationManager.NavigateTo("/LoginPage");
            }
        }

        protected async void LogOut()
        {
            await _customAuthenticationStateProvider.RemoveItem("authToken");
            _navigationManager.NavigateTo("/LoginPage");
        }
    }
}
