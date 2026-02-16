using Shared.Services;

namespace Catalog.Products.Features.GetProductById;

internal class GetProductByIdHandler(
    CatalogDbContext dbContext,
    IImageService imageService
    )
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(query.Id);
        }

        var productDto = new ProductQueryDto(
                product.Id,
                product.Name,
                product.Category,
                product.Description,
                product.Price,
                imageService.GetImageUrl(product.ImageName)
                );



        return new GetProductByIdResult(productDto);
    }
}
