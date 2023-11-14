using ADO.NET_Exercises;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;

namespace AdoNetEx
{
    internal class Program
    {
        const string _connectionString = @"Server = DESKTOP-DU65TFH; Database=MinionsDB; Integrated Security = True";
        static SqlConnection connection;
        static async Task Main(string[] args)
        {
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                //GetVillainsWith3OrMoreMinions();

                //GetOrderedMinionsByVillainId(2);

                //string minionInfoRaw = Console.ReadLine();
                //string villainInfoRaw = Console.ReadLine();

                //string minionInfo = minionInfoRaw.Substring(minionInfoRaw.IndexOf(":") + 1).Trim();
                //string villainName = villainInfoRaw.Substring(villainInfoRaw.IndexOf(":") + 1).Trim();

                //await AddMinion(minionInfo, villainName);

                //string countryName = Console.ReadLine();
                //await ChangeTownNameCasting(countryName);

                PrintAllMinionsName();
            }
            finally { connection?.Close(); }
        }

        //2.Villain Name
        static void GetVillainsWith3OrMoreMinions()
        {
            using SqlCommand getVillainsCommand = new SqlCommand(SqlQueries.GetVillainsWithNumberOfMinions, connection);

            using SqlDataReader sqlDataReader = getVillainsCommand.ExecuteReader();

            while (sqlDataReader.Read())
            {
                Console.WriteLine($"{sqlDataReader["Name"]} - {sqlDataReader["TotalMinions"]}");
            }
        }

        //3. Minion Names
        static async Task GetOrderedMinionsByVillainId(int id)
        {
            using SqlCommand command = new SqlCommand(SqlQueries.GetVillainById, connection);
            command.Parameters.AddWithValue("@Id", id);

            var result = await command.ExecuteScalarAsync();

            if (result is null)
            {
                await Console.Out.WriteLineAsync($"No villain with ID {id} exist in the database");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Villain: {result}");

                using SqlCommand sqlCommand = new SqlCommand(SqlQueries.GetOrderedMinionsByVillainId, connection);
                sqlCommand.Parameters.AddWithValue("@Id", id);

                var result2 = await sqlCommand.ExecuteReaderAsync();

                while (await result2.ReadAsync())
                {
                    if (result2 is null)
                    {
                        await Console.Out.WriteLineAsync("(no minions)");
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync($"{result2["RowNum"]}. {result2["Name"]} {result2["Age"]}");
                    }
                }

            }
        }

        //4. Add Minion
        static async Task AddMinion(string minionInfo, string villainName)
        {
            string[] minionData = minionInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string minionName = minionData[0];
            int minionAge = int.Parse(minionData[1]);
            string minionTown = minionData[2];

            #region Town
            using SqlCommand cmdGetTownId = new SqlCommand(SqlQueries.GetTownByName, connection);
            cmdGetTownId.Parameters.AddWithValue("@townName", minionTown);

            var townResult = await cmdGetTownId.ExecuteScalarAsync();

            int townId = -1;
            if (townResult is null)
            {
                using SqlCommand createTown = new SqlCommand(SqlQueries.InsertNewTown, connection);
                townId = Convert.ToInt32(await createTown.ExecuteScalarAsync());
                createTown.Parameters.AddWithValue("@townName", minionTown);
                await Console.Out.WriteLineAsync($"Town {minionTown} was added to the database");
            }
            else {
                townId = (int)townResult;
            }

            #endregion

            #region Villain

            using SqlCommand cmdGetVillain = new SqlCommand(SqlQueries.GetVillainByName, connection);
            cmdGetVillain.Parameters.AddWithValue("@villainName", villainName);

            var villainResult = await cmdGetVillain.ExecuteScalarAsync();

            int villainId = -1;
            if (villainResult is null)
            {
                using SqlCommand insertVillain = new SqlCommand(SqlQueries.InsertNewVillain, connection);
                insertVillain.Parameters.AddWithValue("@villainName", villainName);
                insertVillain.Parameters.AddWithValue("@evilnessFactorId", 4);
                villainId = Convert.ToInt32(await insertVillain.ExecuteScalarAsync());
                await Console.Out.WriteLineAsync($"Villain {villainName} was added to the database.");
            }
            else {
                villainId = (int)villainResult;
            }
            #endregion

            #region Minion
            using SqlCommand cmdInsertMinion = new SqlCommand(SqlQueries.InsertNewMinion, connection);
            cmdInsertMinion.Parameters.AddWithValue("@minionName", minionName);
            cmdInsertMinion.Parameters.AddWithValue("@minionAge", minionAge);
            cmdInsertMinion.Parameters.AddWithValue("@minionTownId", townId);

            int minionId = Convert.ToInt32(await cmdInsertMinion.ExecuteScalarAsync());

            using SqlCommand cmdInsertMinionsVillains = new SqlCommand(SqlQueries.InsertIntoMinionsVillains, connection);
            cmdInsertMinionsVillains.Parameters.AddWithValue("@minionId", minionId);
            cmdInsertMinionsVillains.Parameters.AddWithValue("@villainId", villainId);
            await cmdInsertMinionsVillains.ExecuteNonQueryAsync();
            await Console.Out.WriteLineAsync($"Successfully added {minionName} to be minion of {villainName}.");
        #endregion
        }

        //5. Change Town Names Casing

        static async Task ChangeTownNameCasting(string countryName) {
            using SqlCommand command = new SqlCommand(SqlQueries.UpdateTownsCastingUpper, connection);
            command.Parameters.AddWithValue("@countryName", countryName);

            var result1 = await command.ExecuteNonQueryAsync();

            using SqlCommand sqlcmd = new SqlCommand(SqlQueries.GetUpdatedTownsUpper, connection);
            sqlcmd.Parameters.AddWithValue("@countryName", countryName);

            var result = await sqlcmd.ExecuteReaderAsync();
            List<string> towns = new List<string>();
            int count = 0;
            if (result is null)
            {
                await Console.Out.WriteLineAsync("No town names were affected.");
            }
            else { 
                while (await result.ReadAsync())
                {
                    count++;
                    towns.Add(result["Name"].ToString());
                }

                await Console.Out.WriteLineAsync($"{count} town names were affected.");
                await Console.Out.WriteLineAsync($"[{String.Join(", ",towns)}]");
            }
        }

        //7. Print All Minions Name
        static void PrintAllMinionsName() { 
            SqlCommand sqlCommand = new SqlCommand(SqlQueries.GetAllMinions, connection);

            var result =sqlCommand.ExecuteReader();
            List<string> minions = new List<string>();
            while (result.Read()) {
                minions.Add(result["Name"].ToString());
            }
            List<string> newOrderMinions = new List<string>();
            for (int i = 0; i < minions.Count/2; i++)
            {
                newOrderMinions.Add(minions[i].ToString());
                newOrderMinions.Add(minions[minions.Count - i - 1].ToString());
            }
            Console.WriteLine(string.Join(" ", minions));
            Console.WriteLine(string.Join(" ", newOrderMinions));
        }
    }
}