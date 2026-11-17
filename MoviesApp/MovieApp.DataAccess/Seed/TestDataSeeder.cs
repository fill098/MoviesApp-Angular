using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Configurations;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Enums;

namespace MovieApp.DataAccess.Seed;

/// <summary>
/// Fills an empty database with test data (directors, movies, actors, reviews and a "customer" user).
/// EF Core runs it from UseSeeding/UseAsyncSeeding on every Update-Database / "dotnet ef database update".
/// It does nothing when there are already directors, so running it again never duplicates data.
/// </summary>
/// <remarks>
/// Unlike HasData, no migration is needed and the database generates the ids,
/// so rows added later through the API never collide with the seeded ones.
/// </remarks>
public static class TestDataSeeder
{
    public const string CustomerUserId = "3f9a6c2e-8d41-4b7a-9c5e-1a2b3c4d5e6f";

    public static void Seed(DbContext context)
    {
        if (context.Set<Director>().Any())
        {
            return;
        }

        AddTestData(context);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        if (await context.Set<Director>().AnyAsync(cancellationToken))
        {
            return;
        }

        AddTestData(context);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void AddTestData(DbContext context)
    {
        const string adminId = UserConfiguration.AdminUserId;

        // A second user, so movies can have reviews from more than one person.
        // Log in with: customer / Customer123!
        if (context.Set<User>().Find(CustomerUserId) is null)
        {
            context.Set<User>().Add(new User
            {
                Id = CustomerUserId,
                UserName = "customer",
                NormalizedUserName = "CUSTOMER",
                Email = "customer@moviesapp.local",
                NormalizedEmail = "CUSTOMER@MOVIESAPP.LOCAL",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEEgO0Pj7ARICgk0dfFybWVp52aw92CmhiJBUeocjQeLO2wPsPlnHNfzevYtG+66SNA==",
                SecurityStamp = "7c1e4b2a-5d3f-4e6a-8b9c-0d1e2f3a4b5c",
                ConcurrencyStamp = "9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b"
            });

            context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
            {
                UserId = CustomerUserId,
                RoleId = RoleConfiguration.CustomerRoleId
            });
        }

        // ---------- Directors ----------
        var nolan = NewDirector("Christopher", "Nolan", new DateOnly(1970, 7, 30), "British-American",
            "Known for large-scale, puzzle-like films that play with time and memory.");
        var villeneuve = NewDirector("Denis", "Villeneuve", new DateOnly(1967, 10, 3), "Canadian",
            "Canadian filmmaker known for atmospheric science-fiction and thrillers.");
        var gerwig = NewDirector("Greta", "Gerwig", new DateOnly(1983, 8, 4), "American",
            "Actor turned writer-director of character-driven coming-of-age stories.");
        var tarantino = NewDirector("Quentin", "Tarantino", new DateOnly(1963, 3, 27), "American",
            "Writer-director famous for sharp dialogue and non-linear storytelling.");
        var miyazaki = NewDirector("Hayao", "Miyazaki", new DateOnly(1941, 1, 5), "Japanese",
            "Co-founder of Studio Ghibli and one of the most celebrated animators.");

        // No movies yet: useful for testing an empty MoviesDirected list and deleting a director.
        var coppola = NewDirector("Sofia", "Coppola", new DateOnly(1971, 5, 14), "American",
            "Director of quiet, stylish films about isolation and celebrity.");

        // ---------- Actors ----------
        var dicaprio = NewActor("Leonardo", "DiCaprio", new DateOnly(1974, 11, 11), "American",
            "Academy Award-winning actor who began his career in television.");
        var mcconaughey = NewActor("Matthew", "McConaughey", new DateOnly(1969, 11, 4), "American",
            "Texan actor known for both romantic comedies and dramatic roles.");
        var hathaway = NewActor("Anne", "Hathaway", new DateOnly(1982, 11, 12), "American",
            "Academy Award-winning actor of stage and screen.");
        var bale = NewActor("Christian", "Bale", new DateOnly(1974, 1, 30), "British",
            "Actor known for dramatic physical transformations for his roles.");
        var caine = NewActor("Michael", "Caine", new DateOnly(1933, 3, 14), "British",
            "Veteran English actor with a career spanning more than six decades.");
        var chalamet = NewActor("Timothée", "Chalamet", new DateOnly(1995, 12, 27), "American-French",
            "Young actor who rose to fame with independent dramas.");
        var ferguson = NewActor("Rebecca", "Ferguson", new DateOnly(1983, 10, 19), "Swedish",
            "Swedish actor known for international action and science-fiction films.");
        var adams = NewActor("Amy", "Adams", new DateOnly(1974, 8, 20), "American",
            "Six-time Academy Award nominee.");
        var ronan = NewActor("Saoirse", "Ronan", new DateOnly(1994, 4, 12), "Irish-American",
            "Irish actor nominated for several Academy Awards.");
        var jackson = NewActor("Samuel L.", "Jackson", new DateOnly(1948, 12, 21), "American",
            "One of the highest-grossing actors of all time.");
        var thurman = NewActor("Uma", "Thurman", new DateOnly(1970, 4, 29), "American",
            "Actor best known for her collaborations with Quentin Tarantino.");

        // No movies yet: useful for testing an actor without a filmography.
        var ruffalo = NewActor("Mark", "Ruffalo", new DateOnly(1967, 11, 22), "American",
            "Actor known for independent films and blockbuster roles alike.");

        // ---------- Movies (with their actors) ----------
        var inception = NewMovie("Inception", new DateOnly(2010, 7, 16), 148, Genre.ScienceFiction, nolan,
            "A thief who steals secrets from people's dreams is asked to plant an idea instead.",
            dicaprio, caine);
        var interstellar = NewMovie("Interstellar", new DateOnly(2014, 11, 7), 169, Genre.ScienceFiction, nolan,
            "Astronauts travel through a wormhole to find a new home for humanity.",
            mcconaughey, hathaway, caine, chalamet);
        var darkKnight = NewMovie("The Dark Knight", new DateOnly(2008, 7, 18), 152, Genre.Action, nolan,
            "Batman faces the Joker, a criminal who wants to plunge Gotham into chaos.",
            bale, caine);
        var dune = NewMovie("Dune", new DateOnly(2021, 10, 22), 155, Genre.ScienceFiction, villeneuve,
            "A young noble and his family take control of a dangerous desert planet.",
            chalamet, ferguson);
        var arrival = NewMovie("Arrival", new DateOnly(2016, 11, 11), 116, Genre.ScienceFiction, villeneuve,
            "A linguist is recruited to communicate with mysterious visitors from space.",
            adams);
        var ladyBird = NewMovie("Lady Bird", new DateOnly(2017, 11, 3), 94, Genre.Comedy, gerwig,
            "A headstrong teenager spends her last year of high school in Sacramento.",
            ronan, chalamet);
        var littleWomen = NewMovie("Little Women", new DateOnly(2019, 12, 25), 135, Genre.Drama, gerwig,
            "Four sisters grow up in New England after the American Civil War.",
            ronan, chalamet);
        var pulpFiction = NewMovie("Pulp Fiction", new DateOnly(1994, 10, 14), 154, Genre.Crime, tarantino,
            "The lives of two hitmen, a boxer and a gangster's wife intertwine in Los Angeles.",
            jackson, thurman);
        var django = NewMovie("Django Unchained", new DateOnly(2012, 12, 25), 165, Genre.Western, tarantino,
            "A freed slave teams up with a bounty hunter to rescue his wife.",
            dicaprio, jackson);

        // No actors: an animated film, useful for testing an empty cast.
        var spiritedAway = NewMovie("Spirited Away", new DateOnly(2001, 7, 20), 125, Genre.Animation, miyazaki,
            "A girl must work in a bathhouse for spirits to free her parents from a curse.");

        // ---------- Reviews (one per user per movie; Little Women has none, so its rating is 0) ----------
        AddReview(inception, adminId, 9, "Mind-bending and beautifully made.");
        AddReview(inception, CustomerUserId, 10, "I've watched it five times and still notice new things.");
        AddReview(interstellar, adminId, 8, "Great visuals, a bit long in the middle.");
        AddReview(interstellar, CustomerUserId, 10, "The docking scene is unforgettable.");
        AddReview(darkKnight, adminId, 10, "The best superhero movie ever made.");
        AddReview(darkKnight, CustomerUserId, 9, null);
        AddReview(dune, adminId, 8, "Huge in scale; I can't wait for part two.");
        AddReview(arrival, CustomerUserId, 7, "Slow, but the ending is worth it.");
        AddReview(ladyBird, adminId, 6, "Charming, though not really my kind of film.");
        AddReview(pulpFiction, adminId, 9, "Endlessly quotable.");
        AddReview(pulpFiction, CustomerUserId, 5, "Too violent for me.");
        AddReview(django, CustomerUserId, 8, "Long, but never boring.");
        AddReview(spiritedAway, adminId, 10, "Pure magic.");
        AddReview(spiritedAway, CustomerUserId, 9, "Beautiful animation.");

        // Adding the directors is enough: EF follows the navigations and also inserts
        // their movies, the movies' actors and reviews, and the MovieActors join rows.
        context.Set<Director>().AddRange(nolan, villeneuve, gerwig, tarantino, miyazaki, coppola);
        context.Set<Actor>().Add(ruffalo);

        Director NewDirector(string firstName, string lastName, DateOnly dateOfBirth, string nationality, string biography)
        {
            return new Director
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Nationality = nationality,
                Biography = biography,
                PhotoUrl = PlaceholderImage($"{firstName} {lastName}"),
                UserId = adminId
            };
        }

        Actor NewActor(string firstName, string lastName, DateOnly dateOfBirth, string nationality, string biography)
        {
            return new Actor
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Nationality = nationality,
                Biography = biography,
                PhotoUrl = PlaceholderImage($"{firstName} {lastName}"),
                UserId = adminId
            };
        }

        Movie NewMovie(string title, DateOnly releaseDate, int durationMinutes, Genre genre, Director director,
            string description, params Actor[] actors)
        {
            var movie = new Movie
            {
                Title = title,
                Description = description,
                ReleaseDate = releaseDate,
                DurationMinutes = durationMinutes,
                Genre = genre,
                PosterUrl = PlaceholderImage(title),
                TrailerUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(title + " trailer")}",
                UserId = adminId,
                Director = director
            };

            foreach (var actor in actors)
            {
                movie.Actors.Add(actor);
            }

            director.Movies.Add(movie);
            return movie;
        }

        static void AddReview(Movie movie, string userId, int rating, string? comment)
        {
            movie.Reviews.Add(new Review { UserId = userId, Rating = rating, Comment = comment });
        }

        static string PlaceholderImage(string text)
        {
            return $"https://placehold.co/300x450?text={Uri.EscapeDataString(text)}";
        }
    }
}
