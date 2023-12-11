namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Invoices.Extensions;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creator = context.Creators
                .Where(c => c.Boardgames.Count > 0)
                .Select(c => new ExportCreatorDTO()
                { 
                    CreatorName = c.FirstName + " " + c.LastName,
                    BoardgamesCount = c.Boardgames.Count,
                    Boardgames = c.Boardgames
                    .Select(bg => new ExportBoardGameWithYearDTO() {
                        Name = bg.Name,
                        YearPublished = bg.YearPublished,
                    })
                    .OrderBy(bg => bg.Name)
                    .ToArray()
                })
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            return creator.SerializeToXml<ExportCreatorDTO[]>("Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating))
                .Select(s => new ExportSellerDTO
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                    .Where(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating)
                    .Select(bg => new ExportBoardGameDTO()
                    {
                        Name = bg.Boardgame.Name,
                        Rating = bg.Boardgame.Rating,
                        Mechanics = bg.Boardgame.Mechanics,
                        Category = bg.Boardgame.CategoryType,
                    })
                    .OrderByDescending(bg => bg.Rating)
                    .ThenBy(bg => bg.Name)
                    .ToArray()

                })
                .OrderByDescending(s => s.Boardgames.Count())
                .ThenBy(s => s.Name)
                .Take(5)
                .ToArray();

            return sellers.SerializeToJson<ExportSellerDTO[]>();
        }
    }
}