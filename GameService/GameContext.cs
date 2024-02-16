using GameDataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GameService
{
    public class GameContext : DbContext
    {
        private readonly IConfiguration configuration;
        public GameContext(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(paramName: nameof(configuration), message: "IConfiguration is not defined");
        }

        public DbSet<PublisherModel> Publishers { get; set; }
        public DbSet<BoardGameModel> Boards { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                throw new InvalidOperationException("Can Not find ConnectionString DefaultConnection");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add all publishers seed data first
            modelBuilder.Entity<PublisherModel>().HasData(
                new PublisherModel()
                {
                    Id = 1,
                    Name = "Days of Wonder"
                },
                new PublisherModel()
                {
                    Id = 2,
                    Name = "Stonemaier Games"
                },
                  new PublisherModel()
                  {
                      Id = 3,
                      Name = "Hasbro"
                  },
                  new PublisherModel()
                  {
                      Id = 4,
                      Name = "Spin Master"
                  }
                );

            // Add BoardGame seed data
            modelBuilder.Entity<BoardGameModel>().HasData(
                new BoardGameModel()
                {
                    Id = 1,
                    PublishersId = 1,
                    Name = "SmallWorld",
                    Description = "Control one fantasy race after another to expand throught the land",
                    ImageURL = "https://cf.geekdo-images.com/aoPM07XzoceB-RydLh08zA__imagepage/img/lHmv0ddOrUvpiLcPeQbZdT5yCEA=/fit-in/900x600/filters:no_upscale():strip_icc()/pic428828.jpg"
                },
                new BoardGameModel()
                {
                    Id = 2,
                    PublishersId = 2,
                    Name = "WingSpan",
                    Description = "Attract a beautiful and diverse" +
                    "collection of birds to your wildlife preserve.",
                    ImageURL = "https://cf.geekdo-images.com/yLZJCVLlIx4c7eJEWUNJ7w__imagepagezoom/img/yS4vL6iTCvHSvGySxyOjV_-R3dI=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic4458123.jpg"
                },
                new BoardGameModel()
                {
                    Id = 3,
                    PublishersId = 3,
                    Name = "Trouble",
                    Description = "Hasbro Gaming Trouble Board Game for Kids Ages 5 and Up 2-4 Players",
                    ImageURL = "https://m.media-amazon.com/images/I/81MdgnO4l9L._AC_UL400_.jpg"
                },
                new BoardGameModel()
                {
                    Id = 4,
                    PublishersId = 3,
                    Name = "The Game of Life",
                    Description = "Hasbro Gaming The Game of Life Board Game Ages 8 & Up",
                    ImageURL = "https://m.media-amazon.com/images/I/81yQxkx3vwL._AC_UL640_QL65_.jpg"
                },
                new BoardGameModel()
                {
                    Id = 5,
                    PublishersId = 3,
                    Name = "Candy Land",
                    Description = "Hasbro Gaming Candy Land Kingdom Of Sweet Adventures Board Game For Kids Ages",
                    ImageURL = "https://m.media-amazon.com/images/I/91yUG40gv0L._AC_UL400_.jpg"
                }
                ,
                new BoardGameModel()
                {
                    Id = 6,
                    PublishersId = 3,
                    Name = "Risk",
                    Description = "Hasbro Gaming Risk Military Wargame",
                    ImageURL = "https://m.media-amazon.com/images/I/91jsvpbPP3L._AC_UL400_.jpg"
                },
                new BoardGameModel()
                {
                    Id = 7,
                    PublishersId = 1,
                    Name = "Ticket to ride",
                    Description = "Ticket to Ride Board Game | Family Board Game | Board Game for Adults and Family",
                    ImageURL = "https://m.media-amazon.com/images/I/91YNJM4oyhL._AC_UL400_.jpg"
                },
                new BoardGameModel()
                {
                    Id = 8,
                    PublishersId = 4,
                    Name = "Sorry",
                    Description = "SORRY Classic Family Board Game Indoor Outdoor Retro Party Activity Summer Toy with Oversized Gameboard",
                    ImageURL = "https://m.media-amazon.com/images/I/81ItkRyOaaL._AC_UL400_.jpg"
                }
                );
        }
    }
}
