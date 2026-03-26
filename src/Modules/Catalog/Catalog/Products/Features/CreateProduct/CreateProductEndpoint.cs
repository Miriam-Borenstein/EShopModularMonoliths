using Microsoft.AspNetCore.Mvc;

namespace Catalog.Products.Features.CreateProduct
{
    public record CreateProductRequest(ProductDto Product, IFormFile ImageFile);
    public record CreateProductResponse(Guid Id);
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async ([FromForm] CreateProductRequest request, ISender sender) =>
                {
                    var command = new CreateProductCommand(request.Product, request.ImageFile);
                    //request.Adapt<CreateProductCommand>();

                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateProductResponse>();

                    return Results.Created($"/products/{response.Id}", response);
                })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product")
            .DisableAntiforgery();

                
        }
    }
}
