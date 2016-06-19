using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using IdentityService.Domain.Entitys;
using Nature.Core.ObjectMapping;
using Nature.Core.Validator;

namespace IdentityService.Domain.Models
{
    [AutoMap(typeof(User))]
    public class CreateUserModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public Sex Sex { get; set; }
    }

    public class UserModelValidation : AbstractValidator<CreateUserModel>
    {
        public UserModelValidation()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("用户名不能为空！")
                .EntryName();

            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Name).NotEmpty().Length(0, 20);
            RuleFor(x => x.PhoneNumber).NotEmpty().PhoneNumber();

            RuleSet("PasswordRuleset", () =>
            {
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.ConfirmPassword).NotEmpty();
                RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
            });
        }
    }

}
