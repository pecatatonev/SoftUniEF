using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_Exercises
{
    public static class SqlQueries
    {
        public const string GetVillainsWithNumberOfMinions = @"SELECT [Name]
                                                        ,COUNT(*) AS TotalMinions
                                                        FROM Villains AS v
                                                        JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
                                                        GROUP BY Name
                                                        HAVING COUNT(*) > 3
                                                        ORDER BY TotalMinions";

        public const string GetVillainById = @"SELECT Name FROM Villains WHERE Id = @Id";
        public const string GetOrderedMinionsByVillainId = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum,
                                                             m.Name, 
                                                             m.Age
                                                           FROM MinionsVillains AS mv
                                                           JOIN Minions As m ON mv.MinionId = m.Id
                                                           WHERE mv.VillainId = @Id
                                                           ORDER BY m.Name";

        public const string GetTownByName = @"SELECT Id FROM Towns WHERE Name = @townName";
        public const string GetVillainByName = @"SELECT Id FROM Villains WHERE Name = @villainName";
        public const string GetUpdatedTownsUpper = @"SELECT t.Name 
                                                      FROM Towns as t
                                                      JOIN Countries AS c ON c.Id = t.CountryCode
                                                      WHERE c.Name = @countryName";
        public const string GetAllMinions = @"SELECT Name FROM Minions";

        public const string InsertNewTown = @"INSERT INTO Towns([Name]) OUTPUT inserted.Id VALUES(@townName)";
        public const string InsertNewVillain = @"INSERT INTO Villains (Name, EvilnessFactorId) OUTPUT inserted.Id VALUES (@villainName, @evilnessFactorId)";
        public const string InsertNewMinion = @"INSERT INTO Minions (Name, Age, TownId) OUTPUT inserted.Id VALUES (@minionName, @minionAge, @minionTownId)";
        public const string InsertIntoMinionsVillains = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

        public const string UpdateTownsCastingUpper = @"UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
    }
}
