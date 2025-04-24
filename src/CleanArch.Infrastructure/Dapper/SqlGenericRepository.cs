using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Dapper;

public class SqlGenericRepository<T> : ISqlGenericRepository<T>
{
    private readonly IConfiguration _configuration;

    public SqlGenericRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("AppDbConnection"));

    public async Task<IEnumerable<T>> GetAllAsync(string storedProcedure)
    {
        using var db = Connection;
        return await db.QueryAsync<T>(storedProcedure, commandType: CommandType.StoredProcedure);
    }

    public async Task<T?> GetByIdAsync(string storedProcedure, object parameters)
    {
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(string storedProcedure, object parameters)
    {
        using var db = Connection;
        return await db.ExecuteScalarAsync<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateAsync(string storedProcedure, object parameters)
    {
        using var db = Connection;
        var affected = await db.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(string storedProcedure, object parameters)
    {
        using var db = Connection;
        var affected = await db.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }
}