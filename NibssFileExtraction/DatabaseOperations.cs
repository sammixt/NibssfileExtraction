using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NibssFileExtraction
{
    class DatabaseOperations
    {
        private string connectionString ;

        Logger log;
        public DatabaseOperations(Logger _log)
        {
            log = _log;
            setconnstring();
        }
        public bool InsertRecord(DataTable table,string tableName)
        {
            int index = 0;
            bool output = false;
            log.Info("Inserting record to Database");
            if (!IsTableExist(connectionString, tableName))
            {
                string str = " CREATE TABLE [dbo].[" + tableName + "]( ";
                foreach (DataColumn column2 in (InternalDataCollectionBase)table.Columns)
                    str = str + "[" + column2.ToString() + "] [varchar](max) NULL,";
                string query = str + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                CreateDatabaseTable(connectionString, query);
            }
            Console.WriteLine("checking if entries exist. ");
            string[] columnsName = getColumnsName(connectionString, tableName);
            bool flag = false;
            if ((uint)columnsName.Length > 0U)
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionString))
                {
                    foreach (DataColumn column2 in (InternalDataCollectionBase)table.Columns)
                    {
                        if (columnsName[index].ToString() == column2.ColumnName.ToString())
                        {
                            sqlBulkCopy.ColumnMappings.Add(column2.ColumnName.ToString(), column2.ColumnName.ToString());
                        }
                        else
                        {
                            log.Info("column mismatch");
                            flag = true;
                        }
                        ++index;
                    }
                    if (!flag)
                    {
                        //log.Info("Writing to Table");
                        sqlBulkCopy.BulkCopyTimeout = Convert.ToInt32("timeout".GetKeyValue());
                        sqlBulkCopy.DestinationTableName = tableName;
                        sqlBulkCopy.WriteToServer(table);
                        output = true;
                    }
                }
            }
            else
            {
                log.Info("This table has not been created in the DB. Create a table for records.");
                output = false;
                //Console.Read();
            }
            return output;
        }

        public string[] getColumnsName(string Connection, string tableName)
        {
            List<string> stringList = new List<string>();
            using (SqlConnection sqlConnection = new SqlConnection(Connection))
            {
                using (SqlCommand command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tableName + "'";
                    sqlConnection.Open();
                    using (SqlDataReader sqlDataReader = command.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                            stringList.Add(sqlDataReader.GetString(0));
                    }
                }
            }
            return stringList.ToArray();
        }

        public  DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {

            DataTable dt = new DataTable();


            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type IcolType = GetProperty.PropertyType;

                        if ((IcolType.IsGenericType) && (IcolType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            IcolType = IcolType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, IcolType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo p in columns)
                {
                    dr[p.Name] = p.GetValue(Record, null) == null ? DBNull.Value : p.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        private static bool IsTableExist(string connString, string table)
        {
            bool flag = false;
            string cmdText = "SELECT COUNT(*) as 'TableCount' " + "FROM INFORMATION_SCHEMA.TABLES " + " WHERE TABLE_SCHEMA = 'dbo' " + " AND TABLE_NAME = '" + table + "' ";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(cmdText, connection))
                {
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    flag = Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0;
                    connection.Close();
                }
            }
            return flag;
        }

        private void CreateDatabaseTable(string connString, string query)
        {
            string cmdText = query;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(cmdText, connection))
                {
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }


        public void TruncateTable(string tablename)
        {
            string cmdText = $"TRUNCATE TABLE {tablename}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(cmdText, connection))
                {
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private  void setconnstring()
        {
            connectionString = new SqlConnectionStringBuilder()
            {
                IntegratedSecurity = true,
                DataSource = "dbip".GetKeyValue(),
                //UserID = (appSettingsReader.GetValue("dbuser", typeof(string)) as string),
                //Password = (appSettingsReader.GetValue("dbpass", typeof(string)) as string),
                InitialCatalog = "db".GetKeyValue(),
                ConnectTimeout = 1
            }.ConnectionString;
        }
    }
}
