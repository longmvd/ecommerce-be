﻿using Common.Entities;
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
        #region GET
        Task<IEnumerable<T>> GetAll<T>(Type type) where T : BaseEntity;

        Task<T> GetByID<T>(Type type, string id) where T : BaseEntity;

        Task<PagingResponse> GetPagingAsync(Type type, PagingRequest pagingRequest);

        #endregion

        //Task<IEnumerable<object>> GetAll(Type type);

        IEnumerable<BaseEntity> GetAllBase();

        #region Save
        ServiceResponse Save(BaseEntity entity);

        ServiceResponse SaveChanges(BaseEntity entity);

        ServiceResponse SaveList(IEnumerable<BaseEntity> entities);

        Task<ServiceResponse> SaveAsync(BaseEntity entity);

        Task<ServiceResponse> SaveChangesAsync(BaseEntity entity);

        Task<ServiceResponse> SaveChangesAsync(BaseEntity entity, List<EntityFieldUpdate> fieldUpdates);

        Task<ServiceResponse> SaveListAsync(IEnumerable<BaseEntity> entities);
        #endregion

        #region Update
        Task<ServiceResponse> UpdateOneAsync(BaseEntity entity);
        #endregion


        string GetInsertProcedureName(BaseEntity entity);
        string GetUpdateProcedureName(BaseEntity entity);
        string GetDeleteProcedureName(BaseEntity entity);

        Task<T> ExecuteScalarAsyncUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);
        T ExecuteScalarUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);
        Task<T> ExecuteUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param);

        Task<T> ExecuteScalarAsyncUsingCommandText<T>(string commandText, IDbTransaction transaction, object param);
        Task<T> ExecuteScalarAsyncUsingCommandText<T>(string commandText, object param);



        #region Query Async
        Task<IEnumerable<T>> QueryAsyncUsingCommandText<T>(string commandText, IDictionary<string, object>? parameters, IDbConnection connection = null, IDbTransaction transaction = null);

        Task<IEnumerable<object>> QueryAsyncUsingCommandText(Type type, string commandText, IDictionary<string, object> parameters, IDbConnection connection = null, IDbTransaction transaction = null);

        Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsyncUsingCommandText(List<Type> types, string commandText, IDictionary<string, object> parameters, IDbConnection connection = null, IDbTransaction transaction = null);
        #endregion

        #region BuildCommandText
        string BuildInsertCommandText<T>(List<T> entities, ref Dictionary<string, object> dic, bool useRowCount);
        string BuildUpdateCommandText<T>(List<T> entities, ref Dictionary<string, object> dic, bool useRowCount);
        string BuildDeleteCommandText<T>(List<T> entities, ref Dictionary<string, object> dic, bool useRowCount);
        string BuildInsertCommandText<T>(T entity, ref Dictionary<string, object> dic);
        string BuildUpdateCommandText<T>(T entity, ref Dictionary<string, object> dic);
        string BuildDeleteCommandText<T>(T entity, ref Dictionary<string, object> dic);
        #endregion
    }
}
