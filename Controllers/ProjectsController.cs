using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DMAWS_T2305M_PhamDangTung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMAWS_T2305M_PhamDangTung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employees)
                .ToListAsync();
        }

        // GET: api/projects/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Project>>> SearchProjects(string projectName, bool? isInProgress)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                query = query.Where(p => p.ProjectName.Contains(projectName));
            }

            if (isInProgress.HasValue)
            {
                if (isInProgress.Value)
                {
                    query = query.Where(p => p.ProjectEndDate == null || p.ProjectEndDate > DateTime.Now);
                }
                else
                {
                    query = query.Where(p => p.ProjectEndDate != null && p.ProjectEndDate < DateTime.Now);
                }
            }

            return await query.ToListAsync();
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employees)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/projects
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project == null)
            {
                return BadRequest("Project data is null.");
            }

            // Kiểm tra nếu có dữ liệu ProjectEmployees
            if (project.ProjectEmployees != null && project.ProjectEmployees.Any())
            {
                foreach (var projectEmployee in project.ProjectEmployees)
                {
                    // Kiểm tra xem Employee có tồn tại không
                    var employee = await _context.Employees.FindAsync(projectEmployee.EmployeeId);

                    if (employee == null)
                    {
                        // Nếu nhân viên không tồn tại, tạo mới nhân viên
                        var newEmployee = new Employee
                        {
                            EmployeeName = projectEmployee.Employees.EmployeeName,
                            EmployeeDOB = projectEmployee.Employees.EmployeeDOB,
                            EmployeeDepartment = projectEmployee.Employees.EmployeeDepartment
                        };

                        _context.Employees.Add(newEmployee);
                        await _context.SaveChangesAsync();

                        // Liên kết ProjectEmployee với nhân viên mới
                        projectEmployee.EmployeeId = newEmployee.EmployeeId;
                        projectEmployee.Employees = newEmployee;
                    }
                    else
                    {
                        // Nếu nhân viên đã tồn tại, liên kết ProjectEmployee với nhân viên đó
                        projectEmployee.Employees = employee;
                    }

                    // Liên kết ProjectEmployee với dự án
                    projectEmployee.Projects = project;
                }
            }

            // Thêm dự án vào cơ sở dữ liệu
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Trả về kết quả với ID của dự án vừa được thêm
            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/projects/{id}/employees
        [HttpGet("{id}/employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetProjectEmployees(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectEmployees)
                .ThenInclude(pe => pe.Employees)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            var employees = project.ProjectEmployees.Select(pe => pe.Employees).ToList();
            return employees;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
