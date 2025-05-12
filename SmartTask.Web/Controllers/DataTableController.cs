using Microsoft.AspNetCore.Mvc;
using SmartTask.Web.Models;
using System.Globalization;

namespace SmartTask.Web.Controllers
{
    public class DataTableController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //GetUsers method is returning the IEnumerable employee from database
        [HttpGet]
        IEnumerable<UserDataTable> GetUsers()
        {
            return new List<UserDataTable>
            {
                new UserDataTable
        {
            Name = "Ahmed Ali",
            Position = "Software Engineer",
            Location = "Cairo",
            Age = 28,
            StartDate = DateTime.Now.AddDays(-1),
            StartDateString = DateTime.Now.AddDays(-1).ToString("dd'/'MM'/'yyyy"),
            Salary = 45000
        },
        new UserDataTable
        {
            Name = "Mona Hassan",
            Position = "Data Scientist",
            Location = "Alexandria",
            Age = 32,
            StartDate = DateTime.Now.AddDays(-2),
            StartDateString = DateTime.Now.AddDays(-2).ToString("dd'/'MM'/'yyyy"),
            Salary = 50000
        },
        new UserDataTable
        {
            Name = "Mohamed Fathy",
            Position = "Backend Developer",
            Location = "Giza",
            Age = 27,
            StartDate = DateTime.Now.AddDays(-3),
            StartDateString = DateTime.Now.AddDays(-3).ToString("dd'/'MM'/'yyyy"),
            Salary = 48000
        },
        new UserDataTable
        {
            Name = "Fatima Ibrahim",
            Position = "Project Manager",
            Location = "Cairo",
            Age = 35,
            StartDate = DateTime.Now.AddDays(-4),
            StartDateString = DateTime.Now.AddDays(-4).ToString("dd'/'MM'/'yyyy"),
            Salary = 60000
        },
        new UserDataTable
        {
            Name = "Youssef Tarek",
            Position = "UI/UX Designer",
            Location = "Tanta",
            Age = 30,
            StartDate = DateTime.Now.AddDays(-5),
            StartDateString = DateTime.Now.AddDays(-5).ToString("dd'/'MM'/'yyyy"),
            Salary = 55000
        },
        new UserDataTable
        {
            Name = "Sara Khaled",
            Position = "Full Stack Developer",
            Location = "Aswan",
            Age = 29,
            StartDate = DateTime.Now.AddDays(-6),
            StartDateString = DateTime.Now.AddDays(-6).ToString("dd'/'MM'/'yyyy"),
            Salary = 47000
        },
        new UserDataTable
        {
            Name = "Omar Mostafa",
            Position = "Security Analyst",
            Location = "Mansoura",
            Age = 33,
            StartDate = DateTime.Now.AddDays(-7),
            StartDateString = DateTime.Now.AddDays(-7).ToString("dd'/'MM'/'yyyy"),
            Salary = 53000
        },
        new UserDataTable
        {
            Name = "Nourhan Adel",
            Position = "Quality Assurance",
            Location = "Sharm El Sheikh",
            Age = 26,
            StartDate = DateTime.Now.AddDays(-8),
            StartDateString = DateTime.Now.AddDays(-8).ToString("dd'/'MM'/'yyyy"),
            Salary = 46000
        },
        new UserDataTable
        {
            Name = "Khaled Samir",
            Position = "DevOps Engineer",
            Location = "Port Said",
            Age = 34,
            StartDate = DateTime.Now.AddDays(-9),
            StartDateString = DateTime.Now.AddDays(-9).ToString("dd'/'MM'/'yyyy"),
            Salary = 52000
        },
        new UserDataTable
        {
            Name = "Salma Ramadan",
            Position = "Database Administrator",
            Location = "Marsa Matruh",
            Age = 31,
            StartDate = DateTime.Now.AddDays(-10),
            StartDateString = DateTime.Now.AddDays(-10).ToString("dd'/'MM'/'yyyy"),
            Salary = 59000
        },
        new UserDataTable
        {
            Name = "Ali Mahmoud",
            Position = "Network Engineer",
            Location = "Suez",
            Age = 26,
            StartDate = DateTime.Now.AddDays(-11),
            StartDateString = DateTime.Now.AddDays(-11).ToString("dd'/'MM'/'yyyy"),
            Salary = 47000
        },
        new UserDataTable
        {
            Name = "Laila ElKady",
            Position = "Software Tester",
            Location = "Sharm El Sheikh",
            Age = 29,
            StartDate = DateTime.Now.AddDays(-12),
            StartDateString = DateTime.Now.AddDays(-12).ToString("dd'/'MM'/'yyyy"),
            Salary = 48000
        },
        new UserDataTable
        {
            Name = "Amr Tamer",
            Position = "Business Analyst",
            Location = "Giza",
            Age = 38,
            StartDate = DateTime.Now.AddDays(-13),
            StartDateString = DateTime.Now.AddDays(-13).ToString("dd'/'MM'/'yyyy"),
            Salary = 56000
        },
        new UserDataTable
        {
            Name = "Hala Nasser",
            Position = "HR Manager",
            Location = "Cairo",
            Age = 40,
            StartDate = DateTime.Now.AddDays(-14),
            StartDateString = DateTime.Now.AddDays(-14).ToString("dd'/'MM'/'yyyy"),
            Salary = 65000
        }, new UserDataTable
        {
            Name = "Marwa sayed",
            Position = "HR Manager",
            Location = "Cairo",
            Age = 40,
            StartDate = DateTime.Now.AddDays(-14),
            StartDateString = DateTime.Now.AddDays(-14).ToString("dd'/'MM'/'yyyy"),
            Salary = 65000
        },
        new UserDataTable
        {
            Name = "Fady Khalil",
            Position = "System Architect",
            Location = "Alexandria",
            Age = 41,
            StartDate = DateTime.Now.AddDays(-15),
            StartDateString = DateTime.Now.AddDays(-15).ToString("dd'/'MM'/'yyyy"),
            Salary = 70000
        }

            };
        }


        [HttpGet]
        public ActionResult GetData()
        {

            // Get request parameters from DataTables
            var draw = Request.Query["draw"].FirstOrDefault();
            var start = Request.Query["start"].FirstOrDefault();
            var length = Request.Query["length"].FirstOrDefault();
            var searchValue = Request.Query["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Query["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Query["order[0][dir]"].FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;




            // Advanced filter parameters
            var location = Request.Query["location"].FirstOrDefault();
            var position = Request.Query["position"].FirstOrDefault();
            var startDateStr = Request.Query["startDate"].FirstOrDefault();
            var endDateStr = Request.Query["endDate"].FirstOrDefault();
            var minSalaryStr = Request.Query["minSalary"].FirstOrDefault();
            var maxSalaryStr = Request.Query["maxSalary"].FirstOrDefault();
            var minAgeStr = Request.Query["minAge"].FirstOrDefault();
            var maxAgeStr = Request.Query["maxAge"].FirstOrDefault();


            DateTime? startDate = null;
            DateTime? endDate = null;
            int? minSalary = null;
            int? maxSalary = null;
            int? minAge = null;
            int? maxAge = null;

            if (!string.IsNullOrEmpty(startDateStr))
            {
                if (DateTime.TryParseExact(startDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate))
                {
                    startDate = parsedStartDate;
                }
            }

            if (!string.IsNullOrEmpty(endDateStr))
            {
                if (DateTime.TryParseExact(endDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate))
                {
                    endDate = parsedEndDate.AddDays(1).AddSeconds(-1); // End of day
                }
            }

            if (!string.IsNullOrEmpty(minSalaryStr) && int.TryParse(minSalaryStr, out int parsedMinSalary))
            {
                minSalary = parsedMinSalary;
            }

            if (!string.IsNullOrEmpty(maxSalaryStr) && int.TryParse(maxSalaryStr, out int parsedMaxSalary))
            {
                maxSalary = parsedMaxSalary;
            }

            if (!string.IsNullOrEmpty(minAgeStr) && int.TryParse(minAgeStr, out int parsedMinAge))
            {
                minAge = parsedMinAge;
            }

            if (!string.IsNullOrEmpty(maxAgeStr) && int.TryParse(maxAgeStr, out int parsedMaxAge))
            {
                maxAge = parsedMaxAge;
            }

            var employees = GetUsers();
            int totalRecords = employees.Count();


            if (!string.IsNullOrEmpty(location))
            {
                employees = employees.Where(x => x.Location == location).ToList();
            }

            if (!string.IsNullOrEmpty(position))
            {
                employees = employees.Where(x => x.Position == position).ToList();
            }

            if (startDate.HasValue)
            {
                employees = employees.Where(x => x.StartDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                employees = employees.Where(x => x.StartDate <= endDate.Value).ToList();
            }

            if (minSalary.HasValue)
            {
                employees = employees.Where(x => x.Salary >= minSalary.Value).ToList();
            }

            if (maxSalary.HasValue)
            {
                employees = employees.Where(x => x.Salary <= maxSalary.Value).ToList();
            }

            if (minAge.HasValue)
            {
                employees = employees.Where(x => x.Age >= minAge.Value).ToList();
            }

            if (maxAge.HasValue)
            {
                employees = employees.Where(x => x.Age <= maxAge.Value).ToList();
            }



            //Filter(Search)
            if (!string.IsNullOrEmpty(searchValue))
            {
                employees = employees.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())
                                              || x.Position.ToLower().Contains(searchValue.ToLower())
                                              || x.Location.ToLower().Contains(searchValue.ToLower())
                                              || x.Salary.ToString().Contains(searchValue.ToLower())
                                              || x.Age.ToString().Contains(searchValue.ToLower())
                                              || x.StartDate.ToString("dd'/'MM'/'yyyy").ToLower().Contains(searchValue.ToLower()))
                    .ToList();
            }

            // Total records after filtering
            int totalRecordsFiltered = employees.Count();

            // Sorting
            if (!string.IsNullOrEmpty(sortColumnIndex) && !string.IsNullOrEmpty(sortDirection))
            {
                switch (Convert.ToInt32(sortColumnIndex))
                {
                    case 0:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.Name) : employees.OrderByDescending(e => e.Name);
                        break;
                    case 1:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.Position) : employees.OrderByDescending(e => e.Position);
                        break;
                    case 2:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.Location) : employees.OrderByDescending(e => e.Location);
                        break;
                    case 3:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.Age) : employees.OrderByDescending(e => e.Age);
                        break;
                    case 4:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.StartDate) : employees.OrderByDescending(e => e.StartDate);
                        break;
                    case 5:
                        employees = sortDirection == "asc" ? employees.OrderBy(e => e.Salary) : employees.OrderByDescending(e => e.Salary);
                        break;
                    default:
                        employees = employees.OrderBy(e => e.Name);
                        break;
                }
            }


            //Pagination 
            if (pageSize > 0)
            {
                employees = employees.Skip(skip).Take(pageSize);
            }



            // Format the data for output
            var result = employees.Select(emp => new
            {
                emp.Name,
                emp.Position,
                emp.Location,
                emp.Age,
                StartDateString = emp.StartDate.ToString("dd'/'MM'/'yyyy"),
                emp.Salary
            }).ToList();

            // Return JSON data for DataTable
            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecordsFiltered,
                data = result
            });









        }

        [HttpGet]
        public ActionResult GetLocations()
        {
            try
            {
                var locations = GetUsers().Select(u => u.Location).Distinct().OrderBy(l => l).ToList();
                return Json(locations);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetPositions()
        {
            try
            {
                var positions = GetUsers().Select(u => u.Position).Distinct().OrderBy(p => p).ToList();
                return Json(positions);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
