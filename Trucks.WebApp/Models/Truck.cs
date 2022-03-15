using System;
using System.ComponentModel.DataAnnotations;

namespace Trucks.Models
{
    public class Truck
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please select your model.")]
        [EnumDataType(typeof(TruckModel))]
        public TruckModel Model { get; set; }
        [Required(ErrorMessage = "You must specify the manufacturing year.")]
        [Display(Name = "Manufacturing Year")]
        [ManufacturingYear]
        public int ManufacturingYear { get; set; }
        [Required(ErrorMessage = "You must specify the model year.")]
        [Display(Name = "Model Year")]
        [ModelYear]
        public int ModelYear { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
    }

    #region Enumerations

    public enum TruckModel
    {
        FH = 1,
        FM = 2
    }

    #endregion

    #region Validations
    
    public class ModelYearAttribute : ValidationAttribute
    {
        public int Year { get; } = DateTime.Now.Year;

        public string GetErrorMessage() =>
            $"The model year must be the current or a subsequent year.";

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var truck = (Truck)validationContext.ObjectInstance;

            if (truck.ModelYear < Year || truck.ModelYear > Year + 1)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }

    public class ManufacturingYearAttribute : ValidationAttribute
    {
        public int Year { get; } = DateTime.Now.Year;

        public string GetErrorMessage() =>
            $"The Manufacturing year must be the current year.";

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var truck = (Truck)validationContext.ObjectInstance;

            if (truck.ManufacturingYear != Year)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }

    #endregion
}
