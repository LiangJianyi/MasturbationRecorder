using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
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
                    const string SQL = "SELECT TRIM(UserName), TRIM(Password), PersonData FROM dbo.MasturbationRecorderUser " +
                                       "WHERE UserName = @name AND Password = @pwd";

                    using (SqlCommand command = new SqlCommand(SQL, connection)) {
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

                        //return await command.ExecuteScalarAsync() != null;

                        SqlDataReader reader = await command.ExecuteReaderAsync();
                    }
                }
            }
            catch (SqlException e) {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async static Task<bool> Login(Configuration configuration) {
            bool status = false;
            using (SqlConnection connect = new SqlConnection(_builder.ConnectionString)) {
                const string SQL = "SELECT TRIM(UserName), TRIM(Password), PersonData FROM dbo.MasturbationRecorderUser " +
                                   "WHERE UserName = @name AND Password = @pwd";
                SqlCommand command = new SqlCommand(SQL, connect);
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

                connect.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess)) {
                    while (reader.Read()) {
                        byte[] bytes = reader.GetSqlBinary(2).Value;
                        try {
                            Windows.Storage.StorageFile file = await bytes.AsStorageFile("Status.png");
                            configuration.Avatar = file;
                        }
                        catch (AggregateException ex) {
                            Debug.WriteLine(ex.InnerException.Message);
                            throw;
                        }
                        status = true;
                    }
                }
            }
            return status;
        }
    }
}
