﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using V.NET.API.Database;

#nullable disable

namespace V.NET.API.Database.Migrations
{
    [DbContext(typeof(UrlShortenerDbContext))]
    [Migration("20240820165731_AddRequestLogs")]
    partial class AddRequestLogs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("V.NET.API.Models.RequestLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("RequestedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RequesterIp")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UrlMappingId")
                        .HasColumnType("integer");

                    b.Property<string>("UserAgent")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UrlMappingId");

                    b.ToTable("RequestLogs");
                });

            modelBuilder.Entity("V.NET.API.Models.UrlMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OriginalUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequesterIp")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.HasKey("Id");

                    b.HasIndex("ShortCode")
                        .IsUnique();

                    b.ToTable("UrlMappings");
                });

            modelBuilder.Entity("V.NET.API.Models.RequestLog", b =>
                {
                    b.HasOne("V.NET.API.Models.UrlMapping", "UrlMapping")
                        .WithMany("RequestLogs")
                        .HasForeignKey("UrlMappingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UrlMapping");
                });

            modelBuilder.Entity("V.NET.API.Models.UrlMapping", b =>
                {
                    b.Navigation("RequestLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
