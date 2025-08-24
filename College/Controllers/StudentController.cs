using AutoMapper;
using College.Data;
using College.Data.Repository;
using College.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        private readonly ICollegeRepository<Student> _studentRepository;

        public StudentController(ILogger<StudentController> logger, IMapper mapper, ICollegeRepository<Student> studentRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudent()
        {
            _logger.LogInformation("GetStudent method called");

            var students = await _studentRepository.GetAllAsync();

            var studentDtoData = _mapper.Map<List<StudentDto>>(students);

            //Ok - 200
            return Ok(studentDtoData);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            _logger.LogInformation("GetStudentById method called with id: {Id}", id);
            if (id <= 0)
                return BadRequest();

            var student = await _studentRepository.GetByIdAsync(student => student.Id == id);
            if (student == null)
                return NotFound($"Student with this id {id} not found");

            var studentDto = _mapper.Map<StudentDto>(student);

            return Ok(studentDto);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> GetStudentByName(string name)
        {
            _logger.LogInformation("GetStudentByName method called with name: {Name}", name);
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var student = await _studentRepository.GetByNameAsync(student => student.StudentName.ToLower().Contains(name));
            if (student == null)
                return NotFound($"Student with this name {name} not found");

            var studentDto = _mapper.Map<StudentDto>(student);

            return Ok(studentDto);
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] StudentDto model)
        {
            _logger.LogInformation("CreateStudent method called with model: {@Model}", model);
            if (model == null)
                return BadRequest();
            Student student = _mapper.Map<Student>(model);
            var studentAfterCreation = await _studentRepository.CreateAsync(student);
            model.Id = studentAfterCreation.Id;
            return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> UpdateStudentAsync([FromBody] StudentDto dto)
        {
            _logger.LogInformation("UpdateStudent method called with model: {@Model}", dto);
            if (dto == null || dto.Id <= 0)
                BadRequest();

            var existingStudent = await _studentRepository.GetByIdAsync(student => student.Id == dto.Id, true);

            if (existingStudent == null)
                return NotFound();

            var newRecord = _mapper.Map<Student>(dto);

            await _studentRepository.UpdateAsync(newRecord);

            return NoContent();

        }

        [HttpPut]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateStudentPartialAsync(int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        {
            _logger.LogInformation("UpdateStudentPartial method called with id: {Id} and patchDocument: {@PatchDocument}", id, patchDocument);
            if (patchDocument == null || id <= 0)
                BadRequest();

            var existingStudent = await _studentRepository.GetByIdAsync(student => student.Id == id, true);

            if (existingStudent == null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDto>(existingStudent);

            patchDocument.ApplyTo(studentDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent = _mapper.Map<Student>(studentDto);

            await _studentRepository.UpdateAsync(existingStudent);

            return NoContent();

        }

        [HttpDelete]
        [Route("Delete/{id:int}", Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("DeleteStudent method called with id: {Id}", id);
            if (id <= 0)
                return BadRequest();

            var student = await _studentRepository.GetByIdAsync(student => student.Id == id);
            if (student == null)
                return NotFound($"Student with this id {id} not found");

            await _studentRepository.DeleteAsync(student);

            return Ok(true);
        }
    }
}
