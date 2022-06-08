using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
namespace auction.Validator
{
    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime requestDate;
            DateTime.TryParse(value.ToString(),out requestDate);

            if(requestDate == DateTime.MinValue){
                new ValidationResult("Date must be in correct format");
            }

            return requestDate.Date > DateTime.Now.Date
                  ? ValidationResult.Success
                  : new ValidationResult("Date must be future date");

        }
    }

}