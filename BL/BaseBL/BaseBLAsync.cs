using Common.Entities;
using ECommerce.Common;
using ECommerce.Common.DTO;
using ECommerce.Common.Entities;
using ECommerce.Common.Enums;
using ECommerce.Common.Extension;
using ECommerce.Common.Resources;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Dapper.SqlMapper;

namespace ECommerce.BL
{
    public partial class BaseBL : IBaseBL
    {

        public virtual async Task<T> GetByID<T>(Type type, string id) where T : BaseEntity
        {
            var instance = (BaseEntity)Activator.CreateInstance(type);
            var keyName = instance.GetKeyProperty()?.Name;
            if (keyName != null)
            {
                var pagingRequset = new PagingRequest() { CustomFilter = $"[\"{keyName}\", \"=\", \"{id}\"]" };
                var result = (await GetPagingAsync(type, pagingRequset)).PageData as IEnumerable<T>;
                return (T)result.FirstOrDefault();
            }
            return default(T);
        }

        public virtual async Task<ServiceResponse> SaveAsync(BaseEntity entity)
        {
            var response = new ServiceResponse();
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {

                //validate
                var validateResult = new List<ValidateResult>();

                if (validateResult.Count > 0)
                {
                    response.IsSuccess = false;
                    return response;

                }

                //before save
                this.BeforeSave(entity);

                connection = _baseDL.GetDbConnection(null);
                connection.Open();
                transaction = connection.BeginTransaction();

                //save
                var result = await this.DoSaveAsync(entity, transaction);

                if (result)
                {
                    this.AfterSave(entity, transaction);
                    transaction.Commit();

                    //log
                }
                else
                {
                    transaction.Rollback();
                    response.IsSuccess = false;
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    response.IsSuccess = false;

                }
                throw;

            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            if (response.IsSuccess)
            {
                this.AfterCommit(entity, response);
            }

            return response;
        }

        public virtual async Task<ServiceResponse> SaveChangesAsync(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ServiceResponse> SaveChangesAsync(BaseEntity entity, List<EntityFieldUpdate> fieldUpdates)
        {
            var response = new ServiceResponse();
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {

                //validate


                //before save



                connection = _baseDL.GetDbConnection(null);
                connection.Open();
                transaction = connection.BeginTransaction();

                //save


                var result = await DoSaveChangesAsync(entity, fieldUpdates, transaction);

                if (result)
                {
                    this.AfterSaveChanges(entity, fieldUpdates, transaction);
                    transaction.Commit();

                    //log
                }
                else
                {
                    transaction.Rollback();
                    response.IsSuccess = false;
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    response.IsSuccess = false;

                }
                throw;

            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            if (response.IsSuccess)
            {
                this.AfterCommitSaveChanges(entity, fieldUpdates, response);
            }

            return response;


        }

        public virtual async Task<ServiceResponse> SaveListAsync(IEnumerable<BaseEntity> entities)
        {
            var response = new ServiceResponse();
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {

                //validate
                var validateResult = new List<ValidateResult>();

                if (validateResult.Count > 0)
                {
                    response.IsSuccess = false;
                    return response;

                }

                //before save
                this.BeforeSaveList(entities);

                connection = _baseDL.GetDbConnection(null);
                connection.Open();
                transaction = connection.BeginTransaction();

                //save
                var result = await this.DoSaveListAsync(entities, transaction);

                if (result)
                {
                    this.AfterSaveList(entities, transaction);
                    transaction.Commit();

                    //log
                }
                else
                {
                    transaction.Rollback();
                    response.IsSuccess = false;
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    response.IsSuccess = false;

                }
                throw;

            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            if (response.IsSuccess)
            {
                this.AfterCommitSaveList(entities, response);
            }

            return response;

        }

        public async virtual Task<bool> DoSaveAsync(BaseEntity entity, IDbTransaction transaction)
        {
            var dic = new Dictionary<string, object>();
            entity.SetAutoPrimaryKey();
            string storedProcedure = GetCommandTextByModelState(entity, ref dic);
            //var dic = Converter.ConvertDatabaseParam(entity);

            if (entity.GetPrimaryKeyType() == typeof(int))
            {
                var primaryKey = await ExecuteScalarAsyncUsingCommandText<int>(storedProcedure, transaction, dic);
                if (entity.State == ModelState.Insert || entity.State == ModelState.Dupplicate)
                {
                    entity.SetValueByAttribute(typeof(KeyAttribute), primaryKey);
                }
            }
            else
            {
                await this.ExecuteScalarAsyncUsingStoredProcedure<object>(storedProcedure, transaction, dic);
            }

            if (entity.EntityDetailConfigs?.Count > 0)
            {
                foreach (var config in entity.EntityDetailConfigs.Where(c => !string.IsNullOrWhiteSpace(c.PropertyNameOnMaster)))
                {
                    IList detailObjects = entity.Get<IList>(config.PropertyNameOnMaster);
                    if (detailObjects != null)
                    {
                        foreach (BaseEntity detailObject in detailObjects)
                        {
                            if (detailObject.State == ModelState.Insert ||
                                detailObject.State == ModelState.Update ||
                                detailObject.State == ModelState.Dupplicate)
                            {
                                detailObject.Set(config.ForeignKeyName, entity.GetValueOfPrimaryKey());
                                if (detailObject.State == ModelState.Insert ||
                                detailObject.State == ModelState.Dupplicate)
                                {
                                    detailObject.CreatedDate = DateTime.Now;
                                    detailObject.CreatedBy = _userName;
                                    if (detailObject.GetValueOfPrimaryKey() == null)
                                    {
                                        detailObject.SetAutoPrimaryKey();
                                    }
                                }
                                detailObject.ModifiedDate = DateTime.Now;
                                detailObject.ModifiedBy = _userName;
                                await DoSaveAsync(detailObject, transaction);
                            }
                            else if (detailObject.State == ModelState.Delete)
                            {
                                await DoDeleteAsync(detailObject, transaction);
                            }
                        }

                    }
                }
            }
            return true;
        }

        public virtual async Task<bool> DoSaveChangesAsync(BaseEntity entity, List<EntityFieldUpdate> fieldUpdates, IDbTransaction transaction)
        {
            var param = new Dictionary<string, object>();
            var sql = BuildUpdateFieldsCommandText(entity, fieldUpdates, ref param, true);
            return await this.ExecuteScalarAsyncUsingCommandText<bool>(sql, transaction, param);
        }

        public async virtual Task<bool> DoDeleteAsync(BaseEntity entity, IDbTransaction transaction)
        {
            var param = new Dictionary<string, object>();
            var query = BuildDeleteCommandText<BaseEntity>(entity, ref param);
            var isSuccess = false;
            if (query != null)
            {
                isSuccess = await ExecuteScalarAsyncUsingCommandText<bool>(query, transaction, param);
            }
            return isSuccess;
        }


        public virtual async Task<bool> DoSaveListAsync(IEnumerable<BaseEntity> entities, IDbTransaction transaction)
        {
            var isSuccess = false;
            foreach (var entity in entities)
            {
                isSuccess = await DoSaveAsync(entity, transaction);
                if (!isSuccess)
                {
                    break;
                }
            }
            return isSuccess;
        }

        public virtual void BeforeSaveList(IEnumerable<BaseEntity> entities)
        {
            // TODOS

        }

        public virtual void AfterCommitSaveList(IEnumerable<BaseEntity> entities, ServiceResponse response)
        {
            // TODOS

        }

        public virtual void AfterSaveList(IEnumerable<BaseEntity> entities, IDbTransaction transaction)
        {

        }


        public virtual async Task<T> ExecuteScalarAsyncUsingCommandText<T>(string commandText, IDbTransaction transaction, object param)
        {
            return await this.ExecuteScalarAsyncUsingCommandText<T>(commandText, (IDictionary<string, object>)param, null, transaction);
        }

        public virtual async Task<T> ExecuteScalarAsyncUsingCommandText<T>(string commandText, object param)
        {
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            var result = default(T);
            try
            {
                connection = _baseDL.GetDbConnection(null);
                connection.Open();
                transaction = connection.BeginTransaction();
                result = await this.ExecuteScalarAsyncUsingCommandText<T>(commandText, (IDictionary<string, object>)param, connection, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }
                throw;

            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return result;

        }


        public virtual async Task<T> ExecuteScalarAsyncUsingCommandText<T>(string commandText, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction)
        {
            return await _baseDL.ExecuteScalarAsyncUsingCommandText<T>(commandText, parameters, connection, transaction);
        }

        private string GetCommandTextByModelState(BaseEntity entity, ref Dictionary<string, object> param)
        {
            switch (entity.State)
            {
                case ModelState.Insert:
                    return BuildInsertCommandText<BaseEntity>(entity, ref param);

                case ModelState.Update:
                    return BuildUpdateCommandText<BaseEntity>(entity, ref param);

                case ModelState.Delete:
                    return BuildDeleteCommandText<BaseEntity>(entity, ref param);
                default:
                    return BuildInsertCommandText<BaseEntity>(entity, ref param);

            }
        }


        /// <summary>
        /// Hàm đang phát triển 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual async Task<bool> DoSaveListAsyncPrototype(IEnumerable<BaseEntity> entities, IDbTransaction transaction)
        {
            var isSuccess = false;
            var listForInsert = new List<BaseEntity>(entities.Where(entity => entity.State == ModelState.Insert));
            var listForUpdate = new List<BaseEntity>(entities.Where(entity => entity.State == ModelState.Update));
            var listForDelete = new List<BaseEntity>(entities.Where(entity => entity.State == ModelState.Delete));
            var dic = new Dictionary<string, object>();
            var insertStatement = BuildInsertCommandText(listForInsert, ref dic, true);
            if (!string.IsNullOrWhiteSpace(insertStatement))
            {
                isSuccess = true;
                var insertedRows = await ExecuteScalarAsyncUsingCommandText<int>(insertStatement, transaction, dic);
                if (insertedRows == listForInsert.Count)
                {
                    foreach (var entity in listForInsert)
                    {
                        if (entity.EntityDetailConfigs?.Count > 0)
                        {
                            foreach (var config in entity.EntityDetailConfigs.Where(c => !string.IsNullOrWhiteSpace(c.PropertyNameOnMaster)))
                            {
                                IEnumerable<BaseEntity> detailObjects = entity.Get<IEnumerable<BaseEntity>>(config.PropertyNameOnMaster);
                                if (detailObjects != null)
                                {
                                    foreach (BaseEntity detailObject in detailObjects)
                                    {
                                        BeforeSave(detailObject);

                                    }
                                    await DoSaveListAsync(detailObjects, transaction);

                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            var updateStatement = BuildUpdateCommandText(listForUpdate, ref dic, true);
            if (!string.IsNullOrWhiteSpace(updateStatement))
            {
                isSuccess = true;
                var updatedRows = await ExecuteScalarAsyncUsingCommandText<int>(updateStatement, transaction, dic);
                if (updatedRows == listForUpdate.Count)
                {
                    foreach (var entity in listForUpdate)
                    {
                        if (entity.EntityDetailConfigs?.Count > 0)
                        {
                            foreach (var config in entity.EntityDetailConfigs.Where(c => !string.IsNullOrWhiteSpace(c.PropertyNameOnMaster)))
                            {
                                IEnumerable<BaseEntity> detailObjects = entity.Get<IEnumerable<BaseEntity>>(config.PropertyNameOnMaster);
                                if (detailObjects != null)
                                {
                                    foreach (BaseEntity detailObject in detailObjects)
                                    {
                                        BeforeSave(detailObject);

                                    }
                                    await DoSaveListAsync(detailObjects, transaction);

                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }

            }

            return isSuccess;
        }

        #region Get
        public virtual async Task<IEnumerable<T>> GetAll<T>(Type type) where T : BaseEntity
        {
            var res = default(IEnumerable<T>);
            var entity = (T)Activator.CreateInstance(type);
            var query = BaseEntityQuery.BaseBL_GetAll;
            query = string.Format(query, entity.GetTableConfig().TableName);
            try
            {
                using IDbConnection connection = _baseDL.GetDbConnection(null);
                var entities = await _baseDL.QueryAsyncUsingCommandText(type, query, null, connection);
                res = entities.OfType<T>();
            }
            catch
            {
                throw;
            }
            return res;
        }

        public virtual async Task<PagingResponse> GetPagingAsync(Type type, PagingRequest pagingRequest)
        {
            var response = new PagingResponse();
            var commandText = BaseEntityQuery.BaseBL_GetPaging;
            GetPagingCommandText(ref commandText);
            var instance = (BaseEntity)Activator.CreateInstance(type);
            var columns = "*";
            if (pagingRequest.Columns != null)
            {
                columns = SecurityUtils.GetColumns(pagingRequest.Columns, instance);

            }
            var param = new Dictionary<string, object>();
            var condition = BuildPagingCommandText(type, pagingRequest, ref param);
            var tableName = instance.GetTableConfig()?.TableName ?? instance.GetType().Name;
            commandText = string.Format(commandText, new object[] { columns, tableName, condition });
            var result = await QueryMultipleAsyncUsingCommandText(new List<Type>() { type, typeof(int) }, commandText, param);
            response.PageData = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(result?.ElementAt(0)), typeof(List<>).MakeGenericType(type));
            response.Total = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(result?.ElementAt(1).FirstOrDefault()));
            return response;
        }

        #endregion

        #region Query Async
        public async Task<IEnumerable<T>> QueryAsyncUsingCommandText<T>(string commandText, IDictionary<string, object>? parameters, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            IEnumerable<T>? res;
            try
            {
                if (connection == null)
                {
                    connection = _baseDL.GetDbConnection(null);
                }
                var entities = await _baseDL.QueryAsyncUsingCommandText<T>(commandText, parameters, connection, transaction);
                res = entities.OfType<T>();
                connection.Close();

            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                }
                throw;
            }
            return res;

        }

        public async Task<IEnumerable<object>> QueryAsyncUsingCommandText(Type type, string commandText, IDictionary<string, object> parameters, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            IEnumerable<object>? res;
            try
            {
                if (connection == null)
                {
                    connection = _baseDL.GetDbConnection(null);
                }
                res = await _baseDL.QueryAsyncUsingCommandText(type, commandText, parameters, connection, transaction);
                connection.Close();

            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                }
                throw;
            }
            return res;
        }

        public async Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsyncUsingCommandText(List<Type> types, string commandText, IDictionary<string, object> parameters, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            var res = await _baseDL.QueryMultipleAsyncUsingCommandText(types, commandText, parameters, connection, transaction);
            return res;
        }

        #endregion

        public async virtual Task<ServiceResponse> UpdateOneAsync(BaseEntity entity)
        {
            var response = new ServiceResponse();
            if (entity == null)
            {
                return response.OnError(Resource.DEV_NullRequestObject);
            }
            else
            {
                entity.State = ModelState.Update;
            }
            return await this.SaveAsync(entity);
        }

        public virtual void GetPagingCommandText(ref string commandText)
        {
            // to do assign command text
        }

    }
}
