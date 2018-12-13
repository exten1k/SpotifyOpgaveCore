using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace SpotifyOpgaveCore.Models
{
    public class RoomContext : DbContext
    {
        public RoomContext(DbContextOptions<RoomContext> options) : base(options)
        { }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Song> Songs { get; set; }

    }

    public class Room
    {
        public int RoomId { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }

        public ICollection<Song> Songs {get;set;}
    }
    public class Song
    {
        public string SongID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Duration { get; set; }
    }
}