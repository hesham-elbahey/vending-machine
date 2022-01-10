using System;
using System.ComponentModel.DataAnnotations;
using FlapKapVendingMachine.Domain.Models.Enums;

namespace FlapKapVendingMachine.Attributes
{
    public class CoinAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int intValue = (int)value;
            if (intValue != ((int)CoinDenomination.Five) &&
                intValue != ((int)CoinDenomination.Ten) &&
                intValue != ((int)CoinDenomination.Twenty) &&
                intValue != ((int)CoinDenomination.Fifty) &&
                intValue != ((int)CoinDenomination.Hundred))
                return new ValidationResult("Coin value is invalid.");
            return ValidationResult.Success;
        }
    }
}
