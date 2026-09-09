using Contracts.Request;
using infrared.Data.Models;
using infrared.Data;
using Microsoft.EntityFrameworkCore;

public class SubscriptionsProvider : ISubscriptionsProvider
{
    private readonly InfraredDbContext _context;

    public SubscriptionsProvider(InfraredDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .ToListAsync(cancellationToken);
    }

    public async Task<Subscription?> GetSubscriptionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest subscriptionRequest, CancellationToken cancellationToken = default)
    {
        var subscription = new Subscription
        {
            Name = subscriptionRequest.Name,
            Timezone = subscriptionRequest.Timezone,
            CreatedOn = subscriptionRequest.CreatedOn
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync(cancellationToken);
        return subscription;
    }

    public async Task<Subscription?> UpdateSubscriptionAsync(Guid id, Subscription subscription, CancellationToken cancellationToken = default)
    {
        var existingSubscription = await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
        if (existingSubscription == null)
        {
            return null;
        }

        existingSubscription.Name = subscription.Name;
        existingSubscription.Timezone = subscription.Timezone;
        // Update other properties as needed

        await _context.SaveChangesAsync(cancellationToken);
        return existingSubscription;
    }

    public async Task<bool> DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
        if (subscription == null)
        {
            return false;
        }

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}