using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DMAWS_T2305M_PhamDangTung.Models; // Đảm bảo bạn đã tạo model Project
using System.Collections.Generic;
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
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects); // Trả về danh sách dự án
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy dự án
            }

            return Ok(project); // Trả về dự án
        }

        // POST: api/projects
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project); // Trả về 201 sau khi tạo thành công
        }

        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest(); // Trả về 400 nếu ID không khớp
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
                    return NotFound(); // Trả về 404 nếu không tìm thấy dự án
                }
                throw; // Ném lại ngoại lệ nếu có lỗi
            }

            return NoContent(); // Trả về 204 nếu cập nhật thành công
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy dự án
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về 204 sau khi xóa thành công
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id); // Kiểm tra xem dự án có tồn tại không
        }
    }
}
