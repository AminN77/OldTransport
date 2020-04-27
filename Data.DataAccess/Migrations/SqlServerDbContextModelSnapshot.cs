﻿// <auto-generated />
using System;
using Data.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.DataAccess.Migrations
{
    [DbContext(typeof(SqlServerDbContext))]
    partial class SqlServerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Data.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("char(128)")
                        .HasMaxLength(128);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("IterationCount")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("char(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Picture")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("varbinary(32)")
                        .HasMaxLength(32);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("FirstName");

                    b.HasIndex("LastName");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreateDateTime = new DateTime(2020, 4, 28, 0, 48, 26, 191, DateTimeKind.Local).AddTicks(7840),
                            EmailAddress = "abolfazl.sh1374@gmail.com",
                            FirstName = "شرکت",
                            IsDeleted = false,
                            IsEnabled = true,
                            IterationCount = 33271,
                            LastName = "کندو",
                            Password = "D00F0CE1E55CB8D22BE3FAA47822FB12FDB61EE22ADFE21697C975EF8945D631C8A75E5150C6F090169F5EDB733EB978EF5FB28786609EAA93D03EF418A37888",
                            Picture = "Source/1.jpg",
                            Salt = new byte[] { 60, 61, 31, 75, 173, 200, 151, 55, 188, 197, 90, 4, 176, 168, 164, 238, 184, 44, 250, 156, 162, 107, 64, 44, 236, 91, 75, 167, 19, 75, 116, 138 },
                            Username = "developersupport"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
