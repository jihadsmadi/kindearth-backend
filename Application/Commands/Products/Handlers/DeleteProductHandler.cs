using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands.Products;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
                if (product == null)
                    return false;

                await _unitOfWork.Products.DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 