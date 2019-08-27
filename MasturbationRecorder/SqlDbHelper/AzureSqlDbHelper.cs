using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Threading.Tasks;

namespace MasturbationRecorder.SqlDbHelper {
    using Debug = System.Diagnostics.Debug;

    static class AzureSqlDbHelper {
        private static readonly string _dataSource = "jianyi.database.chinacloudapi.cn";
        private static readonly string _userID = "jianyi";
        private static readonly string _password = "{FYbteTx4hNU@7Z83+u)2t@QrtQ9^E8EYFBWv67mGbeifsk[BUWxhBL6GzA]Z$r(";
        private static readonly string _initialCatalog = "JianyiAzureDataBase";
        private static readonly SqlConnectionStringBuilder _builder = new SqlConnectionStringBuilder {
            DataSource = _dataSource,
            UserID = _userID,
            Password = _password,
            InitialCatalog = _initialCatalog
        };

        /// <summary>
        /// 登录账户
        /// </summary>
        /// <param name="configuration">接收应用程序的配置</param>
        /// <returns>登陆成功返回 true，否则返回 false</returns>
        public async static Task<bool> LoginAsync(Configuration configuration) {
            try {
                using (SqlConnection connection = new SqlConnection(_builder.ConnectionString)) {
                    connection.Open();
                    String sql = "SELECT TRIM(UserName), TRIM(Password) FROM dbo.MasturbationRecorderUser " +
                                 "WHERE UserName = @name AND Password = @pwd";

                    using (SqlCommand command = new SqlCommand(sql, connection)) {
                        // Create sql parameter @name
                        IDataParameter parameter_username = command.CreateParameter();
                        parameter_username.ParameterName = "name";
                        parameter_username.DbType = DbType.String;
                        parameter_username.Value = configuration.UserName;

                        // Create sql parameter @pwd
                        IDataParameter parameter_password = command.CreateParameter();
                        parameter_password.ParameterName = "pwd";
                        parameter_password.DbType = DbType.String;
                        parameter_password.Value = configuration.Password;

                        command.Parameters.Add(parameter_username);
                        command.Parameters.Add(parameter_password);

                        return await command.ExecuteScalarAsync() != null;
                    }
                }
            }
            catch (SqlException e) {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }
    }
}
