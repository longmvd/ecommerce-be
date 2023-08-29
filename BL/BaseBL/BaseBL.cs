using Common.Entities;
using ECommerce.Common.Attributes;
using ECommerce.Common.DTO;
using ECommerce.Common.Entities;
using ECommerce.Common.Enums;
using ECommerce.Common.Extension;
using ECommerce.DL;
using GraphQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BL
{

    public class BaseBL : IBaseBL
    {
        //protected IAuthService _authService;
        protected IBaseDL _baseDL;
        public BaseBL(IBaseDL baseDL)
        {
            _baseDL = baseDL;
        }

        protected string _userName;

        public IEnumerable<User> GetAll()
        {
            return _baseDL.GetAll<User>();
        }
        public ServiceResponse Save(BaseEntity entity)
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
                var result = this.DoSave(entity, transaction);

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

        public virtual void AfterCommit(BaseEntity entity, ServiceResponse response)
        {
            //todo
        }

        public virtual void AfterSave(BaseEntity entity, IDbTransaction transaction)
        {
            //todo anything after save successfully if needed
        }

        public virtual bool DoSave(BaseEntity entity, IDbTransaction transaction)
        {
            var dic = new Dictionary<string, object>();
            entity.SetAutoPrimaryKey();
            string storedProcedure = entity.State == ModelState.Update ? GetUpdateProcedureName(entity): this.BuildInsertCommandText<BaseEntity>(entity, ref dic);
            //var dic = Converter.ConvertDatabaseParam(entity);
            
            if (entity.GetPrimaryKeyType() == typeof(int))
            {
                var primaryKey = ExecuteScalarUsingStoredProcedure<int>(storedProcedure, transaction, dic);
                if (entity.State == ModelState.Insert || entity.State == ModelState.Dupplicate)
                {
                    entity.SetValueByAttribute(typeof(KeyAttribute), primaryKey);
                }
            }
            else
            {
                this.ExecuteScalarUsingStoredProcedure<object>(storedProcedure, transaction, dic);
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
                                detailObjects.Set(config.ForeignKeyName, entity.GetValueOfPrimaryKey());
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
                                DoSave(detailObject, transaction);
                            }
                            else if (detailObject.State == ModelState.Delete)
                            {
                                DoDelete(detailObject, transaction);
                            }
                        }

                    }
                }
            }
            return true;
        }

        private void DoDelete(BaseEntity detailObject, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public ServiceResponse SaveChanges(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        public ServiceResponse SaveList(IEnumerable<BaseEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {

        }


        public virtual void BeforeSave(BaseEntity entity)
        {
            if (entity.State == ModelState.Insert || entity.State == ModelState.Dupplicate)
            {
                entity.CreatedBy = _userName;
                entity.CreatedDate = DateTime.Now;
                if (entity.GetValueOfPrimaryKey() == null)
                {
                    entity.SetAutoPrimaryKey();
                }

            }

            entity.ModifiedBy = _userName;
            entity.ModifiedDate = DateTime.Now;
        }

        public string GetInsertProcedureName(BaseEntity entity)
        {
            return this._baseDL.GetInsertProcedureName(entity);
        }

        public string GetUpdateProcedureName(BaseEntity entity)
        {
            return this._baseDL.GetUpdateProcedureName(entity);
        }

        public string GetDeleteProcedureName(BaseEntity entity)
        {
            return this._baseDL.GetDeleteProcedureName(entity);
        }

        public T ExecuteScalarUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param)
        {

            return ExecuteScalarAsyncUsingStoredProcedure<T>(storedProcedureName, transaction, param).Result;

        }

        public Task<T> ExecuteUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param)
        {
            throw new NotImplementedException();
        }

        public async Task<T> ExecuteScalarAsyncUsingStoredProcedure<T>(string storedProcedureName, IDbTransaction transaction, object param)
        {
            return await _baseDL.ExecuteScalarUsingStoredProcedure<T>(storedProcedureName, (IDictionary<string, object>)param, null, transaction);
        }
        #region Build Command Text

        public string BuildInsertCommandText<T>(List<T> entities, ref Dictionary<string, object> dic)
        {
            StringBuilder builder = new StringBuilder();
            if (entities != null && entities.Count > 0)
            {
                var tableConfig = entities[0].GetType().GetCustomAttributes<TableConfigAttribute>(true).FirstOrDefault();
                var param = new Dictionary<string, object>();
                var commandParams = new List<string>();
                if (tableConfig != null)
                {
                    builder.Append($"INSERT INTO {tableConfig.TableName} ( ");

                    for (int i = 0; i < entities.Count; i++)
                    {
                        var entity = entities[i];
                        var properties = entity.GetType().GetProperties();
                        if (properties.Length > 0)
                        {
                            StringBuilder paramBuilder = new StringBuilder("(");
                            foreach (var property in properties)
                            {

                                if (property.GetCustomAttributes<NotMappedAttribute>(true).FirstOrDefault() != null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(property.GetValue(entity)?.ToString()))
                                    {
                                        var columnName = property.GetCustomAttributes<ColumnAttribute>(true).FirstOrDefault()?.Name;
                                        string stringParam = property.Name;
                                        if (columnName != null)
                                        {
                                            stringParam = columnName;
                                        }
                                        if (i == 0)
                                        {
                                            builder.Append(stringParam + ",");
                                        }
                                        dic.TryAdd("@" + stringParam + i, property.GetValue(entity));
                                        paramBuilder.Append("@" + stringParam + i + ",");
                                    }
                                }
                            }
                            //remove "," at the end of string builder
                            paramBuilder.Remove(paramBuilder.Length - 1, 1);
                            paramBuilder.Append(")");
                            commandParams.Add(paramBuilder.ToString());
                            if (i == 0)
                            {
                                builder.Remove(builder.Length - 1, 1);
                                builder.Append(") VALUES ");
                            }

                        }

                    }
                    foreach (var commandParam in commandParams)
                    {
                        builder.Append(commandParam + ",");
                    }
                    builder.Remove(builder.Length - 1, 1);
                    builder.Append(";");
                }
            }
            return builder.ToString();
        }

        public string BuildUpdateCommandText<T>(List<T> entities, ref Dictionary<string, object> dic)
        {
            throw new NotImplementedException();
        }

        public string BuildDeleteCommandText<T>(List<T> entities, ref Dictionary<string, object> dic)
        {
            throw new NotImplementedException();
        }

        public string BuildInsertCommandText<T>(T entity, ref Dictionary<string, object> dic)
        {
            return this.BuildInsertCommandText<T>(new List<T>() { entity }, ref dic);
        }

        public string BuildUpdateCommandText<T>(T entity, ref Dictionary<string, object> dic)
        {
            return BuildUpdateCommandText<T>(new List<T>() { entity }, ref dic);
        }

        public string BuildDeleteCommandText<T>(T entity, ref Dictionary<string, object> dic)
        {
            return BuildDeleteCommandText<T>(new List<T>() { entity }, ref dic);
        } 
        #endregion
    }
}
