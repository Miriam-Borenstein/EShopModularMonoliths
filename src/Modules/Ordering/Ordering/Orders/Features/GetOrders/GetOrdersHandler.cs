namespace Ordering.Orders.Features.GetOrders;

public record GetOrdersQuery(PaginationRequest Pagination)
    : IQuery<GetOrdersResult>;

public record GetOrdersResult(PaginatedResult<OrderDto> Orders);
internal class GetOrdersHandler(OrderingDbContext dbContext)
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.Pagination.PageIndex;
        var pageSize = query.Pagination.PageSize;

        var totalCount = await dbContext.Orders.LongCountAsync(cancellationToken);

        var orders =await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .OrderBy(o => o.OrderName)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var ordersDtos = orders.Adapt<List<OrderDto>>();

        var result = new PaginatedResult<OrderDto>(pageIndex, pageSize, totalCount, ordersDtos);

        return new GetOrdersResult(result);
    }
}
