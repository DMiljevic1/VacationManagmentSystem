﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.DtoModels
{
    public class EmployeeHistory
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime VacationFrom { get; set; }
        public DateTime VacationTo { get; set; }
        public int VacationSpent { get; set; }
        public DateTime VacationRequestDate { get; set; }
        public string ApprovedBy { get; set; }
    }
}
