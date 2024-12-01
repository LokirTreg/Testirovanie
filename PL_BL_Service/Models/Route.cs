using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PL_BL_Service.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string RouteFirstStop { get; set; }
        public string RouteLastStop { get; set; }
        public double RouteDistanceKm { get; set; }
    }
}
