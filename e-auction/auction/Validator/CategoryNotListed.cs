using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
namespace auction.Validator
{
    public class CategoryNotListed : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null)
               return new ValidationResult("");

            IList<string> categoryList= new List<string>{
                "Painting","Sculptor","Ornament"
            };

            return categoryList.Any(x=>x.ToLower().Equals(value.ToString().ToLower()))
                  ? ValidationResult.Success
                  : new ValidationResult("category not listed!");

        }
    }

}