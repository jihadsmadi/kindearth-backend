using Application.Commands.Products;
using FluentValidation;

namespace Application.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.Description).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Request.CategoryId).GreaterThan(0);
            RuleFor(x => x.Request.VendorProfileId).NotEmpty();
            RuleFor(x => x.Request.Stocks).NotNull();
            RuleForEach(x => x.Request.Stocks).ChildRules(stocks =>
            {
                stocks.RuleFor(s => s.Quantity).GreaterThanOrEqualTo(0);
            });
        }
    }

    public class CreateProductStockRequestValidator : AbstractValidator<Application.DTOs.Products.CreateProductStockRequest>
    {
        public CreateProductStockRequestValidator()
        {
            RuleFor(x => x.Size).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
            RuleFor(x => x.StockCount).GreaterThanOrEqualTo(0);
        }
    }
} 