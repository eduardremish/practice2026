using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public static class StudentService
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string SerializeStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            return JsonSerializer.Serialize(student, Options);
        }

        public static Student DeserializeStudent(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, Options);
            student?.Valid();
            return student ?? throw new JsonException("Ошибка.");
        }
        public static Student LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            return DeserializeStudent(File.ReadAllText(filePath));
        }
        public static void SaveFile(string filePath, Student student)
        {
            File.WriteAllText(filePath, SerializeStudent(student));
        }

        
    }
}
