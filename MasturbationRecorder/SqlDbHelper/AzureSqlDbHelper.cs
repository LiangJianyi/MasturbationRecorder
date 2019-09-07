﻿using System;
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
        /// 注册账户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="fileBytes">PersonData文件字节流</param>
        public static async Task<int> RegisterUserAsync(string username, string password, byte[] fileBytes) {
            try {
                using (SqlConnection connection = new SqlConnection(_builder.ConnectionString)) {
                    string commandText = "INSERT INTO dbo.MasturbationRecorderUser (UserName,Password,PersonData) " +
                                         "VALUES(@username, @pwd, @personData)";
                    using (SqlCommand cmd = new SqlCommand(commandText, connection)) {
                        // Create sql parameter @username
                        IDataParameter parameter_username = cmd.CreateParameter();
                        parameter_username.ParameterName = "username";
                        parameter_username.DbType = DbType.String;
                        parameter_username.Value = username;

                        // Create sql parameter @pwd
                        IDataParameter parameter_password = cmd.CreateParameter();
                        parameter_password.ParameterName = "pwd";
                        parameter_password.DbType = DbType.String;
                        parameter_password.Value = password;

                        // Create sql parameter @bytes
                        IDataParameter parameter_bytes = cmd.CreateParameter();
                        parameter_bytes.ParameterName = "personData";
                        parameter_bytes.DbType = DbType.Binary;
                        parameter_bytes.Value = fileBytes;

                        cmd.Parameters.Add(parameter_username);
                        cmd.Parameters.Add(parameter_password);
                        cmd.Parameters.Add(parameter_bytes);

                        await cmd.Connection.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException e) {
                Debug.WriteLine(e.ToString());
            }
            return -1;
        }

        /// <summary>
        /// 登录账户
        /// </summary>
        /// <param name="configuration">接收应用程序的配置</param>
        /// <returns>登陆成功返回 true，否则返回 false</returns>
        public async static Task<bool> LoginAsync(Configuration configuration) {
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

                await connect.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess)) {
                    while (reader.Read()) {
                        /*
                         * 使用临时变量 binary 的原因是如果调用两次 GetSqlBinary(2) 会抛出异常，
                         * 下面的代码会抛出 System.InvalidOperationException: 'Invalid attempt to read from column ordinal '2'.  
                         *  With CommandBehavior.SequentialAccess, you may only read from column ordinal '3' or greater.'
                         * 
                         * if (reader.GetSqlBinary(2).IsNull == false) {
                         *      byte[] bytes = reader.GetSqlBinary(2).Value;
                         *      configuration.Avatar = await bytes?.AsStorageFile("Status.png");
                         * }
                         */
                        System.Data.SqlTypes.SqlBinary binary = reader.GetSqlBinary(2);
                        if (binary.IsNull == false) {
                            byte[] bytes = binary.Value;
                            configuration.Avatar = await bytes?.AsStorageFile("Status.png");
                        }
                        status = true;
                    }
                }
            }
            return status;
        }
    }
}
