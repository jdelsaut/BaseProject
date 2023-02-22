using System.Threading.Tasks;

namespace Assignment.Persistance.Base
{
    public interface IRepository
    {
        Task ClearCollectionAsync();
    }
}
