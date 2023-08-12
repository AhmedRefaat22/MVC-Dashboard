using Demo.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;

namespace Demo.PL.ViewModels
{
    // View Model
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name Is Required")]
        [MaxLength(50, ErrorMessage = "Maximunm length of name 50 ")]
        [MinLength(5, ErrorMessage = "Minumim length of name 5 ")]
        public string Name { get; set; }

        [Range(22, 60, ErrorMessage = "Age Must Be Between 22 and 60")]
        public int? Age { get; set; }
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$",
            ErrorMessage = "Address Must be in form of '123-Street-City-Country'")]
        public string Address { get; set; }
        [Range(4000, 8000, ErrorMessage = "Salary Must Be Between 4000 & 8000")]
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        [EmailAddress(ErrorMessage = "Enter Email in a correct form ")]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }

        public string ImageName { get; set; }
        public IFormFile Image { get; set; }
        public int? DepartmentId { get; set; } // FK 

        // Navigational Property-> One
        public Department Department { get; set; }
    }
}
