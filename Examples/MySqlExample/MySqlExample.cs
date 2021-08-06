using MySqlConnector;
using Runtime.Script;
using System;
using System.Drawing;
using System.Threading.Tasks;
using TradeApi.Indicators;

namespace MySqlExample
{
    /// <summary>
    /// Indicator
    /// After complile copy MySqlExample.dll, MySqlConnector.dll, System.Threading.Tasks.Extensions.dll to folder Scripts\Indicators
    /// Somtimes occurrs exception with System.Memory.dll
    /// In this case try to edit TreadeTerminal.config to add redirect to newest version System.Memory.dll
    /// <dependentAssembly>
    ///     <assemblyIdentity name = "System.Memory" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral"/>
    ///     <bindingRedirect oldVersion = "0.0.0.0-4.0.1.1" newVersion="4.0.1.1"/>
    /// </dependentAssembly>
    /// </summary>
    public class MySqlExample : IndicatorBuilder
    {
        public MySqlExample()
            : base()
        {
            #region Initialization
            Credentials.Author = "";
            Credentials.Company = "";
            Credentials.Copyrights = "";
            Credentials.DateOfCreation = new DateTime(2021, 8, 3);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "Indicator";
            #endregion

            Lines.Set("line1");
            Lines["line1"].Color = Color.Blue;

            SeparateWindow = false;
        }

        /// <summary>
        /// This function will be called after creating
        /// </summary>
        public override void Init()
        {
            WorkingWithDatabase();
        }

        /// <summary>
        /// Entry point. This function is called when new quote comes or new bar created
        /// </summary>
        public override void Update(TickStatus args)
        {

        }

        /// <summary>
        /// This function will be called before removing
        /// </summary>
        public override void Complete()
        {

        }
        public async void WorkingWithDatabase()
        {
            //connection data
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = 3306,
                Database = "testdb",
                UserID = "root",
                Password = "root",
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Notification.Print("Opening connection");
                await conn.OpenAsync();

                /*working with the database here
                (reading, writing, etc.)*/
                await CreateTableAndInsertDataAsync(conn);
                await ReadDataFromDatabaseAsync(conn);
                await DeleteDataFromDatabaseAsync(conn);
                await ReadDataFromDatabaseAsync(conn);
                // connection will be closed by the 'using' block
                Notification.Print("Closing connection");
            }
        }

        public async Task CreateTableAndInsertDataAsync(MySqlConnection conn)
        {
            using (var command = conn?.CreateCommand())
            {
                //command to drop a table named "inventory" if it exists
                command.CommandText = "DROP TABLE IF EXISTS inventory;";
                //command execution
                await command.ExecuteNonQueryAsync();
                Notification.Print("Finished dropping table (if existed)");

                //command to create a table named "inventory"
                command.CommandText = "CREATE TABLE inventory (id serial PRIMARY KEY, name VARCHAR(50), quantity INTEGER);";
                //command execution
                await command.ExecuteNonQueryAsync();
                Notification.Print("Finished creating table");

                //command to insert an item into a table
                command.CommandText = @"INSERT INTO inventory (name, quantity) VALUES (@name1, @quantity1),
                        (@name2, @quantity2), (@name3, @quantity3);";

                //parameter substitution
                command.Parameters.AddWithValue("@name1", "banana");
                command.Parameters.AddWithValue("@quantity1", 150);
                command.Parameters.AddWithValue("@name2", "orange");
                command.Parameters.AddWithValue("@quantity2", 154);
                command.Parameters.AddWithValue("@name3", "apple");
                command.Parameters.AddWithValue("@quantity3", 100);

                //command execution
                int rowCount = await command.ExecuteNonQueryAsync();
                Notification.Print(String.Format("Number of rows inserted={0}", rowCount));
            }
        }

        public async Task ReadDataFromDatabaseAsync(MySqlConnection conn)
        {
            using (var command = conn.CreateCommand())
            {
                //command to select all data from a table
                command.CommandText = "SELECT * FROM inventory;";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    //read data until the method returns false
                    while (await reader.ReadAsync())
                    {
                        Notification.Print(string.Format(
                            "Reading from table=({0}, {1}, {2})",
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2)));
                    }
                }
            }
        }

        public async Task DeleteDataFromDatabaseAsync(MySqlConnection conn)
        {
            using (var command = conn.CreateCommand())
            {
                //command to delete all records where name = the value of the "@name" parameter
                command.CommandText = "DELETE FROM inventory WHERE name = @name;";

                //substitute the value in the parameter
                command.Parameters.AddWithValue("@name", "orange");

                //command execution
                int rowCount = await command.ExecuteNonQueryAsync();
                Notification.Print(String.Format("Number of rows deleted={0}", rowCount));
            }
        }
    }
}
