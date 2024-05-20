﻿// <auto-generated />
using System;
using DotNetTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DotNetTask.Migrations
{
    [DbContext(typeof(DotNetTaskDbContext))]
    [Migration("20240518003437_AddTables")]
    partial class AddTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.CandidateAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CandidateFormId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CandidateFormId");

                    b.HasIndex("QuestionId");

                    b.ToTable("CandidateAnswers");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.CandidateForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProgramTemplateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProgramTemplateId");

                    b.ToTable("CandidateForms");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.ProgramTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("IDCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nationality")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProgramTemplate");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.Questions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.QuestionsMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProgramTemplateId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProgramTemplateId");

                    b.HasIndex("QuestionsId");

                    b.ToTable("QuestionsMapping");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.CandidateAnswer", b =>
                {
                    b.HasOne("DotNetTask.Models.DatabaseModels.CandidateForm", "CandidateForm")
                        .WithMany()
                        .HasForeignKey("CandidateFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DotNetTask.Models.DatabaseModels.Questions", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CandidateForm");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.CandidateForm", b =>
                {
                    b.HasOne("DotNetTask.Models.DatabaseModels.ProgramTemplate", "ProgramTemplate")
                        .WithMany()
                        .HasForeignKey("ProgramTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProgramTemplate");
                });

            modelBuilder.Entity("DotNetTask.Models.DatabaseModels.QuestionsMapping", b =>
                {
                    b.HasOne("DotNetTask.Models.DatabaseModels.ProgramTemplate", "ProgramTemplate")
                        .WithMany()
                        .HasForeignKey("ProgramTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DotNetTask.Models.DatabaseModels.Questions", "Questions")
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProgramTemplate");

                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
