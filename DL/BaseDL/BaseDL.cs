using Common.Entities;
using Dapper;
using Ecommerce.Common.Constants;
using Ecommerce.DL;
using ECommerce.Common.Attributes;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DL
{
    public class BaseDL : IBaseDL
    {
        protected IDbConnection? mySqlConnection;

        public bool DeleteEntities<T>(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>()
        {

            
            string storedProcedure = String.Format(Procedure.GET_ALL, typeof(T).Name);
            OpenDB();
            var result = mySqlConnection.Query<T>(storedProcedure, commandType: CommandType.StoredProcedure);
            CloseDB();
            return result;
        }
        #region Get
        public virtual T GetByID<T>(int ID)
        {
            throw new NotImplementedException();
        }

        public virtual T GetByID<T>(string ID)
        {
            throw new NotImplementedException();
        }


       
        #endregion

        public IEnumerable<BaseEntity> GetAllBase()
        {
            return new List<BaseEntity>() {
            new BaseEntity() { CreatedDate = DateTime.Now, CreatedBy = "Long" },
            new BaseEntity() { CreatedDate = DateTime.Now, CreatedBy = "Mike" },
            new BaseEntity() { CreatedDate = DateTime.Now, CreatedBy = "John" },
            new BaseEntity() { CreatedDate = DateTime.Now, CreatedBy = "Tony"}
            };
        }

        public IEnumerable<T> GetByFilter<T>(string filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Khởi tạo và mở connection tới database
        /// </summary>
        public void OpenDB()
        {
            mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString);
            mySqlConnection.Open();
        }

        /// <summary>
        /// Đóng connection tới database
        /// </summary>
        public void CloseDB()
        {
            if (mySqlConnection != null)
            {
                mySqlConnection.Close();
            }
        }

        public async Task<int> ExecuteAsyncUsingStoredProcedure(string storedProcedure, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.ExecuteAsync(storedProcedure, commandType: CommandType.StoredProcedure, param: parameters, transaction: transaction);
            return result;
        }

        public IDbConnection GetDbConnection(string connectionString) 
        {
            if (connectionString != null)
            {
                return new MySqlConnection(connectionString);
            }
            return new MySqlConnection(DatabaseContext.ConnectionString);
        }

        public T UpdateOneByID<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntities<T>(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges<T>(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public string GetInsertProcedureName(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        public string GetUpdateProcedureName(BaseEntity entity)
        {
            var tableConfig = entity.GetType().GetCustomAttributes<TableConfigAttribute>(true).FirstOrDefault();
            if(tableConfig != null)
            {
                if (!string.IsNullOrWhiteSpace(tableConfig.UpdateStoredProcedure))
                {
                    return tableConfig.UpdateStoredProcedure;
                }else if (!string.IsNullOrWhiteSpace(tableConfig.TableName))
                {
                    var tableName = tableConfig.TableName;
                    string prefix = tableConfig.StoreProcedurePrefixName ?? string.Empty ;
                    return string.Format(Procedure.INSERT, tableName);

                }
            }
            return null;
        }

        public string GetDeleteProcedureName(BaseEntity entity)
        {
            var tableConfig = entity.GetType().GetCustomAttributes<TableConfigAttribute>(true).FirstOrDefault();
            if (tableConfig != null)
            {
                if (!string.IsNullOrWhiteSpace(tableConfig.DeleteStoredProcedure))
                {
                    return tableConfig.DeleteStoredProcedure;
                }
                else if (!string.IsNullOrWhiteSpace(tableConfig.TableName))
                {
                    var tableName = tableConfig.TableName;
                    string prefix = tableConfig.StoreProcedurePrefixName ?? string.Empty;
                    return string.Format(Procedure.DELETE_BY_ID, tableName);

                }
            }
            return null;
        }

        public async Task<bool> ExecuteUsingStoredProcedure(string storedProcedure, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.ExecuteAsync(storedProcedure, parameters, transaction, commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<T> ExecuteScalarUsingStoredProcedure<T>(string storedProcedure, IDictionary<string, object> parameters, IDbConnection connection, IDbTransaction transaction)
        {
            T result = default(T);
            var cd = new CommandDefinition();

            try
            {
                var con = transaction != null ? transaction.Connection : connection;
                if (con != null)
                {
                    result = await con.ExecuteScalarAsync<T>(storedProcedure, parameters, transaction);
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
