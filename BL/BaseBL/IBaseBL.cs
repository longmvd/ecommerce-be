using Common.Entities;
using ECommerce.Common.DTO;
using ECommerce.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BL
{
    public interface IBaseBL
    {

        IEnumerable<User> GetAll();

        ServiceResponse Save(BaseEntity entity);

        ServiceResponse SaveChanges(BaseEntity entity);

        ServiceResponse SaveList(IEnumerable<BaseEntity> entities);

        string GetInsertProcedureName(BaseEntity entity);
        string GetUpdateProcedureName(BaseEntity entity);
        string GetDeleteProcedureName(BaseEntity entity);

        Task<T> ExecuteScalarAsyncUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);
        T ExecuteScalarUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);
        Task<T> ExecuteUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);

        #region BuildCommandText
        string BuildInsertCommandText<T>(List<T> entities, ref Dictionary<string, object> dic);
        string BuildUpdateCommandText<T>(List<T> entities, ref Dictionary<string, object> dic);
        string BuildDeleteCommandText<T>(List<T> entities, ref Dictionary<string, object> dic);
        string BuildInsertCommandText<T>(T entity, ref Dictionary<string, object> dic);
        string BuildUpdateCommandText<T>(T entity, ref Dictionary<string, object> dic);
        string BuildDeleteCommandText<T>(T entity, ref Dictionary<string, object> dic);
        #endregion
    }
}
