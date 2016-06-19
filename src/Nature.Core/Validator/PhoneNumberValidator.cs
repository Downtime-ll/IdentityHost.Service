using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation.Validators;
using Nature.Core.Properties;

namespace Nature.Core.Validator
{
    public class PhoneNumberValidator : PropertyValidator, IRegularExpressionValidator
    {
        private readonly Regex regex;
        private const string expression = @"^((\d{3,4}\-?\d{7,8})|(\d{5}))$";
        private const string errorStr = "'{PropertyName}'格式不正确！请输入正确的电话号码！";
        public PhoneNumberValidator()
        : base(() => errorStr)
        {
            regex = new Regex(expression, RegexOptions.IgnoreCase);
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null) return true;
            if (!regex.IsMatch((string)context.PropertyValue))
            {
                return false;
            }
            return true;
        }
        public string Expression
        {
            get { return expression; }
        }
    }
}
