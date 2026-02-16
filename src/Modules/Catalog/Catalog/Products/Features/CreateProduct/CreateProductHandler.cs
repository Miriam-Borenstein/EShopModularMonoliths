using Microsoft.Extensions.Configuration;
using Shared.Services;
using System.Threading.Tasks;

namespace Catalog.Products.Features.CreateProduct
{
    public record CreateProductCommand(ProductDto Product, IFormFile ImageFile)
        : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is requierd");
            RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category is requierd");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is requierd");
            RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }

    internal class CreateProductHandler
        (CatalogDbContext dbContext,
        IImageService imageService)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {

            var fileName = $"{Guid.NewGuid()}_{command.ImageFile.FileName}";
 
            await imageService.UploadImageAsync(fileName, command.ImageFile.OpenReadStream());

            var product = CreateNewProduct(command.Product, fileName);
       
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }

        private Product CreateNewProduct(ProductDto productDto, string fileName)
        {

            var product = Product.Create(
                Guid.NewGuid(),
                productDto.Name,
                productDto.Category,
                productDto.Description,
                fileName,
                //productDto.ImageFile,
                productDto.Price
                );

            return product;
        }
    }
}
