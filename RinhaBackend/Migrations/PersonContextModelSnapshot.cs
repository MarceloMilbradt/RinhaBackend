﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RinhaBackend.Persistence;

#nullable disable

namespace RinhaBackend.Migrations
{
    [DbContext(typeof(PersonContext))]
    partial class PersonContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RinhaBackend.Models.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Apelido")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateOnly>("Nascimento")
                        .HasColumnType("date");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("SearchField")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<List<string>>("Stack")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.HasIndex("Apelido");

                    b.HasIndex("Id");

                    b.HasIndex("SearchField");

                    b.ToTable("Persons");
                });
#pragma warning restore 612, 618
        }
    }
}
