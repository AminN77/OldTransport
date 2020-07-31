using Data.Model;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Cross.Abstractions.EntityEnums;
using System.IO;
using Newtonsoft.Json;

namespace Data.DataAccess.Context
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {


            const string jsonFileName = @"//SourceCountries.json";
            var contentRootPath = System.IO.Directory.GetCurrentDirectory();
            var jsonString = System.IO.File.ReadAllText(contentRootPath + jsonFileName);
            var myModel = JsonConvert.DeserializeObject<List<Country>>(jsonString);

            modelBuilder.Entity<Country>().HasData(myModel);


            const string jsonCitiesFileName = @"//SourceCities.json";
            var jsonCitiesString = System.IO.File.ReadAllText(contentRootPath + jsonCitiesFileName);
            var myCitiesModel = JsonConvert.DeserializeObject<List<City>>(jsonCitiesString);

            modelBuilder.Entity<City>().HasData(myCitiesModel);


            var role = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = RoleTypes.Admin.ToString()
                },
                new Role
                {
                    Id = 2,
                    Name = RoleTypes.User.ToString()
                },
                new Role
                {
                    Id = 3,
                    Name = RoleTypes.DeveloperSupport.ToString()
                }
            };

            modelBuilder.Entity<Role>().HasData(role);

            var settings = new Settings()
            {
                Id = 1,
                ContactEmail = "abolfazl.sh1374@gmail.com",
                AboutUs = "We're The Transport Team",
                ContactNumber = "+98 937 733 9223",
                Logo = "abcd",
                OffersCountLimit = 5
            };

            modelBuilder.Entity<Settings>().HasData(settings);

            var salt = new byte[32];
            var iterations = new Random().Next(1000, 100000);
            byte[] hashByteArray;
            var stringBuilder = new StringBuilder();
            string hashPassword;
            var now = DateTime.Now;

            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes("transport@developer", salt, iterations))
            {
                hashByteArray = pbkdf2.GetBytes(64);
            }

            foreach (var item in hashByteArray)
            {
                stringBuilder.Append(item.ToString("X2"));
            }

            hashPassword = stringBuilder.ToString();

            var developerUser = new User
            {
                Id = 1,
                Name = "Developer",
                Password = hashPassword,
                CreateDateTime = now,
                IsEnabled = true,
                IsDeleted = false,
                EmailAddress = "abolfazl.sh1374@gmail.com",
                Salt = salt,
                IterationCount = iterations,
                SerialNumber = Guid.NewGuid().ToString()
            };

            modelBuilder.Entity<User>().HasData(developerUser);

            var userRole = new UserRole
            {
                RoleId = 3,
                UserId = developerUser.Id
            };

            modelBuilder.Entity<UserRole>().HasData(userRole);

            //var data = Path.Combine("contries.json");
            //Dictionary<string, string[]> dicData = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(data);

            //int i = 1;
            //int j = 1;
            //foreach (var item in dicData)
            //{
            //    var country = new Country()
            //    {
            //        Id = i,
            //        Name = item.Key
            //    };
            //    modelBuilder.Entity<Country>().HasData(country);

            //    foreach (var unit in item.Value)
            //    {
            //        var city = new City()
            //        {
            //            Id = j,
            //            Name = unit,
            //            CountryId = i
            //        };
            //        modelBuilder.Entity<City>().HasData(city);
            //        j++;
            //    }
            //    i++;
            //}
        }
    }
}