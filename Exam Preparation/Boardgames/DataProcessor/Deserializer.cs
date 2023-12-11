namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Invoices.Extensions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<ImportCreatorsDTO> creatorsList = xmlString.DeserializeFromXml <List<ImportCreatorsDTO>>("Creators");

            List<Creator> creators = new List<Creator>();

            foreach (var creator in creatorsList)
            {
                if (!IsValid(creator))
                {
                    stringBuilder.AppendLine(ErrorMessage);

                    continue;
                }

                var creatorToAdd = new Creator()
                {
                    FirstName = creator.FirstName,
                    LastName = creator.LastName,
                };

                foreach (var boardGame in creator.Boardgames)
                {
                    if (!IsValid(boardGame)) 
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    var boardGameToAdd = new Boardgame()
                    {
                        Name = boardGame.Name,
                        Rating = boardGame.Rating,
                        YearPublished = boardGame.YearPublished,
                        CategoryType = (CategoryType)boardGame.CategoryType,
                        Mechanics = boardGame.Mechanics
                    };

                    creatorToAdd.Boardgames.Add(boardGameToAdd);
                }

                creators.Add(creatorToAdd);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedCreator, creatorToAdd.FirstName, creatorToAdd.LastName, creatorToAdd.Boardgames.Count));
            }

            context.Creators.AddRange(creators);
            context.SaveChanges();

            return stringBuilder.ToString();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<ImportSellerDTO> sellerDto = jsonString.DeserializeFromJson<List<ImportSellerDTO>>();

            List<Seller> sellers = new List<Seller>();

            int[] boardGamesIds = context.Boardgames.Select(x => x.Id).ToArray();

            foreach (var seller in sellerDto)
            {
                if (!IsValid(seller)) 
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                var sellerToAdd = new Seller()
                {
                    Name = seller.Name,
                    Address = seller.Address,
                    Country = seller.Country,
                    Website = seller.Website,
                };

                foreach (var boardGameId in seller.BoardgameIds.Distinct())
                {
                    if (boardGamesIds.Contains(boardGameId))
                    {
                        sellerToAdd.BoardgamesSellers.Add(new BoardgameSeller()
                        {
                            BoardgameId = boardGameId,
                        });
                    }
                    else 
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                    }
                }

                sellers.Add(sellerToAdd);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedSeller, sellerToAdd.Name, sellerToAdd.BoardgamesSellers.Count()));
            }

            context.AddRange(sellers);
            context.SaveChanges();

            return stringBuilder.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
