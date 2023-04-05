using Insurance.Api.Data;
using Insurance.Api.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.OrderRepo
{
    public class OrderRepo : BaseRepository<Order>, IOrderRepo
    {
        public OrderRepo(DataContext dataContext) : base(dataContext.Orders)
        {
        }
    }
}
