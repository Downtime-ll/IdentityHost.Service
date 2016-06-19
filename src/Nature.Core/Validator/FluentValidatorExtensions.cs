using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.Validator
{
    using FluentValidation;
    public static class FluentValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> EntryName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EntryNameValidator());
        }

        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PhoneNumberValidator());
        }
    }
}
