using Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DL
{
    public interface IBaseDL
    {
         IDbConnection GetDbConnection(string connectionString);
         T GetByID<T>(int ID);

         T GetByID<T>(string ID);

         IEnumerable<T> GetByFilter<T>(string filter);

         IEnumerable<T> GetAll<T>();

         IEnumerable<BaseEntity> GetAllBase();

         T UpdateOneByID<T>(T entity);

         bool UpdateEntities<T>(IEnumerable<T> entities);

         bool SaveChanges<T>(IEnumerable<T> entities);

         bool DeleteEntities<T>(IEnumerable<T> entities);

        Task<bool> ExecuteUsingStoredProcedure(string storedProcedure, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction);

        Task<T> ExecuteScalarUsingStoredProcedure<T>(string storedProcedure, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction);

        string GetInsertProcedureName(BaseEntity entity);

        string GetUpdateProcedureName(BaseEntity entity);

        string GetDeleteProcedureName(BaseEntity entity);


    }
}
