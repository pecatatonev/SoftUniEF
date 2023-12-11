namespace Cadastre.DataProcessor
{
    using Cadastre.Data;
    using Cadastre.Data.Enumerations;
    using Cadastre.Data.Models;
    using Cadastre.DataProcessor.ImportDtos;
    using Invoices.Extensions;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Net;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportDistirctDTO> distirctDTOs = xmlDocument.DeserializeFromXml<List<ImportDistirctDTO>>("Districts");

            List<District> districts = new List<District>();

            foreach (var districtDTO in distirctDTOs)
            {
                if (!IsValid(districtDTO)) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (districts.Any(d => d.Name == districtDTO.Name)) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                District district = new District()
                {
                    Region = (Region)Enum.Parse(typeof(Region), districtDTO.Region),
                    Name = districtDTO.Name,
                    PostalCode = districtDTO.PostalCode,
                };

                foreach (var propDTO in districtDTO.Properties)
                {
                    if (!IsValid(propDTO))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime propDate;
                    bool isDateValid = DateTime
                        .TryParseExact(propDTO.DateOfAcquisition, "dd/MM/yyyy", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out propDate);

                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (districts.Any(d => d.Properties.Any(p => p.PropertyIdentifier == propDTO.PropertyIdentifier))) 
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (district.Properties.Any(p => p.PropertyIdentifier == propDTO.PropertyIdentifier))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (districts.Any(d => d.Properties.Any(p => p.Address == propDTO.Address))) 
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (district.Properties.Any(p => p.Address == propDTO.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Property property = new Property()
                    {
                        PropertyIdentifier = propDTO.PropertyIdentifier,
                        Area = propDTO.Area,
                        Details = propDTO.Details,
                        Address = propDTO.Address,
                        DateOfAcquisition = propDate
                    };

                    district.Properties.Add(property);
                }

                districts.Add(district);
                sb.AppendLine(string.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count));
            }

            dbContext.Districts.AddRange(districts);   
            dbContext.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            string[] validMaritalStatus = new string[] { "Unmarried", "Married", "Divorced" , "Widowed" };
            StringBuilder sb = new StringBuilder();

            List<ImportCitizenDTO> citizenDTOs = jsonDocument.DeserializeFromJson<List<ImportCitizenDTO>>();

            List<Citizen> citizens = new List<Citizen>();

            foreach (var citizenDTO in citizenDTOs)
            {
                if (!IsValid(citizenDTO)) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validMaritalStatus.Contains(citizenDTO.MaritalStatus)) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                DateTime birthDate;
                bool isDateValid = DateTime
                    .TryParseExact(citizenDTO.BirthDate, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);

                if (!isDateValid) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Citizen citizen = new Citizen()
                {
                    FirstName = citizenDTO.FirstName,
                    LastName = citizenDTO.LastName,
                    BirthDate = birthDate,
                    MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus),citizenDTO.MaritalStatus),
                };

                foreach (var propId in citizenDTO.Properties)
                {
                    if (citizen.PropertiesCitizens.Any(x => x.PropertyId == propId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    PropertyCitizen propertyCitizen = new PropertyCitizen()
                    {
                        PropertyId = propId,
                        Citizen = citizen,
                    };

                    citizen.PropertiesCitizens.Add(propertyCitizen);
                }

                citizens.Add(citizen);
                sb.AppendLine(string.Format(SuccessfullyImportedCitizen, citizen.FirstName, citizen.LastName, citizen.PropertiesCitizens.Count));
            }

            dbContext.Citizens.AddRange(citizens);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
