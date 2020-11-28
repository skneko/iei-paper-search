﻿// <auto-generated />
using System;
using IEIPaperSearch.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IEIPaperSearch.Migrations
{
    [DbContext(typeof(PaperSearchContext))]
    partial class PaperSearchContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IEIPaperSearch.Models.Issue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("JournalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Month")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Volume")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("JournalId");

                    b.ToTable("Issue");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Journal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Journals");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surnames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Submission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PersonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Submission");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Submission");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Article", b =>
                {
                    b.HasBaseType("IEIPaperSearch.Models.Submission");

                    b.Property<string>("EndPage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PublishedInId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StartPage")
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("PublishedInId");

                    b.HasDiscriminator().HasValue("Article");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Book", b =>
                {
                    b.HasBaseType("IEIPaperSearch.Models.Submission");

                    b.Property<string>("Publisher")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Book");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.InProceedings", b =>
                {
                    b.HasBaseType("IEIPaperSearch.Models.Submission");

                    b.Property<string>("Conference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Edition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EndPage")
                        .HasColumnName("InProceedings_EndPage")
                        .HasColumnType("int");

                    b.Property<int>("StartPage")
                        .HasColumnName("InProceedings_StartPage")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("InProceedings");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Issue", b =>
                {
                    b.HasOne("IEIPaperSearch.Models.Journal", "Journal")
                        .WithMany("Issues")
                        .HasForeignKey("JournalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Submission", b =>
                {
                    b.HasOne("IEIPaperSearch.Models.Person", null)
                        .WithMany("AuthorOf")
                        .HasForeignKey("PersonId");
                });

            modelBuilder.Entity("IEIPaperSearch.Models.Article", b =>
                {
                    b.HasOne("IEIPaperSearch.Models.Issue", "PublishedIn")
                        .WithMany("Articles")
                        .HasForeignKey("PublishedInId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
