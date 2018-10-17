using System.Collections.Generic;
using Dapper;

namespace CHOMP_DEMO.Providers
{
    public interface IASIProvider
    {
        ICollection<T> Procedure<T>(string procedureName, DynamicParameters parameters);
    }
}