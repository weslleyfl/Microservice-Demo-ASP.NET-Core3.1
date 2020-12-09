using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using OrderApi.Models.v1;

namespace OrderApi.Validators.v1
{
    public class OrderModelValidator : AbstractValidator<OrderModel>
    {
        public OrderModelValidator()
        {
            RuleFor(x => x.CustomerFullName)
                .NotNull().WithMessage("Campo CustomerFullName obrigatorio")
                .MinimumLength(2).WithMessage("O nome do cliente deve ter pelo menos 2 caracteres");

            //RuleFor(x => x.CustomerFullName)
            //   .MinimumLength(2).WithMessage("The customer name must be at least 2 character long");

        }
    }
}
