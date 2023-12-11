using Cadastre.Data;
using Cadastre.DataProcessor.ExportDtos;
using Invoices.Extensions;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
           var propsWithOwners = dbContext.Properties
                .Where(p => p.DateOfAcquisition >= DateTime.Parse("01/01/2000"))
                .OrderByDescending(p => p.DateOfAcquisition)
                .ThenBy(p => p.PropertyIdentifier)
                .Select(p => new ExportPropertiesDTO 
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    Address = p.Address,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy"),
                    Owners = p.PropertiesCitizens
                    .Select(c => new ExportOwnerDTO 
                    {
                        LastName = c.Citizen.LastName,
                        MaritalStatus = c.Citizen.MaritalStatus
                    }).OrderBy(c => c.LastName)
                    .ToArray()
                })
                
                .ToArray();

            return propsWithOwners.SerializeToJson<ExportPropertiesDTO[]>();
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            var properties = dbContext.Properties
                .Where(p => p.Area >= 100)
                .OrderByDescending(p => p.Area)
                .ThenBy(p => p.DateOfAcquisition)
                .Select(p => new ExportPropertyDTO 
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy"),
                    PostalCode = p.District.PostalCode
                })
                .ToArray();

            return properties.SerializeToXml<ExportPropertyDTO[]>("Properties");
        }
    }
}
