using System;

namespace DMAWS_T2305M_PhamDangTung.Models
{
    public class ProjectEmployee
    {
        public int EmployeeId { get; set; } // Id nhân viên
        public int ProjectId { get; set; } // Id dự án
        public string Tasks { get; set; } // Nhiệm vụ trong dự án
        public virtual Employee Employees { get; set; } // Nhân viên
        public virtual Project Projects { get; set; } // Dự án
    }
}
