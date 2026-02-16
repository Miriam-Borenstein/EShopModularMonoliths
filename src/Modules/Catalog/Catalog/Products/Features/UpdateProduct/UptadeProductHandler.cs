using Shared.Services;
using System.Threading.Tasks;

namespace Catalog.Products.Features.UpdateProduct
{
    public record UpdateProductCommand(ProductDto Product, IFormFile? ImageFile)
        : ICommand<UpdateProductResult>;

    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Product.Id).NotEmpty().WithMessage("Id is requierd");
            RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is requierd");
            RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }

    internal class UptadeProductHandler(
        CatalogDbContext dbContext,
        IImageService imageService)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FindAsync([command.Product.Id], cancellationToken: cancellationToken);

            if (product is null) {

                throw new ProductNotFoundException(command.Product.Id.Value);
            }

            if(command.ImageFile is null)
            {
                UpdateProductWithNewValues(product, command.Product, product.ImageName);
            }


            else
            {
                var newImageName = $"{Guid.NewGuid()}_{command.ImageFile.FileName}";

                await imageService.DeleteImageAsync(product.ImageName);

                await imageService.UploadImageAsync(newImageName, command.ImageFile.OpenReadStream());

                UpdateProductWithNewValues(product, command.Product, newImageName);

            }


            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }

        private void UpdateProductWithNewValues(Product product, ProductDto productDto, string? imageName)
        {

            product.Update(
                productDto.Name,
                productDto.Category,
                productDto.Description,
                //productDto.ImageFile,
                imageName,
                productDto.Price
                );
        }
    }
}
