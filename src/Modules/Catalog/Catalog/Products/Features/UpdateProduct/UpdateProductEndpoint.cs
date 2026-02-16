using Catalog.Products.Features.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Products.Features.UpdateProduct
{
    //public class UpdateProductRequest(ProductDto Product, IFormFile? ImageFile = null);
    public class UpdateProductRequest
    {
        public ProductDto Product { get; set; } = default!;

        public IFormFile? ImageFile { get; set; }
    };
    public record UpdateProductResponse(bool IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async ([FromForm] UpdateProductRequest request, ISender sender) =>
            {
                var command = new UpdateProductCommand(request.Product, request.ImageFile);
                //var command = request.Adapt<UpdateProductCommand>();

                var result = await sender.Send(command);    

                var response = result.Adapt<UpdateProductResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product")
            .WithDescription("Update Product")
            .DisableAntiforgery();
            
        }
    }
}
