using AutoMapper;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        //have problems with connecting to database and its not finished
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            string inputUsersXml = File.ReadAllText("../../../Datasets/users.xml");

            Console.WriteLine(ImportUsers(context, inputUsersXml));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            ImportUsersDTO[] importUsersDTOs = DeserializeXml<ImportUsersDTO[]>(inputXml, "Users");

            var mapper = GetMapper();
            User[] users = mapper.Map<User[]>(importUsersDTOs);

            context.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<ProductShopProfile>());
            return new Mapper(cfg);
        }

        static T DeserializeXml<T>(string xml, string rootElement) where T : class
        {
            T result = default(T);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = (T)xmlSerializer.Deserialize(ms);
            }

            return result;
        }

        string SerializeObject<T>(T data, string rootElement) where T : class
        {
            string result = null!;

            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));

            using (MemoryStream ms = new MemoryStream())
            {

                serializer.Serialize(ms, data);
                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result;
        }
    }
}