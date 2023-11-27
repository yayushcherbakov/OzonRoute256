using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.DataAccess.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Contracts;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Enums;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Models.Order;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

public class OrderService : IOrderService
{
    private readonly IItemRepository _itemRepository;

    public OrderService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task UpdateProductAccounting(OrderEvent[] events, CancellationToken cancellationToken)
    {
        var itemIdsToUpdate = events
            .SelectMany(x => x.Positions)
            .Select(x => x.ItemId)
            .Distinct()
            .ToArray();

        var statisticsToUpdate = (await _itemRepository.Get(itemIdsToUpdate, cancellationToken))
            .ToDictionary(x => x.ItemId);

        var allItems = events.SelectMany(
            x => x.Positions,
            (order, position) => new
            {
                order.Status,
                position.ItemId,
                position.Quantity
            });

        var totalItemsQuantityByStatus = allItems
            .GroupBy(
                x => new
                {
                    x.Status,
                    x.ItemId
                })
            .Select(
                x => new
                {
                    x.Key.Status,
                    x.Key.ItemId,
                    TotalQuantity = x.Sum(y => y.Quantity)
                });

        var itemStatisticsFromEvents = totalItemsQuantityByStatus
            .GroupBy(x => x.ItemId)
            .Select(
                x => new
                {
                    ItemId = x.Key,
                    CreatedCount = x
                        .Where(y => y.Status == OrderStatus.Created)
                        .Sum(y => y.TotalQuantity),
                    DeliveredCount = x
                        .Where(y => y.Status == OrderStatus.Delivered)
                        .Sum(y => y.TotalQuantity),
                    CancelledCount = x
                        .Where(y => y.Status == OrderStatus.Canceled)
                        .Sum(y => y.TotalQuantity)
                });

        foreach (var newStatistic in itemStatisticsFromEvents)
        {
            if (statisticsToUpdate.TryGetValue(newStatistic.ItemId, out var statisticFromDatabase))
            {
                statisticFromDatabase.CreatedCount = statisticFromDatabase.CreatedCount + newStatistic.CreatedCount
                                                     - newStatistic.DeliveredCount - newStatistic.CancelledCount;

                statisticFromDatabase.DeliveredCount += newStatistic.DeliveredCount;

                statisticFromDatabase.CancelledCount += newStatistic.CancelledCount;

                await _itemRepository.Update(statisticFromDatabase, cancellationToken);
            }
            else
            {
                await _itemRepository.Add(
                    new[]
                    {
                        new ProductStatisticsEntityV1
                        {
                            ItemId = newStatistic.ItemId,
                            CreatedCount = newStatistic.CreatedCount - newStatistic.DeliveredCount - newStatistic.CancelledCount,
                            DeliveredCount = newStatistic.DeliveredCount,
                            CancelledCount = newStatistic.CancelledCount,
                            ModifiedAt = DateTimeOffset.UtcNow
                        }
                    },
                    cancellationToken);
            }
        }
    }
}
