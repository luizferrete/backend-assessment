using EmployeeMaintenance.DL.Entities.Base;
using EmployeeMaintenance.DL.Services.DAL;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMaintenance.DAL.EF.Base
{
    public class EFRepository<T>(DbContext db) : IRepository<T>
        where T : EntityBase, new()
    {
       
        protected readonly DbContext _dbContext = db;

        protected virtual void Set(T model, EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    _dbContext.Set<T>().Add(model);
                    break;
                case EntityState.Modified:
                    _dbContext.Set<T>().Update(model).Property(_ => _.Id).IsModified = false;
                    break;
                case EntityState.Deleted:
                    _dbContext.Set<T>().Remove(model);
                    break;
            }
        }

        protected virtual void SingleOperation(T model, EntityState state, bool autoDetectChangesEnabled = false)
        {
            if (model == null)
                return;

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;

            Set(model, state);

            _dbContext.SaveChanges();
        }

        protected virtual async Task SingleOperationAsync(T model, EntityState state, bool autoDetectChangesEnabled = false)
        {
            if (model == null)
                return;

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;

            Set(model, state);

            await _dbContext.SaveChangesAsync();
        }

        public async virtual Task DeleteAsync(T model)
        {
            if (model == null)
                return;

            model.FlagActive = false;
            await UpdateAsync(model);
        }

        public async virtual Task<T> FindByIdAsync<TKeyProp>(TKeyProp key)
        {
            return await _dbContext.Set<T>().FindAsync(key);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().Where(obj => obj.FlagActive == true);
        }

        public virtual Task InsertAsync(T model, bool autoDetectChangesEnabled = false) => SingleOperationAsync(model, EntityState.Added, autoDetectChangesEnabled);

        public virtual Task UpdateAsync(T model, bool autoDetectChangesEnabled = false) => SingleOperationAsync(model, EntityState.Modified, autoDetectChangesEnabled);

        public async Task<bool> ExistsAsync<TKeyProp>(TKeyProp key)
        {
            return await _dbContext.Set<T>().AnyAsync(e => e.FlagActive && e.Id.Equals(key));
        }
    }
}
