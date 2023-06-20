using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("insert into coupon (productname,description,amount) values (@productName,@description,@amount)", new { productName = coupon.ProductName, description = coupon.Description, amount = coupon.Amount });
            return affected == 0 ? false : true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("delete from coupon where productname=@productname",
                new { productName = productName });
            return affected == 0 ? false : true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("select * from coupon where productname = @productName", new { productName = productName });
            if (coupon == null)
            {
                return new Coupon() { ProductName = "No Discount", Amount = 0, Description = "" };
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("update coupon set productname=@productName,description=@description,amount=@amount where id=@id",
                new { productName = coupon.ProductName, description = coupon.Description, amount = coupon.Amount, id = coupon.Id });
            return affected == 0 ? false : true;
        }
    }
}
