﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Context;

public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContextDesignFactory()
    {

    }

    public OrderDbContext CreateDbContext(string[] args)
    {
        var connStr = "YOUR_CONNECTION_STRING";

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(connStr);

        return new OrderDbContext(optionsBuilder.Options, new NoMeditor());
    }

    private class NoMeditor : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<TResponse>(default);
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(default);
        }
    }
}