using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Trucks.Models;
using Xunit;

namespace Trucks.Test
{
    public class TruckModelTest
    {
        [Fact]
        public void Truck_CheckTitleValidation_WhenInvalid()
        {   
            // Arrange
            var truckWithTitleRequired = new Truck { Title = null };
            var truckWithTitleTooLong = new Truck { Title = new string('a', 101) };

            // Act
            Func<IList<ValidationResult>> titleRequiredValidation = () => { return ValidateModel(truckWithTitleRequired); };
            Func<IList<ValidationResult>> titleTooLongValidation = () => { return ValidateModel(truckWithTitleTooLong); };

            // Assert
            Assert.Contains(titleRequiredValidation(), v
                => v.MemberNames.Contains("Title")
                && v.ErrorMessage.Contains("required")
            );
            Assert.Contains(titleTooLongValidation(), v
                => v.MemberNames.Contains("Title")
                && v.ErrorMessage.Contains("more than 100")
            );
        }

        [Fact]
        public void Truck_CheckTitleValidation_WhenValid()
        {
            // Arrange
            var truck = new Truck { Title = "aaa" };

            // Act
            Func<IList<ValidationResult>> titleCorrectValidation = () => { return ValidateModel(truck); };

            // Assert
            Assert.DoesNotContain(titleCorrectValidation(), v
                => v.MemberNames.Contains("Title")
            );
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2022)]
        [InlineData(2023)]
        public void Truck_CheckManufacturingYearValidation(int year)
        {
            // Arrange
            var instance = new Truck { ManufacturingYear = year };

            if (year == DateTime.Now.Year)
            {
                Assert.True(ValidateProperty(instance, instance.ManufacturingYear, nameof(instance.ManufacturingYear)).Count == 0);
            }
            else
            {
                Assert.True(ValidateProperty(instance, instance.ManufacturingYear, nameof(instance.ManufacturingYear)).Count > 0);
            }
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2022)]
        [InlineData(2023)]
        [InlineData(2024)]
        public void Truck_CheckModelYearValidation(int year)
        {
            // Arrange
            var instance = new Truck { ModelYear = year };

            if (year == DateTime.Now.Year || year == DateTime.Now.Year + 1)
            {
                Assert.True(ValidateProperty(instance, instance.ModelYear, nameof(instance.ModelYear)).Count == 0);
            }
            else
            {
                Assert.True(ValidateProperty(instance, instance.ModelYear, nameof(instance.ModelYear)).Count > 0);
            }
        }

        #region Members validation

        private IList<ValidationResult> ValidateProperty(object model, object value, string memberName)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model) { MemberName = memberName };
            Validator.TryValidateProperty(value, ctx, validationResults);
            return validationResults;
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}
