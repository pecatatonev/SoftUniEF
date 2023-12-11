﻿using Cadastre.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastre.Data.Models
{
    public class PropertyCitizen
    {
        public int PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }
        public int CitizenId { get; set; }
        [ForeignKey(nameof(CitizenId))]
        public Citizen Citizen { get; set;}
    }
}

//•	PropertyId – integer, Primary Key, foreign key (required)
//•	Property – Property
//•	CitizenId – integer, Primary Key, foreign key (required)
//•	Citizen – Citizen
