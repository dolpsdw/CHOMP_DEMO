using System.Collections.Generic;
using Dapper;

namespace CHOMP_DEMO.Providers
{
    public interface IProspectProvider
    {
        ICollection<T> Procedure<T>(string procedureName, DynamicParameters parameters);
    }
}