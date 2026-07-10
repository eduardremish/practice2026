using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task11
{
    public static class ClassGenerator
    {
        public static ICalculator CreateCalculator(string sourceCode)
        {

            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentNullException(nameof(sourceCode), "Исходный код не может быть пустым");


            string preparedCode = PrepareSourceCode(sourceCode);


            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(preparedCode);


            MetadataReference[] assemblyReferences = GetAssemblyReferences();


            CSharpCompilation compilation = CreateCompilation(syntaxTree, assemblyReferences);


            return CompileAndCreateInstance(compilation);
        }

        private static string PrepareSourceCode(string sourceCode)
        {

            return sourceCode.Replace(
                "public class Calculator",
                "public class Calculator : task11.ICalculator"
            );
        }

        private static MetadataReference[] GetAssemblyReferences()
        {
            return new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.CoreLib")).Location),
            };
        }

        private static CSharpCompilation CreateCompilation(SyntaxTree syntaxTree, MetadataReference[] references)
        {
            string assemblyName = $"DynamicAssembly_{Guid.NewGuid():N}";

            return CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
        }

        private static ICalculator CompileAndCreateInstance(CSharpCompilation compilation)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {

                var compileResult = compilation.Emit(memoryStream);


                if (!compileResult.Success)
                {
                    string errorMessage = GetCompilationErrors(compileResult);
                    throw new InvalidOperationException($"Ошибка компиляции: {errorMessage}");
                }


                memoryStream.Seek(0, SeekOrigin.Begin);
                byte[] assemblyBytes = memoryStream.ToArray();
                Assembly compiledAssembly = Assembly.Load(assemblyBytes);


                return CreateCalculatorInstance(compiledAssembly);
            }
        }

        private static string GetCompilationErrors(EmitResult compileResult)
        {
            var errors = compileResult.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.GetMessage())
                .ToList();

            return errors.Count > 0 ? string.Join("; ", errors) : "Неизвестная ошибка";
        }

        private static ICalculator CreateCalculatorInstance(Assembly assembly)
        {
            Type calculatorType = assembly.GetType("Calculator");

            if (calculatorType == null)
                throw new InvalidOperationException("В сборке не найден класс Calculator");

            object instance = Activator.CreateInstance(calculatorType);

            if (instance == null)
                throw new InvalidOperationException("Не удалось создать экземпляр класса Calculator");

            if (instance is ICalculator calculator)
                return calculator;

            throw new InvalidOperationException("Класс Calculator не реализует интерфейс ICalculator");
        }
    }
}
