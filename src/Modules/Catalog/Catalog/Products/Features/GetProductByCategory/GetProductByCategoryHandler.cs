
using Shared.Services;

namespace Catalog.Products.Features.GetProductByCategory
{
    public record GetProductByCategoryQuery(string category):
        IQuery<GetProductByCategoryResult>;

    public record GetProductByCategoryResult(IEnumerable<ProductQueryDto> Products);
    internal class GetProductByCategoryHandler(
        CatalogDbContext dbContext,
        IImageService imageService)
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .Where(p => p.Category.Contains(query.category))
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            var productDtos = (products.Select(product => new ProductQueryDto(
                product.Id,
                product.Name,
                product.Category,
                product.Description,
                product.Price,
                imageService.GetImageUrl(product.ImageName)))).ToList();

            return  new GetProductByCategoryResult(productDtos);
        }
    }
}
