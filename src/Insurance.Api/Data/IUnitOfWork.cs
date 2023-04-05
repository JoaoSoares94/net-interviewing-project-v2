using System.Threading.Tasks;

namespace Insurance.Api.Data
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}
