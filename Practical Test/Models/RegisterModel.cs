using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Practical_Test.Models
{
    public class RegisterModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; }        

        [Required(ErrorMessage = "Age is required.")]
        public int Age { get; set; }

        [Display(Name = "City Name")]
        [Required(ErrorMessage = "City is required.")]
        public int CityID { get; set; }

        [Display(Name = "State Name")]
        [Required(ErrorMessage = "State is required.")]
        public int StateID { get; set; }

        [Required(ErrorMessage = "Email ID is required.")]
        [Display(Name = "Email ID")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email ID.")]
        public string EmailID { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Pass { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Pass",ErrorMessage ="The password does not match")]
        public string ConfirmPass { get; set; }

        [Display(Name= "Upload File")]
        public string ImageName { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Image is required.")]
        public HttpPostedFileBase ImageFile { get; set; }

        public string StateName { get; set; }

        public string CityName { get; set; }

        public string ActionType { get; set; }
    }   
    
}