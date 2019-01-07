using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        private string Password { get; set; }
        [NotMapped]
        public List<FullTrack> FullTrack { get; set; }
        [NotMapped]
        public PlaybackContext PlaybackContext { get; set; }

        public ICollection<Song> Songs { get; set; }
        //public Song Song { get; set; }
    }
    public class Song
    {
        public string SongID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Duration { get; set; }
        public int RoomId { get; set; }

        internal object FirstOrDefault(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}