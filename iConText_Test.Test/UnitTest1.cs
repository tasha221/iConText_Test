using Newtonsoft.Json;
using EmployeeManager; 

namespace EmployeeManager.Tests
{
    [TestFixture]
    public class EmployeeManagerTests
    {
        private const string FilePath = "employees.json";

        [SetUp]
        public void Setup()
        {
            var initialEmployees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Alice", LastName = "Smith", SalaryPerHour = 50 },
                new Employee { Id = 2, FirstName = "Bob", LastName = "Johnson", SalaryPerHour = 60 }
            };

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(initialEmployees, Formatting.Indented));
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        [Test]
        public void TestAddEmployee()
        {
            var args = new[] { "-add", "FirstName:John", "LastName:Doe", "Salary:100.5" };

            Program.ProcessArguments(args);

            var employees = JsonConvert.DeserializeObject<List<Employee>>(File.ReadAllText(FilePath));

            Assert.AreEqual(3, employees.Count);
            Assert.AreEqual("John", employees[2].FirstName);
            Assert.AreEqual("Doe", employees[2].LastName);
            Assert.AreEqual(100.5, employees[2].SalaryPerHour);
        }
    }
}
