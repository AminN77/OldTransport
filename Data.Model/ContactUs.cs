using Data.Abstractions.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class ContactUs : IContactUs
    {
        public ContactUs()
        {

        }

        [Key]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
