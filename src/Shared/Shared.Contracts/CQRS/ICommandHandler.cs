using MediatR;

namespace Shared.Contracts.CQRS
{
    public interface ICommandHandler<in TCommand>
        : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {
    }

    public interface ICommandHandler<in TCommand, TResult> 
        : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
    }
}
