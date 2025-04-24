using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Dapper;

public interface ISqlGenericRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string storedProcedure);
    Task<T?> GetByIdAsync(string storedProcedure, object parameters);
    Task<int> CreateAsync(string storedProcedure, object parameters);
    Task<bool> UpdateAsync(string storedProcedure, object parameters);
    Task<bool> DeleteAsync(string storedProcedure, object parameters);
}