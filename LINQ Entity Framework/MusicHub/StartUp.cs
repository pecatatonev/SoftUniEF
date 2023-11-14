namespace MusicHub
{
    using System;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(p => p.ProducerId == producerId)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    AlbumReleaseDate = a.ReleaseDate,
                    AlbumProducerName = a.Producer.Name,
                    AlbumSongs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price,
                        SongWriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.SongWriterName).ToList(),
                    AlbumTotalPrice = a.Songs.Sum(p => p.Price),
                }).OrderByDescending(a => a.AlbumSongs.Sum(p => p.SongPrice)).ToList();

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var album in albums)
            {
                stringBuilder.AppendLine($"-AlbumName: {album.AlbumName}");
                stringBuilder.AppendLine($"-ReleaseDate: {album.AlbumReleaseDate.ToString("MM/dd/yyyy")}");
                stringBuilder.AppendLine($"-ProducerName: {album.AlbumProducerName}");
                stringBuilder.AppendLine($"-Songs:");
                int counter = 1;
                if (album.AlbumSongs.Any())
                {

                    foreach (var song in album.AlbumSongs)
                    {
                        stringBuilder.AppendLine($"---#{counter++}");
                        stringBuilder.AppendLine($"---SongName: {song.SongName}");
                        stringBuilder.AppendLine($"---Price: {song.SongPrice:f2}");
                        stringBuilder.AppendLine($"---Writer: {song.SongWriterName}");
                    }
                }
                stringBuilder.AppendLine($"-AlbumPrice: {album.AlbumTotalPrice:f2}");
            }
            return stringBuilder.ToString().Trim();
        }
        //public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        //{
        //    var albumInfo = context.Producers
        //        .Include(producer => producer.Albums)
        //            .ThenInclude(album => album.Songs)
        //                .ThenInclude(song => song.Writer)
        //        .FirstOrDefault(p => p.Id == producerId)!
        //        .Albums.Select(x => new
        //        {
        //            AlbumName = x.Name,
        //            x.ReleaseDate,
        //            ProducerName = x.Producer?.Name,
        //            AlbumSongs = x.Songs.Select(s => new
        //            {
        //                SongName = s.Name,
        //                SongPrice = s.Price,
        //                SongWriterName = s.Writer.Name
        //            })
        //            .OrderByDescending(x => x.SongName)
        //                .ThenBy(x => x.SongWriterName),
        //            TotalAlbumPrice = x.Price
        //        }).OrderByDescending(x => x.TotalAlbumPrice).AsEnumerable();

        //    StringBuilder stringBuilder = new();

        //    foreach (var album in albumInfo)
        //    {
        //        stringBuilder
        //            .AppendLine($"-AlbumName: {album.AlbumName}")
        //            .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}")
        //            .AppendLine($"-ProducerName: {album.ProducerName}")
        //            .AppendLine("-Songs:");

        //        int counter = 1;
        //        foreach (var song in album.AlbumSongs)
        //        {
        //            stringBuilder
        //                .AppendLine($"---#{counter++}")
        //                .AppendLine($"---SongName: {song.SongName}")
        //                .AppendLine($"---Price: {song.SongPrice:f2}")
        //                .AppendLine($"---Writer: {song.SongWriterName}");
        //        }
        //        stringBuilder
        //            .AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
        //    }

        //    return stringBuilder.ToString().TrimEnd();
        //}

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var span = new TimeSpan(0, 0, duration);
            var songs = context.Songs
                .Include(s => s.SongPerformers)
                .ThenInclude(sp => sp.Performer)
                .Include(s => s.Writer)
                .Include(s => s.Album)
                .ThenInclude(a => a.Producer)
                .AsEnumerable()
                .Where(s => s.Duration > span)
                .Select(s => new { 
                    SongName = s.Name,
                    PerformersFullName = s.SongPerformers.Select(sp => sp.Performer.FirstName + " " +sp.Performer.LastName)
                    .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c"),
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToList();
               

            StringBuilder stringBuilder = new StringBuilder();
            int counter = 0;
            foreach (var song in songs) {
                stringBuilder.AppendLine($"-Song #{++counter}");
                stringBuilder.AppendLine($"---SongName: {song.SongName}");
                stringBuilder.AppendLine($"---Writer: {song.WriterName}");
                if (song.PerformersFullName.Any())
                {
                    stringBuilder.AppendLine(string
                        .Join(Environment.NewLine, song.PerformersFullName
                            .Select(p => $"---Performer: {p}").OrderBy(p => p).ToList()));

                }
                stringBuilder.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                stringBuilder.AppendLine($"---Duration: {song.Duration}");
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
