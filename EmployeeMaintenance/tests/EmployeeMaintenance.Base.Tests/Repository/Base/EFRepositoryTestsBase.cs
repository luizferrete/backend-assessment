using EmployeeMaintenance.DAL.EF;
using EmployeeMaintenance.DAL.EF.Base;
using EmployeeMaintenance.DL.Entities.Base;
using EmployeeMaintenance.Tests.Fixtures;

namespace EmployeeMaintenance.Tests.Repository.Base
{
    public abstract class EFRepositoryTestsBase<TEntity, TRepository>
        where TEntity : EntityBase, new()
        where TRepository : EFRepository<TEntity>
    {
        protected readonly EntityContext _context;
        protected readonly TRepository _repository;

        protected EFRepositoryTestsBase(DbContextFixture fixture, TRepository repository)
        {
            fixture.ResetDatabase();
            _context = fixture.Context;
            _repository = repository;
        }

        protected abstract TEntity CreateEntity(bool isActive = true);

        [Fact]
        public async Task FindById_Should_Return_Entity()
        {
            var entity = CreateEntity();
            await _repository.InsertAsync(entity);

            var foundEntity = await _repository.FindByIdAsync(entity.Id);

            Assert.NotNull(foundEntity);
            Assert.Equal(foundEntity.Id, entity.Id);
        }

        [Fact]
        public async Task InsertAsync_Should_Insert_Entity()
        {
            var entity = CreateEntity();

            await _repository.InsertAsync(entity);
            var inserted = await _repository.FindByIdAsync(entity.Id);

            Assert.NotNull(inserted);
            Assert.Equal(entity.Id, inserted.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity()
        {
            var entity = CreateEntity();
            await _repository.InsertAsync(entity);

            entity.FlagActive = false;
            await _repository.UpdateAsync(entity);

            var updated = await _repository.FindByIdAsync(entity.Id);

            Assert.NotNull(updated);
            Assert.False(updated.FlagActive);
        }

        [Fact]
        public async Task DeleteAsync_Should_Set_FlagActive_False()
        {
            var entity = CreateEntity();
            await _repository.InsertAsync(entity);

            await _repository.DeleteAsync(entity);

            var deleted = await _repository.FindByIdAsync(entity.Id);

            Assert.NotNull(deleted);
            Assert.False(deleted.FlagActive);
        }

        [Fact]
        public async Task GetAll_Should_Return_Only_Active_Entities()
        {
            var activeEntity = CreateEntity(true);
            var inactiveEntity = CreateEntity(false);

            await _repository.InsertAsync(activeEntity);
            await _repository.InsertAsync(inactiveEntity);

            var activeEntities = _repository.GetAll();

            Assert.NotEmpty(activeEntities);
            Assert.Contains(activeEntities, entity => entity.Id == activeEntity.Id);
            Assert.DoesNotContain(activeEntities, entity => entity.Id == inactiveEntity.Id);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_If_Exists()
        {
            var entity = CreateEntity();
            await _repository.InsertAsync(entity);

            var exists = await _repository.ExistsAsync(entity.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_False_If_NotExists()
        {
            var exists = await _repository.ExistsAsync(9999);

            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_False_If_Deleted()
        {
            var entity = CreateEntity();
            await _repository.InsertAsync(entity);
            Assert.True(entity.FlagActive);
            await _repository.DeleteAsync(entity);

            var exists = await _repository.ExistsAsync(entity.Id);

            Assert.False(exists);
            Assert.False(entity.FlagActive);
        }
    }
}
