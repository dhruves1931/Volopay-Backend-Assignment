using Microsoft.AspNetCore.Mvc;
using VolopayAssignment.Model;


namespace VolopayAssignment.Controllers
{
    public class SoftwarePurchaseController : Controller
    {
        private readonly List<Purchase> purchases = new List<Purchase>()
        {
            new Purchase { Id = 1, Date = DateTime.Parse("2022-10-23 18:50:10"), User = "Wil Wheaton", Department = "Marketing", Software = "Apple", Seats = 3, Amount = 60 },
            new Purchase { Id = 2, Date = DateTime.Parse("2022-11-17 16:37:10"), User = "Leslie Winkle", Department = "Tech", Software = "Outplay", Seats = 1, Amount = 12 },
            new Purchase { Id = 3, Date = DateTime.Parse("2022-05-14 09:00:57"), User = "Amy Farrah Fowler", Department = "Tech", Software = "Apple", Seats = 3, Amount = 60 },
            new Purchase { Id = 4, Date = DateTime.Parse("2023-02-03 14:37:45 "), User = "Bert Kibbler", Department = "Customer Success", Software = "Outplay", Seats = 1, Amount = 12 },
            new Purchase { Id = 5, Date = DateTime.Parse("2022-10-18 09:11:51 "), User = "Dr. Beverly Hofstadter", Department = "Sales", Software = "Apple", Seats = 3, Amount = 60 },
            new Purchase { Id = 6, Date = DateTime.Parse("2022-11-29 08:38:37 "), User = "Amy Farrah Fowler", Department = "Tech", Software = "Outplay", Seats = 1, Amount = 12 },
            new Purchase { Id = 7, Date = DateTime.Parse("2022-08-27 06:37:02 "), User = "Priya Koothrappali", Department = "Sales", Software = "Apple", Seats = 3, Amount = 60 },
            new Purchase { Id = 8, Date = DateTime.Parse("2022-06-04 20:14:20 "), User = "Wil Wheaton", Department = "Marketting", Software = "Discord", Seats = 1, Amount = 12 },
            new Purchase { Id = 9, Date = DateTime.Parse("2023-04-29 22:33:56 "), User = "Sheldon Cooper", Department = "Tech", Software = "Apple", Seats = 3, Amount = 60 },
            new Purchase { Id = 10, Date = DateTime.Parse("2022-06-19 12:21:18"), User = "Leslie Winkle", Department = "HR", Software = "Github", Seats = 1, Amount = 12 },
        };

        //For Getting all the data
        [HttpGet]
        public ActionResult<List<Purchase>> GetAllPurchases()
        {
            return purchases;
        }

        
        //For getting data for particular Id
        [HttpGet("{id}")]
        public ActionResult<Purchase> GetPurchaseById(int id)
        {
            var purchase = purchases.FirstOrDefault(p => p.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }
            return purchase;
        }

        //For getting  total number of items (seats) sold in the Marketing department during the third quarter (Q3) of the year:
        [HttpGet("total_items")]
        public ActionResult<int> GetTotalItemsSold(DateTime start_date, DateTime end_date, string department)
        {
            if (department != "Marketting")
            {
                return BadRequest("Invalid department. Please provide 'Marketing' as the department.");
            }

            if (!IsWithinQuarter(start_date, end_date, 3))
            {
                return BadRequest("The provided date range should fall within the third quarter of the year.");
            }

            try
            {
                var totalItemsSold = purchases
                    .Where(p => p.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.Date >= start_date && p.Date <= end_date)
                    .Sum(p => p.Seats);

                return totalItemsSold;
            }
            catch (Exception ex)
            {
       
                return StatusCode(500, "An error occurred while calculating the total items sold.");
            }
        }

        //For Getting percentage of sold items (seats) department wise
        [HttpGet("percentage_of_department_wise_sold_items")]
        public ActionResult<Dictionary<string, double>> GetPercentageOfDepartmentWiseSoldItems(DateTime start_date, DateTime end_date)
        {
            if (start_date > end_date)
            {
                return BadRequest("Invalid date range. The start date cannot be greater than the end date.");
            }

            try
            {
                var departmentSales = purchases
                    .Where(p => p.Date >= start_date && p.Date <= end_date)
                    .GroupBy(p => p.Department)
                    .ToDictionary(
                        group => group.Key,
                        group => Math.Round((group.Sum(p => p.Seats) * 100.0) / purchases.Sum(p => p.Seats), 2)
                    );

                return departmentSales;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed

                return StatusCode(500, "An error occurred while calculating the percentage of sold items by department.");
            }
        }


        //For getting Monthly Sales
        [HttpGet("monthly_sales")]
        public ActionResult<decimal> GetMonthlySales(string product, int year, int month)
        {
            if (string.IsNullOrWhiteSpace(product))
            {
                return BadRequest("Product name is required.");
            }

            if (year <= 0)
            {
                return BadRequest("Invalid year. Please provide a positive value for the year.");
            }

            if (month < 1 || month > 12)
            {
                return BadRequest("Invalid month. Please provide a value between 1 and 12 for the month.");
            }

            try
            {
                var monthlySales = purchases
                    .Where(p => p.Software.Equals(product, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.Date.Year == year && p.Date.Month == month)
                    .Sum(p => p.Amount);

                return monthlySales;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "An error occurred while retrieving the monthly sales.");
            }
        }


        private bool IsWithinQuarter(DateTime startDate, DateTime endDate, int quarter)
        {
            var quarterStartDate = GetQuarterStartDate(quarter);
            var quarterEndDate = GetQuarterEndDate(quarter);

            return startDate >= quarterStartDate && endDate <= quarterEndDate;
        }

        private DateTime GetQuarterStartDate(int quarter)
        {
            var year = DateTime.Now.Year;
            var quarterStartMonth = (quarter - 1) * 3 + 1;
            return new DateTime(year, quarterStartMonth, 1);
        }

        private DateTime GetQuarterEndDate(int quarter)
        {
            var year = DateTime.Now.Year;
            var quarterEndMonth = quarter * 3;
            var lastDayOfMonth = DateTime.DaysInMonth(year, quarterEndMonth);
            return new DateTime(year, quarterEndMonth, lastDayOfMonth);
        }





    }
}
