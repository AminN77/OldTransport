﻿using Data.Model;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Cross.Abstractions.EntityEnums;

namespace Data.DataAccess.Context
{

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var role = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = RoleType.Admin.ToString()
                },
                new Role
                {
                    Id = 2,
                    Name = RoleType.Merchant.ToString()
                },
                new Role
                {
                    Id = 3,
                    Name = RoleType.Transporter.ToString()
                },
                new Role
                {
                    Id = 4,
                    Name = RoleType.DeveloperSupport.ToString()
                }
            };

            modelBuilder.Entity<Role>().HasData(role);

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
                RoleId = 4,
                UserId = developerUser.Id
            };

            modelBuilder.Entity<UserRole>().HasData(userRole);
        }
    }
}