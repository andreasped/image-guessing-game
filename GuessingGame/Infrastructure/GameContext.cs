using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GuessingGame.Core.Domain.User;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Results;

namespace GuessingGame.SharedKernel;

public class GameContext : IdentityDbContext<UserIdentity>
{
	private readonly IMediator _mediator;

	public GameContext(DbContextOptions configuration, IMediator mediator) : base(configuration)
	{
		_mediator = mediator;
	}

	public DbSet<Core.Domain.Image.Image> Images { get; set; } = null!;
	public DbSet<Game> Game { get; set; } = null!;
	public DbSet<Leaderboard> Leaderboards { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Game>()
			.HasOne(g => g.Image)
			.WithMany()
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Game>()
			.HasMany(g => g.UsedTiles)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);

		base.OnModelCreating(modelBuilder);
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		if (_mediator == null) return result;

		var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
			.Select(e => e.Entity)
			.Where(e => e.Events.Any())
			.ToArray();

		foreach (var entity in entitiesWithEvents)
		{
			var events = entity.Events.ToArray();
			entity.Events.Clear();
			foreach (var domainEvent in events)
			{
				await _mediator.Publish(domainEvent, cancellationToken);
			}
		}
		return result;
	}

	public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();
}

internal class ImageData
{
	public static Core.Domain.Image.Image[] Images { get; private set; } = null!;
	internal static void Init()
	{
		string directoryPath = "Assets/task_2_scattered_images";

		List<Core.Domain.Image.Image> imageModels = new List<Core.Domain.Image.Image>();

		if (Directory.Exists(directoryPath))
		{
			string[] imageFolderPaths = Directory.GetDirectories(directoryPath, "*");
			string[] labelMapping = File.ReadAllLines("Assets/label_mapping.csv");
			string[] imageMapping = File.ReadAllLines("Assets/image_mapping.csv");
			string correctLabel = "";

			foreach (var imageFolderPath in imageFolderPaths)
			{

				string[] imagePaths = Directory.GetFiles(imageFolderPath, "*.png");
				string folderName = Path.GetFileName(imageFolderPath).Replace("_scattered", "");

				foreach (var img in imageMapping)
				{
					string[] imgParts = img.Split(' ');
					if (folderName == imgParts[0])
					{
						foreach (var label in labelMapping)
						{
							string[] labelParts = label.Split(' ', 2);
							if (imgParts[1] == labelParts[0])
							{
								correctLabel = labelParts[1];
							}
						}
					}
				}

				List<Tile> tiles = new List<Tile>();

				foreach (var imagePath in imagePaths)
				{
					byte[] imageBytes = File.ReadAllBytes(imagePath);
					string image = Convert.ToBase64String(imageBytes);
					tiles.Add(new Tile(image));
				}
				imageModels.Add(new Core.Domain.Image.Image(imageFolderPath, tiles, correctLabel));
			}
		}
		else
		{
			Console.WriteLine("Could not find the directory");
		}
		Images = imageModels.ToArray();
	}
}