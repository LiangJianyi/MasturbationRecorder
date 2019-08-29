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
                    String sql = "SELECT TRIM(UserName), TRIM(Password), PersonData FROM dbo.MasturbationRecorderUser " +
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


        /*
         * Reference: https://stackoverflow.com/a/5371286/10975306
         */
        static byte[] ReadBinaryDataFromSqlServer(SqlConnectionStringBuilder builder, string outputPath) {
            SqlConnection connect = new SqlConnection(builder.ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT PersonData FROM dbo.MasturbationRecorderUser WHERE UserName='Janyee Liang'", connect);

            // Create a file to hold the output.
            // Writes the BLOB to a file (*.png).
            FileStream fileStream = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
            // Streams the BLOB to the FileStream object.
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);

            int bufferSize = int.MaxValue / 100;                  // Size of the BLOB buffer.
            int columnIndex = 0;                            // Index within the column of SQL Server.
            byte[] outbyte = new byte[bufferSize];          // The BLOB byte[] buffer to be filled by GetBytes.
            long retval;                                    // The bytes returned from GetBytes.
            long startIndex = 0;                            // The starting position in the BLOB output.

            // Open the connection and read data into the DataReader.
            connect.Open();
            SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

            while (myReader.Read()) {
                // Reset the starting byte for the new BLOB.
                startIndex = 0;

                // Read the bytes into outbyte[] and retain the number of bytes returned.
                retval = myReader.GetBytes(columnIndex, startIndex, outbyte, 0, bufferSize);

                // Continue reading and writing while there are bytes beyond the size of the buffer.
                while (retval == bufferSize) {
                    binaryWriter.Write(outbyte);
                    binaryWriter.Flush();

                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = myReader.GetBytes(1, startIndex, outbyte, 0, bufferSize);
                }

                // Write the remaining buffer.
                if (retval > 0) // if file size can divide to buffer size
                    binaryWriter.Write(outbyte, 0, (int)retval); //original MSDN source had retval-1, a bug
                binaryWriter.Flush();

                // Close the output file.
                binaryWriter.Close();
                fileStream.Close();
            }

            // Close the reader and the connection.
            myReader.Close();
            connect.Close();

            return outbyte;
        }
    }
}
