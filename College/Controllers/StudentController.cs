using AutoMapper;
using College.Data;
using College.Data.Repository;
using College.Models;
using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(PolicyName = "AllowOnlyLocalhost")]
    [Authorize(AuthenticationSchemes = "LoginForLocalUSers", Roles = "Superadmin, Admin")]
    //[AllowAnonymous]
    public class StudentController : ControllerBase
    {

        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private APIResponse _apiResponse;

        public StudentController(ILogger<StudentController> logger, IMapper mapper, IStudentRepository studentRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _apiResponse = new();
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetStudent()
        {
            try
            {
                _logger.LogInformation("GetStudent method called");

                var students = await _studentRepository.GetAllAsync();

                _apiResponse.Data = _mapper.Map<List<StudentDto>>(students);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                //Ok - 200
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStudent method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }

        }

        [HttpGet]
        [Route("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetStudentById(int id)
        {
            try
            {
                _logger.LogInformation("GetStudentById method called with id: {Id}", id);
                if (id <= 0)
                    return BadRequest();

                var student = await _studentRepository.GetAsync(student => student.Id == id);
                if (student == null)
                    return NotFound($"Student with this id {id} not found");

                _apiResponse.Data = _mapper.Map<StudentDto>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStudentById method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetStudentByName(string name)
        {
            try
            {
                _logger.LogInformation("GetStudentByName method called with name: {Name}", name);
                if (string.IsNullOrEmpty(name))
                    return BadRequest();

                var student = await _studentRepository.GetAsync(student => student.StudentName.ToLower().Contains(name));
                if (student == null)
                    return NotFound($"Student with this name {name} not found");

                _apiResponse.Data = _mapper.Map<StudentDto>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStudentByName method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateStudent([FromBody] StudentDto dto)
        {
            try
            {
                 _logger.LogInformation("CreateStudent method called with model: {@Model}", dto);
                if (dto == null)
                    return BadRequest();
                Student student = _mapper.Map<Student>(dto);
                var studentAfterCreation = await _studentRepository.CreateAsync(student);
                dto.Id = studentAfterCreation.Id;

                _apiResponse.Data = dto;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return CreatedAtRoute("GetStudentById", new { id = dto.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateStudent method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
           
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateStudentAsync([FromBody] StudentDto dto)
        {
            try
            {
                _logger.LogInformation("UpdateStudent method called with model: {@Model}", dto);
                if (dto == null || dto.Id <= 0)
                    BadRequest();

                var existingStudent = await _studentRepository.GetAsync(student => student.Id == dto.Id, true);

                if (existingStudent == null)
                    return NotFound();

                var newRecord = _mapper.Map<Student>(dto);

                await _studentRepository.UpdateAsync(newRecord);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudent method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            

        }

        [HttpPut]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateStudentPartialAsync(int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument)
        {
            try
            {
                _logger.LogInformation("UpdateStudentPartial method called with id: {Id} and patchDocument: {@PatchDocument}", id, patchDocument);
                if (patchDocument == null || id <= 0)
                    BadRequest();

                var existingStudent = await _studentRepository.GetAsync(student => student.Id == id, true);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentPartial method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
           

        }

        [HttpDelete]
        [Route("Delete/{id:int}", Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteStudentAsync(int id)
        {
            try
            {
                _logger.LogInformation("DeleteStudent method called with id: {Id}", id);
                if (id <= 0)
                    return BadRequest();

                var student = await _studentRepository.GetAsync(student => student.Id == id);
                if (student == null)
                    return NotFound($"Student with this id {id} not found");

                await _studentRepository.DeleteAsync(student);

                _apiResponse.Data = true;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteStudent method");
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
            
        }
    }
}
