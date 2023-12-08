using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
    public class UserUploadImage
    {
        public record Request(IFormFile imageFile, string imageLabel, int GridX, int GridY) : IRequest<Response>;
        public record Response(bool Success, string[] Errors);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly GameContext _db;

            public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    string folderPath = "Assets/task_2_scattered_images";

                    if (!Directory.Exists(folderPath))
                    {
                        return new Response(false, new[] { $"The specified folder path '{folderPath}' does not exist." });
                    }

                    string imageFileName = request.imageFile.FileName;
                    string imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileName);
                    string newFolder = Guid.NewGuid().ToString() + imageNameWithoutExtension;
                    string newDirectoryPath = Path.Combine(folderPath, newFolder);

                    if (Directory.Exists(newDirectoryPath))
                    {
                        return new Response(false, new[] { $"The directory '{newDirectoryPath}' already exists." });
                    }
                    Directory.CreateDirectory(newDirectoryPath);

                    using (Image<Rgba32> originalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(request.imageFile.OpenReadStream()))
                    {
                        int width = originalImage.Width;
                        int height = originalImage.Height;

                        if (width > height)
                        {
                            int resizedWidth = 600;
                            int resizedHeight = 0;
                            originalImage.Mutate(x => x.Resize(resizedWidth, resizedHeight));
                        }
                        else
                        {
                            int resizedWidth = 0;
                            int resizedHeight = 300;
                            originalImage.Mutate(x => x.Resize(resizedWidth, resizedHeight));
                        }

                        width = originalImage.Width;
                        height = originalImage.Height;

                        int fragment_no = 0;
                        List<Tile> tiles = new List<Tile>();

                        for (int x = 0; x < request.GridX; x += 1)
                        {
                            for (int y = 0; y < request.GridY; y += 1)
                            {
                                int fragmentSizeX = width / request.GridX;
                                int fragmentSizeY = height / request.GridY;

                                int startX = x * fragmentSizeX;
                                int startY = y * fragmentSizeY;

                                if (width - (startX + fragmentSizeX) < fragmentSizeX && x == request.GridX - 1)
                                {
                                    fragmentSizeX += width - (startX + fragmentSizeX);
                                }
                                if (height - (startY + fragmentSizeY) < fragmentSizeY && y == request.GridY - 1)
                                {
                                    fragmentSizeY += height - (startY + fragmentSizeY);
                                }

                                SixLabors.ImageSharp.Rectangle fragmentRect = new SixLabors.ImageSharp.Rectangle(startX, startY, fragmentSizeX, fragmentSizeY);
                                // Create a new image frame with the same dimensions as the original image
                                using (Image<Rgba32> fragmentImage = new Image<Rgba32>(width, height))
                                {
                                    using (Image<Rgba32> fragment = originalImage.Clone(ctx => ctx.Crop(fragmentRect)))
                                    {
                                        // Draw the fragment onto the image frame
                                        fragmentImage.Mutate(ctx => ctx.DrawImage(fragment, new Point(startX, startY), 1f));
                                    }

                                    // Save as PNG file
                                    string fragmentPath = Path.Combine(newDirectoryPath, $"{fragment_no}.png");
                                    fragmentImage.Save(fragmentPath);

                                    // Convert to base64 string
                                    byte[] fragmentBytes = File.ReadAllBytes(fragmentPath);
                                    string fragment_as_string = Convert.ToBase64String(fragmentBytes);
                                    tiles.Add(new Tile(fragment_as_string));

                                    fragment_no += 1;
                                }
                            }
                        }
                        var image = new Image(Guid.NewGuid().ToString() + imageFileName, tiles, request.imageLabel);
                        await _db.Images.AddAsync(image);
                        await _db.SaveChangesAsync();
                    }
                    return new Response(true, Array.Empty<string>());
                }
                catch (Exception e)
                {
                    return new Response(false, new[] { $"An error occurred: {e.Message}" });
                }
            }
        }
    }
}