using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trucks.Data.Models;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
        public int TruckId { get; set; }
        [ForeignKey(nameof(TruckId))]
        public Truck Truck { get; set; }
    }
}

//•	ClientId – integer, Primary Key, foreign key (required)
//•	Client – Client
//•	TruckId – integer, Primary Key, foreign key (required)
//•	Truck – Truck
