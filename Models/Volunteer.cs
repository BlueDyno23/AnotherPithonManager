using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AnotherPithonManager.Models
{
    public class Volunteer
    {
        [Key]
        public int dId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Entry { get; set; }
        public string Exit { get; set; }
        public string Date { get; set; }

        public Volunteer()
        {

        }
        public Volunteer(int id, string name, string entry, string exit, string date)
        {
            Id = id;
            Name = name;
            Entry = entry;
            Exit = exit;
            Date = date;
        }
    }
}
