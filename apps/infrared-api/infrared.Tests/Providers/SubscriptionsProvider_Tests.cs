using infrared.Contracts.Request;
using infrared.Data;
using infrared.Data.Models;
using Microsoft.EntityFrameworkCore;
// using EntityFrameworkCore.InMemory;

namespace infrared.Tests.Providers;

public class SubscriptionsProvider_Tests
{
    private readonly SubscriptionsProvider _subscriptionsProvider;
    private readonly InfraredDbContext _context;

    public SubscriptionsProvider_Tests()
    {
        var options = new DbContextOptionsBuilder<InfraredDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new InfraredDbContext(options);
        _subscriptionsProvider = new SubscriptionsProvider(_context);
    }

    private async Task ClearDatabaseAsync()
    {
        _context.Subscriptions.RemoveRange(_context.Subscriptions);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateSubscriptionAsync_ShouldAddSubscription()
    {
        // Arrange
        var subscriptionRequest = new SubscriptionRequest
        {
            Name = "Test Subscription",
            Timezone = "UTC",
            CreatedOn = DateTime.UtcNow
        };

        // Act
        var result = await _subscriptionsProvider.CreateSubscriptionAsync(subscriptionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subscriptionRequest.Name, result.Name);
        Assert.Equal(subscriptionRequest.Timezone, result.Timezone);
    }

    [Fact]
    public async Task GetAllSubscriptionsAsync_ShouldReturnAllSubscriptions()
    {
        await ClearDatabaseAsync();
        
        // Arrange
        var subscription1 = new Subscription { Name = "Subscription 1", Timezone = "UTC", CreatedOn = DateTime.UtcNow };
        var subscription2 = new Subscription { Name = "Subscription 2", Timezone = "UTC", CreatedOn = DateTime.UtcNow };
        _context.Subscriptions.AddRange(subscription1, subscription2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subscriptionsProvider.GetAllSubscriptionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}