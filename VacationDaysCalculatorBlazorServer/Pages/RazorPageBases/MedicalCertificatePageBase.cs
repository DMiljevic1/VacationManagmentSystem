﻿using DomainModel.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using System;
using System.IO;
using System.Text;
using VacationDaysCalculatorBlazorServer.Services;

namespace VacationDaysCalculatorBlazorServer.Pages.RazorPageBases
{
	public class MedicalCertificatePageBase : ComponentBase
	{
		[Parameter]
		public string sickLeaveId { get; set; }
		[Inject]
		protected IJSRuntime _jsruntime { get; set; }
		[Inject]
		protected SickLeaveService _sickLeaveService { get; set; }
		[Inject]
		protected NavigationManager _navigationManager { get; set; }
		protected List<MedicalCertificate> medicalCertificates { get; set; }

		protected IList<IBrowserFile> files = new List<IBrowserFile>();
		protected IFormFile file { get; set; }

		protected override async Task OnInitializedAsync()
		{
			medicalCertificates = await _sickLeaveService.GetMedicalCertificatesAsync(int.Parse(sickLeaveId));
		}

		protected void NavigateToEmployeePage()
		{
			_navigationManager.NavigateTo("/Employee");
		}

		protected void NavigateToSickLeaveList()
		{
			_navigationManager.NavigateTo("/SickLeaveList");
		}

		protected void NavigateToAdminPage()
		{
			_navigationManager.NavigateTo("/Admin");
		}

		protected async Task UploadMedicalCertificate(InputFileChangeEventArgs e, MedicalCertificate medicalCertificate)
		{
			foreach (var file in e.GetMultipleFiles(1))
			{
				await using var memoryStream = new MemoryStream();
				await file.OpenReadStream().CopyToAsync(memoryStream);
				medicalCertificate.Attachment = memoryStream.ToArray();
				medicalCertificate.FileSize = file.Size / 1000;  //divide by 1000 to get size from bytes to kb
				medicalCertificate.FileName = file.Name;
			}
            await _sickLeaveService.UploadMedicialCertificate(medicalCertificate);
			medicalCertificates = await _sickLeaveService.GetMedicalCertificatesAsync(int.Parse(sickLeaveId));
		}

		protected async Task DownloadMedicalCertificate(MedicalCertificate medicalCertificate)
		{
			await _jsruntime.InvokeVoidAsync("download", medicalCertificate.Attachment, medicalCertificate.FileName);
			medicalCertificates = await _sickLeaveService.GetMedicalCertificatesAsync(int.Parse(sickLeaveId));
		}
	}
}
