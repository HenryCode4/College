
using Microsoft.EntityFrameworkCore;

namespace College.Data.Repository
{
    public class StudentRepository : CollegeRepository<Student>, IStudentRepository
    {
        private readonly CollegeDBContext _dbContext;
        public StudentRepository(CollegeDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Student>> GetStudentsByFeeStatusAsync(int feesStatus)
        {
            //Write code to return student having fee status pending
            return null;
        }
    }
}
