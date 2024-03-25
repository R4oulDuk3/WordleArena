﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WordleArena.Infrastructure;

#nullable disable

namespace WordleArena.Migrations
{
    [DbContext(typeof(ArenaDbContext))]
    [Migration("20240311224135_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WordleArena.Domain.Bot", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<bool>("InUse")
                        .HasColumnType("boolean")
                        .HasColumnName("in_use");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("UserId")
                        .HasName("pk_bots");

                    b.ToTable("bots", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.Hash", b =>
                {
                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Value")
                        .HasName("pk_hashes");

                    b.ToTable("hashes", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.PlayerMatchmakingInfo", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("EnterTimestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("enter_timestamp");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("UserId")
                        .HasName("pk_player_matchmaking_infos");

                    b.ToTable("player_matchmaking_infos", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.WordDefinition", b =>
                {
                    b.Property<string>("Word")
                        .HasColumnType("text")
                        .HasColumnName("word");

                    b.Property<bool>("Inflected")
                        .HasColumnType("boolean")
                        .HasColumnName("inflected");

                    b.Property<bool>("IsInDictionary")
                        .HasColumnType("boolean")
                        .HasColumnName("is_in_dictionary");

                    b.HasKey("Word")
                        .HasName("pk_word_definitions");

                    b.ToTable("word_definitions", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.WordleWord", b =>
                {
                    b.Property<string>("TargetWord")
                        .HasColumnType("text")
                        .HasColumnName("target_word");

                    b.Property<int>("Frequency")
                        .HasColumnType("integer")
                        .HasColumnName("frequency");

                    b.Property<int>("WordLenght")
                        .HasColumnType("integer")
                        .HasColumnName("word_lenght");

                    b.HasKey("TargetWord")
                        .HasName("pk_wordle_words");

                    b.ToTable("wordle_words", (string)null);
                });

            modelBuilder.Entity("WordleArena.Infrastructure.Providers.DocumentProviderSerializedState", b =>
                {
                    b.Property<string>("Provider")
                        .HasColumnType("text")
                        .HasColumnName("provider");

                    b.Property<string>("SerializedState")
                        .HasColumnType("text")
                        .HasColumnName("serialized_state");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Provider")
                        .HasName("pk_document_provider_states");

                    b.ToTable("document_provider_states", (string)null);
                });

            modelBuilder.Entity("WordleArena.Domain.WordDefinition", b =>
                {
                    b.HasOne("WordleArena.Domain.WordleWord", null)
                        .WithOne()
                        .HasForeignKey("WordleArena.Domain.WordDefinition", "Word")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_word_definitions_wordle_words_wordle_word_target_word");

                    b.OwnsOne("WordleArena.Domain.PossibleMeanings", "PossibleMeanings", b1 =>
                        {
                            b1.Property<string>("WordDefinitionWord")
                                .HasColumnType("text")
                                .HasColumnName("word");

                            b1.HasKey("WordDefinitionWord");

                            b1.ToTable("word_definitions");

                            b1.ToJson("PossibleMeanings");

                            b1.WithOwner()
                                .HasForeignKey("WordDefinitionWord")
                                .HasConstraintName("fk_word_definitions_word_definitions_word");

                            b1.OwnsMany("WordleArena.Domain.Meaning", "Meanings", b2 =>
                                {
                                    b2.Property<string>("PossibleMeaningsWordDefinitionWord")
                                        .HasColumnType("text");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<List<string>>("Antonyms")
                                        .IsRequired()
                                        .HasColumnType("text[]");

                                    b2.Property<string>("DefinitionText")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Example")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("PartOfSpeech")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<List<string>>("Synonyms")
                                        .IsRequired()
                                        .HasColumnType("text[]");

                                    b2.HasKey("PossibleMeaningsWordDefinitionWord", "Id")
                                        .HasName("pk_word_definitions");

                                    b2.ToTable("word_definitions");

                                    b2.WithOwner()
                                        .HasForeignKey("PossibleMeaningsWordDefinitionWord")
                                        .HasConstraintName("fk_word_definitions_word_definitions_possible_meanings_word_defin");
                                });

                            b1.Navigation("Meanings");
                        });

                    b.Navigation("PossibleMeanings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
