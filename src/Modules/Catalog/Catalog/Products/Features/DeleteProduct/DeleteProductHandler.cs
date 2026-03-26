using Shared.Services;

namespace Catalog.Products.Features.DeleteProduct
{
    public record DeleteProductCommand(Guid ProductId)
        : ICommand<DeleteProductResult>;

    public record DeleteProductResult(bool IsSuccess);

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Id is requierd");
        }
    }
    internal class DeleteProductHandler(
        CatalogDbContext dbContext,
        IImageService imageService)
        : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products
                .FindAsync([command.ProductId], cancellationToken: cancellationToken);

            if (product is null)
            {
                throw new ProductNotFoundException(command.ProductId);
            }

            if (!string.IsNullOrEmpty(product.ImageName))
            {
                await imageService.DeleteImageAsync(product.ImageName);
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);



            return new DeleteProductResult(true);
        }
    }
}
