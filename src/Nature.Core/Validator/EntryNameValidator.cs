using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation.Validators;
using Nature.Core.Properties;

namespace Nature.Core.Validator
{
    public class EntryNameValidator : PropertyValidator, IRegularExpressionValidator
    {
        private readonly Regex regex;
        private const string expression = @"^[a-zA-Z0-9][\w-_]{1,50}$";
        private const string errorStr = "'{PropertyName}'格式不正确！只能包含英文字母、数字、下划线(_)，并且以英文字母或数字开头，且最小长度为2，最大长度为50！";
        public EntryNameValidator()
        : base(() => Resource.EntryName_Error)
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
        public string Expression => expression;
    }
}
