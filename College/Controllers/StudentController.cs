using College.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDto>> GetStudent()
        {
            //var students = new List<StudentDto>();
            //foreach (var item in CollegeRepository.Students)
            //{
            //    StudentDto obj = new StudentDto()
            //    {
            //        Id = item.Id,
            //        StudentName = item.StudentName,
            //        Address = item.Address,
            //        Email = item.Email
            //    };
            //    students.Add(obj);
            //}

            var students = CollegeRepository.Students.Select(s => new StudentDto()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Address = s.Address,
                Email = s.Email
            });

            //Ok - 200
            return Ok(students);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> GetStudentById(int id)
        {
            if (id <= 0)
                return BadRequest();

            var student = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();
            if (student == null)
                return NotFound($"Student with this id {id} not found");

            var studentDto = new StudentDto
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };

            return Ok(studentDto);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var student = CollegeRepository.Students.Where(s => s.StudentName == name).FirstOrDefault();
            if (student == null)
                return NotFound($"Student with this name {name} not found");

            var studentDto = new Student
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };

            return Ok(studentDto);
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> CreateStudent([FromBody] StudentDto model)
        {
            if (model == null)
                return BadRequest("");
            int newId = CollegeRepository.Students.LastOrDefault().Id + 1;
            Student student = new Student
            {
                Id = newId,
                StudentName = model.StudentName,
                Email = model.Email,
                Address = model.Address
            };
            CollegeRepository.Students.Add(student);
            model.Id = student.Id;
            return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> UpdateStudent([FromBody] StudentDto model)
        {
            if (model == null || model.Id <= 0)
                BadRequest();

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == model.Id).FirstOrDefault();

            if (existingStudent == null)
                return NotFound();

            existingStudent.StudentName = model.StudentName;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;

            return NoContent();

        }

        [HttpPut]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        {
            if (patchDocument == null || id <= 0)
                BadRequest();

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();

            if (existingStudent == null)
                return NotFound();

            var studentDto = new StudentDto
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Email = existingStudent.Email,
                Address = existingStudent.Address,
            };

            patchDocument.ApplyTo(studentDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent.StudentName = studentDto.StudentName;
            existingStudent.Email = studentDto.Email;
            existingStudent.Address = studentDto.Address;

            return NoContent();

        }

        [HttpDelete]
        [Route("Delete/{id:int}", Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteStudent(int id)
        {
            if (id <= 0)
                return BadRequest();

            var student = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();
            if (student == null)
                return NotFound($"Student with this id {id} not found");

            CollegeRepository.Students.Remove(student);
            return Ok(true);
        }
    }
}
