﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyOpgaveCore.Models;

namespace SpotifyOpgaveCore.Migrations
{
    [DbContext(typeof(RoomContext))]
    partial class RoomContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SpotifyOpgaveCore.Models.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("Owner");

                    b.HasKey("RoomId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("SpotifyOpgaveCore.Models.Song", b =>
                {
                    b.Property<int>("SongID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("RoomId");

                    b.Property<string>("SpotifyUri");

                    b.HasKey("SongID");

                    b.HasIndex("RoomId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("SpotifyOpgaveCore.Models.Vote", b =>
                {
                    b.Property<int>("VoteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("SongID");

                    b.Property<string>("UserId");

                    b.Property<int>("Value");

                    b.HasKey("VoteId");

                    b.HasIndex("SongID");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("SpotifyOpgaveCore.Models.Song", b =>
                {
                    b.HasOne("SpotifyOpgaveCore.Models.Room")
                        .WithMany("Songs")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SpotifyOpgaveCore.Models.Vote", b =>
                {
                    b.HasOne("SpotifyOpgaveCore.Models.Song", "Song")
                        .WithMany("Votes")
                        .HasForeignKey("SongID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
