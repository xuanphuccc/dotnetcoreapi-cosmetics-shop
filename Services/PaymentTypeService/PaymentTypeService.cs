using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PaymentTypeService
{
    public class PaymentTypeService : IPaymentTypeService
    {
        private readonly CosmeticsShopContext _context;
        public PaymentTypeService(CosmeticsShopContext context)
        {
            _context = context;
        }

        public async Task<PaymentType> AddPaymentType(PaymentType paymentType)
        {
            await _context.AddAsync(paymentType);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot create payment type");
            }

            return paymentType;
        }

        public async Task<List<PaymentType>> GetAllPaymentTypes()
        {
            var paymentTypes = await _context.PaymentTypes.ToListAsync();
            return paymentTypes;
        }

        public async Task<PaymentType> GetPaymentType(int paymentTypeId)
        {
            var paymentType = await _context.PaymentTypes.FirstOrDefaultAsync(p => p.PaymentTypeId == paymentTypeId);
            return paymentType!;
        }

        public async Task<int> RemovePaymentType(PaymentType paymentType)
        {
            _context.Remove(paymentType);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete payment type");
            }

            return result;
        }

        public async Task<PaymentType> UpdatePaymentType(PaymentType paymentType)
        {
            var existPaymentType = await GetPaymentType(paymentType.PaymentTypeId);
            if (existPaymentType == null)
            {
                throw new Exception("payment type not found");
            }

            existPaymentType.Value = paymentType.Value;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update payment type");
            }

            return existPaymentType;
        }
    }
}
