using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace task13
{
    public class Student
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate { get; set; }

        public List<Subject>? Grades { get; set; }

        public void Valid()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("Ошибка. Пустое значение");

            if (Grades != null)
                foreach (var subject in Grades)
                {
                    if (string.IsNullOrWhiteSpace(subject.Name))
                        throw new ArgumentException("Ошибка. Пустое значение");

                    if (subject.Grade < 0 || subject.Grade > 100)
                        throw new ArgumentOutOfRangeException("Баллы вне диапазона");
                }

        }
    }
}
