
namespace EmployeeMaintenance.DL.Services.DAL
{
    public interface IRepository<T> where T : IEntity
    {
        Task InsertAsync(T model, bool autoDetectChangesEnabled = false);

        Task UpdateAsync(T model, bool autoDetectChangesEnabled = false);

        Task DeleteAsync(T model);

        Task<T> FindByIdAsync<TKeyProp>(TKeyProp key);

        IEnumerable<T> GetAll();

        Task<bool> ExistsAsync<TKeyProp>(TKeyProp key);
    }
}
