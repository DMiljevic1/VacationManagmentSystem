﻿using DomainModel.Enums;
using DomainModel.Models;
using VacationDaysCalculatorWebAPI.Repositories;

namespace VacationDaysCalculatorWebAPI.Services
{
	public class SickLeaveService
	{
		private readonly SickLeaveRepository _sickLeaveRepository;
		public SickLeaveService(SickLeaveRepository sickLeaveRepository)
		{
			_sickLeaveRepository = sickLeaveRepository;
		}
		public void CloseSickLeave(SickLeave sickLeave)
		{
			sickLeave.SickLeaveStatus = SickLeaveStatus.Closed;
			sickLeave.SickLeaveTo = DateTime.Today;
			_sickLeaveRepository.UpdateSickLeave(sickLeave);
		}
		public void AddSickLeave(SickLeave sickLeave)
		{
			if(sickLeave != null)
			{
				sickLeave.SickLeaveStatus = SickLeaveStatus.Opened;
				_sickLeaveRepository.AddSickLeave(sickLeave);
				AddMedicalCertificate(sickLeave.Id);
			}
		}

		public void AddMedicalCertificate(int sickLeaveId)
		{
            MedicalCertificate medicalCertificate = new MedicalCertificate();
            medicalCertificate.MedicalCertificateDate = DateTime.Now;
            medicalCertificate.SickLeaveId = sickLeaveId;
			_sickLeaveRepository.AddMedicalCertificate(medicalCertificate);
        }
	}
}
