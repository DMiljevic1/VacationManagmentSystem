﻿using DomainModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class Vacation
    {
        public int Id { get; set; }
        public DateTime VacationFrom { get; set; }
        public DateTime VacationTo { get; set; }
        public VacationStatus Status { get; set; }
        public int VacationSpent { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime VacationRequestDate { get; set; }
    }
}
