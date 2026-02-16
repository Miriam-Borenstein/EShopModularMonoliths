using Microsoft.Extensions.Configuration;
using Shared.Pagination;
using Shared.Services;
using System.Linq;

namespace Catalog.Products.Features.GetProducts
{
    public record GetProductsQuery(PaginationRequest PaginationRequest)
        :IQuery<GetProductsResult>;

    public record GetProductsResult(PaginatedResult<ProductQueryDto> Products);
    internal class GetProductsHandler(
        CatalogDbContext dbContext,
        IImageService imageService)
        : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await dbContext.Products.LongCountAsync(cancellationToken);

            var products = await dbContext.Products
                .AsNoTracking()                
                .OrderBy(p => p.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            //var productDtos = products.Adapt<List<ProductQueryDto>>();

            // Map products to ProductQueryDto and include the ImageUrl
            var productDtos = products.Select(product => new ProductQueryDto(
                product.Id,
                product.Name,
                product.Category,
                product.Description,
                product.Price,
                imageService.GetImageUrl(product.ImageName) // Construct the full ImageUrl
            )).ToList();


            return new GetProductsResult(
                new PaginatedResult<ProductQueryDto>(
                    pageIndex, pageSize, totalCount, productDtos)
                );
        }

     
    }
}
