﻿using DomainModel.DtoModels;
using DomainModel.Enums;
using DomainModel.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using VacationDaysCalculatorBlazorServer.Service;
using VacationDaysCalculatorBlazorServer.Services;

namespace VacationDaysCalculatorBlazorServer.Pages.RazorPageBases
{
    public class AdminPageBase : ComponentBase
    {
        protected bool dense = false;
        protected bool hover = true;
        protected bool striped = false;
        protected bool bordered = false;
        protected string searchString1 = "";

        protected Vacation selectedVacation { get; set; }
        [Inject]
        protected AdminService _adminService { get; set; }
        [Inject]
        protected EmployeeService _employeeService { get; set; }
        [Inject]
        protected NavigationManager _navigationManager { get; set; }
        protected ConfirmationDialog CancelConfirmationDialog { get; set; }
        protected AdminDetails adminDetails { get; set; }

        protected IEnumerable<Vacation> vacations = new List<Vacation>();
        protected override async Task OnInitializedAsync()
        {
            adminDetails = await _adminService.GetAdminDetailsAsync();
            vacations = adminDetails.EmployeeVacationDays;
        }
        public bool FilterFunction(Vacation vacationRequests) => FilterFunc(vacationRequests, searchString1);

        private bool FilterFunc(Vacation vacationRequests, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (vacationRequests.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (vacationRequests.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{vacationRequests.Status}".Contains(searchString))
                return true;
            return false;
        }
        protected async Task ApproveVacationAsync(Vacation vacation)
        {
            vacation.Status = VacationStatus.Approved;
            vacation.ApprovedBy = adminDetails.LastName + " " + adminDetails.FirstName;
            await _employeeService.ApproveVacationAsync(vacation);
            adminDetails = await _adminService.GetAdminDetailsAsync();
            vacations = adminDetails.EmployeeVacationDays;
        }
        protected async Task CancelVacationAsync(Vacation vacation)
        {
            selectedVacation = vacation;
            CancelConfirmationDialog.Show();
        }
        protected async Task OpenAddUserPage()
        {
            _navigationManager.NavigateTo("/AddUser");
        }
        protected async Task OpenApprovedVacations()
        {
            _navigationManager.NavigateTo("/ApprovedVacations");
        }
        protected async Task OnCancelConfirmationSelected(bool isCancelConfirmed)
        {
            if (isCancelConfirmed)
            {
                selectedVacation.Status = VacationStatus.Cancelled;
                await _employeeService.CancelVacationAsync(selectedVacation);
                adminDetails = await _adminService.GetAdminDetailsAsync();
                vacations = adminDetails.EmployeeVacationDays;
            }
        }
    }
}
