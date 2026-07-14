using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests
{
    public class StudentTests
    {
        [Fact]
        public void Deserialize_WhiteSpaceName_ShouldThrowArgumentException()
        {

            string json = "{\"FirstName\": \"   \", \"BirthDate\": \"2004-05-10\"}";

            var ex = Assert.Throws<ArgumentException>(() => StudentService.DeserializeStudent(json));
            Assert.Contains("Ошибка", ex.Message);
        }

        [Fact]
        public void Deserialize_MultipleGrades_ShouldSumCorrectly()
        {
            string json = @"{
        ""FirstName"": ""Анна"",
        ""BirthDate"": ""2003-09-01"",
        ""Grades"": [
            { ""Name"": ""Математика"", ""Grade"": 85 },
            { ""Name"": ""Физика"", ""Grade"": 90 },
            { ""Name"": ""Химия"", ""Grade"": 78 }
        ]
    }";


            Student student = StudentService.DeserializeStudent(json);

            Assert.Equal("Анна", student.FirstName);
            Assert.Equal(3, student.Grades.Count);
            Assert.Equal(85, student.Grades[0].Grade);
            Assert.Equal(90, student.Grades[1].Grade);
            Assert.Equal(78, student.Grades[2].Grade);
        }

        [Fact]
        public void Deserialize_NegativeGrade_ShouldThrowArgumentOutOfRangeException()
        {
            string json = @"{
        ""FirstName"": ""Пётр"",
        ""BirthDate"": ""2002-12-01"",
        ""Grades"": [
            { ""Name"": ""История"", ""Grade"": -10 }
        ]
    }";

            Assert.Throws<ArgumentOutOfRangeException>(() => StudentService.DeserializeStudent(json));
        }
    }
}
