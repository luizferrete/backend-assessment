﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.DL.Services.DAL
{
    public interface IEntity
    {
        long Id { get; set; }

        bool FlagActive { get; set; }
    }
}
