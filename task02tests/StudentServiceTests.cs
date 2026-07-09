using task02;

namespace task02tests
{
    public class StudentServiceTests
    {
        private readonly List<Student> _testStudents;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _testStudents = new List<Student>
            {
                new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
                new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
                new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
            };
            _service = new StudentService(_testStudents);
        }

        [Fact]
        public void GetStudentsByFaculty_ReturnsCorrectStudents()
        {
            var result = _service.GetStudentsByFaculty("ФИТ").ToList();
            Assert.Equal(2, result.Count);
            Assert.True(result.All(s => s.Faculty == "ФИТ"));
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_ReturnsCorrectStudents()
        {
            var result = _service.GetStudentsWithMinAverageGrade(4).ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Иван");
            Assert.Contains(result, s => s.Name == "Петр");
        }

        [Fact]
        public void GetStudentsOrderedByName_ReturnsSortedStudents()
        {
            var result = _service.GetStudentsOrderedByName().ToList();
            Assert.Equal("Анна", result[0].Name);
            Assert.Equal("Иван", result[1].Name);
            Assert.Equal("Петр", result[2].Name);
        }

        [Fact]
        public void GroupStudentsByFaculty_GroupsCorrectly()
        {
            var result = _service.GroupStudentsByFaculty();

            Assert.Equal(2, result["ФИТ"].Count());
            Assert.Single(result["Экономика"]);
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
        {
            var result = _service.GetFacultyWithHighestAverageGrade();
            Assert.Equal("Экономика", result);
        }

    }
}
