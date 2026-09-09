using Contracts.Request;
using infrared.Data.Models;

public interface ISubscriptionsProvider
{
    Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task<Subscription?> GetSubscriptionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest subscriptionRequest, CancellationToken cancellationToken = default);
    Task<Subscription?> UpdateSubscriptionAsync(Guid id, Subscription subscription, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken = default);
}