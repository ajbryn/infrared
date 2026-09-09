using Contracts.Response;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsProvider _subscriptionsProvider;

    public SubscriptionsController(ISubscriptionsProvider subscriptionsProvider)
    {
        _subscriptionsProvider = subscriptionsProvider;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetAllSubscriptions(CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionsProvider.GetAllSubscriptionsAsync(cancellationToken);
        var response = subscriptions.Select(s => new SubscriptionResponse
        {
            Id = s.Id,
            Name = s.Name,
            Timezone = s.Timezone,
            CreatedOn = s.CreatedOn
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionResponse>> GetSubscriptionById(Guid id, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsProvider.GetSubscriptionByIdAsync(id, cancellationToken);
        if (subscription == null)
        {
            return NotFound();
        }

        var response = new SubscriptionResponse
        {
            Id = subscription.Id,
            Name = subscription.Name,
            Timezone = subscription.Timezone,
            CreatedOn = subscription.CreatedOn
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> CreateSubscription(SubscriptionRequest subscriptionRequest, CancellationToken cancellationToken)
    {
        var createdSubscription = await _subscriptionsProvider.CreateSubscriptionAsync(subscriptionRequest, cancellationToken);

        var response = new SubscriptionResponse
        {
            Id = createdSubscription.Id,
            Name = createdSubscription.Name,
            Timezone = createdSubscription.Timezone,
            CreatedOn = createdSubscription.CreatedOn
        };

        return Created(response);
    }

}