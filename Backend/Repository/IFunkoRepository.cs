using Backend.DTO;
using Backend.Models;

namespace Backend.Repository;

public interface IFunkoRepository
{
    Task<Funko?> GetByIdAsync(long id);
    Task<(IEnumerable<Funko> Items, int TotalCount)> GetAllAsync(FilterDTO filter);    
    Task<Funko> CreateAsync(Funko funko);
    Task<Funko?> UpdateAsync(long id, Funko funko);
    Task<Funko?> DeleteAsync(long id);
}