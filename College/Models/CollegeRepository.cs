namespace College.Models
{
    public class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>()
        {
            new Student
            {
                Id = 1,
                StudentName = "Student 1",
                Email = "Henry@gmail.com",
                Address = "henry steeet"
            },
            new Student
            {
                Id = 2,
                StudentName = "Student 2",
                Email = "Henry2@gmail.com",
                Address = "henry avenue"
            }
        };
            
    }
}
