﻿// <auto-generated />
using System;
using Data.BlizzardAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(BlizzardAPIContext))]
    [Migration("20250715211524_UpdatingStringsToFitEnums")]
    partial class UpdatingStringsToFitEnums
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ETL.BlizzardAPI.Endpoints.ItemAppearance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ClassType")
                        .HasColumnType("integer");

                    b.Property<int>("DisplayInfoId")
                        .HasColumnType("integer");

                    b.Property<string>("ItemIds")
                        .IsRequired()
                        .HasMaxLength(511)
                        .HasColumnType("character varying(511)");

                    b.Property<DateTime>("LastUpdatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SlotType")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("SubclassType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("item_appearance", "wow");
                });
#pragma warning restore 612, 618
        }
    }
}
