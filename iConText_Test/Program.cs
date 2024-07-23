using System.Globalization;
using Newtonsoft.Json;

namespace EmployeeManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            ProcessArguments(args);
        }

        public static void ProcessArguments(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments provided");
                return;
            }

            string filePath = "employees.json";

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            var employees = JsonConvert.DeserializeObject<List<Employee>>(File.ReadAllText(filePath));

            switch (args[0].ToLower())
            {
                case "-add":
                    AddEmployee(args.Skip(1).ToArray(), employees, filePath);
                    break;
                case "-update":
                    UpdateEmployee(args.Skip(1).ToArray(), employees, filePath);
                    break;
                case "-get":
                    GetEmployee(args.Skip(1).ToArray(), employees);
                    break;
                case "-delete":
                    DeleteEmployee(args.Skip(1).ToArray(), employees, filePath);
                    break;
                case "-getall":
                    GetAllEmployees(employees);
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }

        public static void AddEmployee(string[] args, List<Employee> employees, string filePath)
        {
            var employee = new Employee();
            var properties = new Dictionary<string, Action<string>>
            {
                { "firstname", value => employee.FirstName = value },
                { "lastname", value => employee.LastName = value },
                { "salary", value =>
                    {
                        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary))
                        {
                            employee.SalaryPerHour = salary;
                        }
                        else
                        {
                            throw new FormatException($"Invalid salary format: {value}");
                        }
                    }
                }
            };

            foreach (var arg in args)
            {
                var parts = arg.Split(':');
                if (parts.Length == 2 && properties.ContainsKey(parts[0].ToLower()))
                {
                    try
                    {
                        properties[parts[0].ToLower()](parts[1]);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid argument format: {arg}");
                    return;
                }
            }

            employee.Id = employees.Count > 0 ? employees.Max(e => e.Id) + 1 : 1;
            employees.Add(employee);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(employees, Formatting.Indented));
            Console.WriteLine("Employee added successfully");
        }

        public static void UpdateEmployee(string[] args, List<Employee> employees, string filePath)
        {
            if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out int id))
            {
                Console.WriteLine("Invalid or missing Id");
                return;
            }

            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                Console.WriteLine($"Employee with Id {id} not found");
                return;
            }

            var properties = new Dictionary<string, Action<string>>
            {
                { "firstname", value => employee.FirstName = value },
                { "lastname", value => employee.LastName = value },
                { "salary", value =>
                    {
                        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary))
                        {
                            employee.SalaryPerHour = salary;
                        }
                        else
                        {
                            throw new FormatException($"Invalid salary format: {value}");
                        }
                    }
                }
            };

            foreach (var arg in args)
            {
                var parts = arg.Split(':');
                if (parts.Length == 2 && properties.ContainsKey(parts[0].ToLower()))
                {
                    try
                    {
                        properties[parts[0].ToLower()](parts[1]);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid argument format: {arg}");
                    return;
                }
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(employees, Formatting.Indented));
            Console.WriteLine("Employee updated successfully");
        }

        public static void GetEmployee(string[] args, List<Employee> employees)
        {
            if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out int id))
            {
                Console.WriteLine("Invalid or missing Id");
                return;
            }

            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                Console.WriteLine($"Employee with Id {id} not found");
                return;
            }

            Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
        }

        public static void DeleteEmployee(string[] args, List<Employee> employees, string filePath)
        {
            if (!int.TryParse(args.FirstOrDefault(a => a.StartsWith("Id:"))?.Split(':')[1], out int id))
            {
                Console.WriteLine("Invalid or missing Id");
                return;
            }

            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                Console.WriteLine($"Employee with Id {id} not found");
                return;
            }

            employees.Remove(employee);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(employees, Formatting.Indented));
            Console.WriteLine("Employee deleted successfully");
        }

        public static void GetAllEmployees(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
            }
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal SalaryPerHour { get; set; }
    }
}
