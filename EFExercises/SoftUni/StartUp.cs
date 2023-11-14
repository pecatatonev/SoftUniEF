using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();


            Console.WriteLine(GetLatestProjects(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                }).ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));
            return result;
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));
            return result;
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context) 
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .Where(e => e.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));
            return result;
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var adreess = new Address { AddressText = "Vitoshka 15", TownId = 4 };
            var employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employee.Address = adreess;
            context.SaveChanges();

            var employees = context.Employees.Select(e => new
            {
                e.Address.AddressText
                ,e.Address.AddressId
            })
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));
            return result;
        }

        //doesnt workpublic static string GetEmployeesInPeriod(SoftUniContext context)
        //{
        //    var employees = context.Employees.Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
        //        .Select(e => new
        //        {
        //            FirstName = e.FirstName,
        //            LastName = e.LastName,
        //            ManagerFirstName = e.Manager.FirstName,
        //            ManagerLastName = e.Manager.LastName,
        //            Projects = e.EmployeesProjects.Select(ep => new
        //            {
        //                ProjectName = ep.Project.Name,
        //                ProjectStartDate = ep.Project.StartDate,
        //                ProjectEndDate = ep.Project.EndDate
        //            }).ToList()
        //        }).Take(10).ToList();

        //    StringBuilder employeeManagerResult = new StringBuilder();

        //    foreach (var employee in employees)
        //    {
        //        employeeManagerResult.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

        //        foreach (var project in employee.Projects)
        //        {
        //            var startDate = project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt");
        //            var endDate = project.ProjectEndDate.HasValue ? project.ProjectEndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

        //            employeeManagerResult.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
        //        }
        //    }
        //    return employeeManagerResult.ToString().TrimEnd();
        //}

        public static string GetAddressesByTown(SoftUniContext context) {
            var addreses = context.Addresses.Select(a => new
            {
                address = a.AddressText,
                townName = a.Town.Name,
                employeesCount = a.Employees.Count
            })
                .OrderByDescending(a => a.employeesCount)
                .ThenBy(t => t.townName)
                .ThenBy(a => a.address)
                .Take(10);

            string result = string.Join(Environment.NewLine, addreses.Select(a => $"{a.address}, {a.townName} - {a.employeesCount} employees"));
                return result;
        }

        //doesnt work public static string GetEmployee147(SoftUniContext context)
        //{
        //    var employee = context.Employees
        //        .Where(e => e.EmployeeId == 147)
        //        .Select(e => new
        //        {
        //            e.FirstName,
        //            e.LastName,
        //            e.JobTitle,
        //            projects = e.EmployeesProjects
        //            .Select(p => p.Project.Name)
        //        }).ToList();
        //    StringBuilder result = new StringBuilder();

        //    foreach (var e in employee)
        //    {
        //        result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
        //        result.AppendLine($"{e.projects}");

        //    }
        //    return result.ToString().Trim();
        //}

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context) {
            var departments = context.Departments.Where(e => e.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(e => e.Name)
                .Select(d => new
                {
                    departmentName = d.Name,
                    managerFirstName = d.Manager.FirstName,
                    managerLastName = d.Manager.LastName,
                    departmentEmployees = d.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    }).OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName).ToList()
                }).ToList();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var department in departments)
            {
                stringBuilder.AppendLine($"{department.departmentName} - {department.managerFirstName} {department.managerLastName}");
                foreach (var employees in department.departmentEmployees) 
                {
                    stringBuilder.AppendLine($"{employees.FirstName} {employees.LastName} - {employees.JobTitle}");
                }
            }
            return stringBuilder.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context) {

            StringBuilder result = new StringBuilder();

            var projects = context.Projects.OrderByDescending(p => p.StartDate).Take(10).Select(s => new
            {
                ProjectName = s.Name,
                ProjectDescription = s.Description,
                ProjectStartDate = s.StartDate
            }).OrderBy(n => n.ProjectName).ToArray();

            foreach (var p in projects)
            {
                var startDate = p.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt");
                result.AppendLine($"{p.ProjectName}");
                result.AppendLine($"{p.ProjectDescription}");
                result.AppendLine($"{startDate}");
            }
            return result.ToString().TrimEnd();
        }
    }
    
}
