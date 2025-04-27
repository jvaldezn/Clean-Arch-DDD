using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Http;

public interface IHttpRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string? url = null);
    Task<T?> GetByIdAsync(string id, string? url = null);
    Task<T?> CreateAsync(T entity, string? url = null);
    Task<bool> UpdateAsync(string id, T entity, string? url = null);
    Task<bool> DeleteAsync(string id, string? url = null);
}