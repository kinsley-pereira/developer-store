using FluentValidation;
using Sales.Application.Sales.Commands.UpdateSale;

namespace Sales.Application.Validators
{
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.CustomerName).NotEmpty();
            RuleFor(x => x.BranchId).NotEmpty();
            RuleFor(x => x.BranchName).NotEmpty();
            RuleFor(x => x.Items).NotNull().Must(i => i.Count > 0).WithMessage("Sale must have at least one item");
            RuleForEach(x => x.Items).ChildRules(it =>
            {
                it.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
                it.RuleFor(i => i.UnitPrice).GreaterThan(0);
                it.RuleFor(i => i.ProductName).NotEmpty();
            });
            RuleFor(x => x.Items)
                .Must(items => items.All(i => i.Quantity <= 20))
                .WithMessage("Cannot sell more than 20 identical items");
        }
    }
}
