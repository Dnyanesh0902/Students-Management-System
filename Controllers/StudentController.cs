using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Management_API.Data;
using Student_Management_API.DTOs;
using Student_Management_API.Models;
using Student_Management_API.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Student_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        //get All Students
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _studentRepository.GetAllStudents();
            var studentDtos = students.Select(s => new StudentDto
            {
                Name = s.Name,
                Email = s.Email,
                Course = s.Course,
            });
            return Ok(studentDtos);
        }
        //Get student by id
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentRepository.GetStudentById(id);
            if (student == null)
                return NotFound();
            var studentDto = new StudentDto
            {
                Name = student.Name,
                Email = student.Email,
                Course = student.Course,
            };
            return Ok(studentDto);
        }
        //Post Student
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(StudentDto studentDto)
        {
            var student = new Student
            {
                Name = studentDto.Name,
                Email = studentDto.Email,
                Course = studentDto.Course,
            };
            await _studentRepository.AddStudent(student);
            return Ok(student);
        }
        //put update student
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentDto studentDto)
        {
            var student = await _studentRepository.GetStudentById(id);

            if (student == null)
                return NotFound();
            student.Name = studentDto.Name;
            student.Email = studentDto.Email;
            student.Course = studentDto.Course;

            await _studentRepository.UpdateStudent(student);
            return Ok(student);

        }
        //Delete Student
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentRepository.DeleteStudent(id);
           if(!result)
                return NotFound();
            return Ok("Student Deleted SuccessFully");
        }
    }
}
