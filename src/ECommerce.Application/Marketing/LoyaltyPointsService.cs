using System;
using System.Threading.Tasks;
using ECommerce.Settings;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Settings;
using Volo.Abp.Guids;

namespace ECommerce.Marketing;

/// <summary>
/// Awards loyalty points when an order is confirmed. Called from PaymentAppService and OrderAdminAppService.
/// </summary>
public interface ILoyaltyPointsService
{
    Task AwardPointsForOrderAsync(Guid orderId, Guid? userId, decimal orderTotal);
}

public class LoyaltyPointsService : ILoyaltyPointsService
{
    private readonly IRepository<CustomerPoints, Guid> _customerPointsRepository;
    private readonly IRepository<PointsTransaction, Guid> _transactionRepository;
    private readonly ISettingProvider _settingProvider;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<LoyaltyPointsService> _logger;

    public LoyaltyPointsService(
        IRepository<CustomerPoints, Guid> customerPointsRepository,
        IRepository<PointsTransaction, Guid> transactionRepository,
        ISettingProvider settingProvider,
        IGuidGenerator guidGenerator,
        ILogger<LoyaltyPointsService> logger)
    {
        _customerPointsRepository = customerPointsRepository;
        _transactionRepository = transactionRepository;
        _settingProvider = settingProvider;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    public async Task AwardPointsForOrderAsync(Guid orderId, Guid? userId, decimal orderTotal)
    {
        if (!userId.HasValue || userId.Value == Guid.Empty)
            return;

        var pointsPerUnitStr = await _settingProvider.GetOrNullAsync(ECommerceSettings.Loyalty.PointsPerCurrencyUnit);
        if (!int.TryParse(pointsPerUnitStr, out var pointsPerUnit) || pointsPerUnit <= 0)
            return;

        var points = (int)Math.Floor(orderTotal * pointsPerUnit);
        if (points <= 0)
            return;

        var customerPoints = await _customerPointsRepository.FirstOrDefaultAsync(c => c.UserId == userId.Value);
        if (customerPoints == null)
        {
            customerPoints = new CustomerPoints(_guidGenerator.Create(), userId.Value, 0);
            await _customerPointsRepository.InsertAsync(customerPoints);
        }

        customerPoints.AddPoints(points);
        await _customerPointsRepository.UpdateAsync(customerPoints);

        var transaction = new PointsTransaction(
            _guidGenerator.Create(),
            userId.Value,
            points,
            PointsTransactionType.Earn,
            orderId: orderId,
            description: $"Points for order {orderId}");
        await _transactionRepository.InsertAsync(transaction);
    }
}
