﻿using DomainModel.DtoModels;
using Microsoft.AspNetCore.Components;
using VacationDaysCalculatorBlazorServer.Service;
using DomainModel.Models;
using VacationDaysCalculatorBlazorServer.Services;
using DomainModel.Enums;
using MudBlazor;
using VacationDaysCalculatorBlazorServer.ValidationModels;

namespace VacationDaysCalculatorBlazorServer.Pages.RazorPageBases
{
    public class EmployeePageBase : ComponentBase
    {
        [Inject]
        protected SickLeaveService _sickLeaveService { get; set; }
        [Inject]
        protected IDialogService _dialogService { get; set; }
        [Inject]
        protected EmployeeService _employeeService { get; set; }
        [Inject]
        protected NavigationManager _navigationManager { get; set; }
        protected EmployeeDetails currentEmployee { get; set; }
        protected List<Vacation> approvedAndPendingVacationRequests { get; set; }
        protected string vacationOrSickLeave { get; set; }
        protected override async Task OnInitializedAsync()
        {
            currentEmployee = await _employeeService.GetEmployeeDetailsAsync();
            if(currentEmployee != null)
                approvedAndPendingVacationRequests = currentEmployee.VacationDays.Where(v => v.Status == VacationStatus.Pending || v.Status == VacationStatus.Approved).ToList();
        }
        protected void OpenEmployeeHistoryPage()
        {
            _navigationManager.NavigateTo("/EmployeeHistory");
        }
        protected void OpenAddVacationPage()
        {
            _navigationManager.NavigateTo("/AddVacation");
        }
        protected async Task DeleteVacationRequest(int vacationId)
        {
            await _employeeService.DeleteVacationRequestAndRestoreVacationAsync(vacationId);
            currentEmployee = await _employeeService.GetEmployeeDetailsAsync();
            approvedAndPendingVacationRequests = currentEmployee.VacationDays.Where(v => v.Status == VacationStatus.Pending || v.Status == VacationStatus.Approved).ToList();
        }
        protected string searchString1 = "";
        protected Vacation selectedVacation { get; set; }
        public bool FilterFunction(Vacation vacationRequest) => FilterFunc(vacationRequest, searchString1);

        private bool FilterFunc(Vacation vacationRequest, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (vacationRequest.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (vacationRequest.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (vacationRequest.VacationRequestDate.ToString("dd.MM.yyyy.hh:mm").Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (vacationRequest.VacationFrom.ToString("dd.MM.yyyy.").Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (vacationRequest.VacationTo.ToString("dd.MM.yyyy.").Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
		public bool FilterSickLeaveDelegate(SickLeave sickLeave) => FilterSickLeaveFunction(sickLeave, searchString1);
		private bool FilterSickLeaveFunction(SickLeave sickLeave, string searchString)
		{
			if (string.IsNullOrWhiteSpace(searchString))
				return true;
			if (sickLeave.User.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
				return true;
			if (sickLeave.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
				return true;
			if (sickLeave.SickLeaveFrom.ToString("dd.MM.yyyy.hh:mm").Contains(searchString, StringComparison.OrdinalIgnoreCase))
				return true;
			return false;
		}
		protected async Task OpenAddVacationDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = _dialogService.Show<AddVacationDialog>("Send Vacation Request", options);
			var result = await dialog.Result;
			if (!result.Cancelled)
			{
				currentEmployee = await _employeeService.GetEmployeeDetailsAsync();
				if (currentEmployee != null)
					approvedAndPendingVacationRequests = currentEmployee.VacationDays.Where(v => v.Status == VacationStatus.Pending || v.Status == VacationStatus.Approved).ToList();
			}
		}
        protected void OpenErrorDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "You don't have enough vacation for that period.");
            _dialogService.Show<ErrorDialog>("Error",parameters, options);
        }
        protected async Task AddVacation(Vacation newVacation)
        {
            var vacationList = new List<DateTime>();
            vacationList.Add(newVacation.VacationFrom);
            vacationList.Add(newVacation.VacationTo);
            int daysInVacationRequest = await _employeeService.CalculateTotalVacationForGivenPeriodAsync(vacationList);
            if (daysInVacationRequest <= currentEmployee.RemainingDaysOffCurrentYear + currentEmployee.RemainingDaysOffLastYear)
            {
                newVacation.Status = VacationStatus.Pending;
                newVacation.VacationRequestDate = DateTime.Now;
                await _employeeService.AddVacationAsync(newVacation);
            }
            else
            {
                OpenErrorDialog();
            }
        }
        protected bool isCurrentUserOnVacation()
        {
            if(currentEmployee != null && currentEmployee.VacationDays != null && currentEmployee.VacationDays.Count > 0)
            {
                foreach(var vacationRequest in currentEmployee.VacationDays)
                {
                    if (vacationRequest.Status == VacationStatus.OnVacation)
                        return true;
                }
            }
            return false;
        }
        protected async Task CloseSickLeave(SickLeave sickLeave)
        {
			var options = new DialogOptions { CloseOnEscapeKey = true };
			var dialog = _dialogService.Show<MudConfirmationDialog>("Are you sure you want to close sick leave?", options);
			var result = await dialog.Result;
            if(!result.Cancelled)
            {
			    await _sickLeaveService.CloseSickLeaveAsync(sickLeave);
			    currentEmployee = await _employeeService.GetEmployeeDetailsAsync();
			    if (currentEmployee != null)
				    approvedAndPendingVacationRequests = currentEmployee.VacationDays.Where(v => v.Status == VacationStatus.Pending || v.Status == VacationStatus.Approved).ToList();
            }
		}
        protected void OpenMedicalCertificatePage(int sickLeaveId)
        {
            _navigationManager.NavigateTo("Employee/MedicalCertificate/" + sickLeaveId);
        }

        protected async Task OpenAddSickLeaveDialog()
        {
			var options = new DialogOptions { CloseOnEscapeKey = true };
			var dialog = _dialogService.Show<AddSickLeave>("Open Sick Leave", options);
			var result = await dialog.Result;
			if (!result.Cancelled)
			{
				currentEmployee = await _employeeService.GetEmployeeDetailsAsync();
			}
		}

        protected async Task AddSickLeaveAsync(SickLeave sickLeave)
        {
            await _sickLeaveService.AddSickLeave(sickLeave);
        }
    }
}
