﻿using DomainModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VacationDaysCalculatorWebAPI.Repositories;
using VacationDaysCalculatorWebAPI.Services;

namespace VacationDaysCalculatorWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SickLeaveController : ControllerBase
	{
		private readonly SickLeaveService _sickLeaveService;
		private readonly SickLeaveRepository _sickLeaveRepository;
		private readonly EmailService _emailService;
		public SickLeaveController(SickLeaveService sickLeaveService, SickLeaveRepository sickLeaveRepository, EmailService emailService)
		{
			_sickLeaveService = sickLeaveService;
			_sickLeaveRepository = sickLeaveRepository;
			_emailService = emailService;
		}

		[HttpPut("closeSickLeave")]
		[Authorize]
		public IActionResult CloseSickLeave([FromBody] SickLeave sickLeave)
		{
			try
			{
				_sickLeaveService.CloseSickLeave(sickLeave);
				_emailService.SendSickLeaveClosedMail();
				return Ok();
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("getMedicalCertificates/{sickLeaveId:int}")]
		[Authorize]
		public IActionResult GetMedicalCertificates(int sickLeaveId)
		{
			try
			{
				return Ok(_sickLeaveRepository.GetMedicalCertificates(sickLeaveId));
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost("addSickLeave")]
		[Authorize]
		public IActionResult AddSickLeave(SickLeave sickLeave)
		{
			try
			{
				_sickLeaveService.AddSickLeave(sickLeave);
				return Ok();
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpPost("uploadMedicalCertificate")]
		[Authorize]
		public IActionResult UploadMedicalCertificate([FromBody] MedicalCertificate medicalCertificate)
		{
			try
			{
				_sickLeaveRepository.UploadMedicalCertificateFile(medicalCertificate);
			return Ok();
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("getMedicalCertificate/{medicalCertificateId:int}")]
		[Authorize]
		public IActionResult GetMedicalCertificate(int medicalCertificateId)
		{
			try
			{
				return Ok(_sickLeaveRepository.GetMedicalCertificate(medicalCertificateId));
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost("addMedCertViaScheduler")]
		[AllowAnonymous]
		public IActionResult AddMedicalCertificateViaScheduler([FromBody] DateTime date)
		{
            try
            {
				_sickLeaveService.AddMedCertForEveryOpenedSickLeave(date);
                return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

		[HttpGet("getArchivedSickLeavesByUserId/{userId:int}")]
		[Authorize]
		public IActionResult GetArchivedSickLeaves(int userId)
		{
			try
			{
				return Ok(_sickLeaveService.GetArchivedSickLeavesByUserId(userId));
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("getClosedSickLeaves")]
		[Authorize]
		public IActionResult GetClosedSickLeaves()
		{
			try
			{
				return Ok(_sickLeaveService.GetClosedSickLeaves());
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("getSickLeaves")]
		[Authorize]
		public IActionResult GetSickLeaves()
		{
			try
			{
				return Ok(_sickLeaveRepository.GetSickLeaves());
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPut("archiveSickLeave")]
		[Authorize]
		public IActionResult ArchiveSickLeave([FromBody] SickLeave sickLeave)
		{
			try
			{
				_sickLeaveService.ArchiveSickLeave(sickLeave);
				return Ok();
			}
			catch (System.Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
