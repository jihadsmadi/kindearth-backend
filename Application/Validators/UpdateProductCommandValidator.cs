using Application.Commands.Products;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Request.Id).GreaterThan(0);
            RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.Description).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Request.CategoryId).GreaterThan(0);
            RuleFor(x => x.Request.Stocks).NotNull();
            RuleForEach(x => x.Request.Stocks).ChildRules(stocks =>
            {
                stocks.RuleFor(s => s.Quantity).GreaterThanOrEqualTo(0);
            });
        }
    }

    public class UpdateProductStockRequestValidator : AbstractValidator<Application.DTOs.Products.UpdateProductStockRequest>
    {
        public UpdateProductStockRequestValidator()
        {
            RuleFor(x => x.Size).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
            RuleFor(x => x.StockCount).GreaterThanOrEqualTo(0);
        }
    }
} 