using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.SharedKernel;
using Microsoft.EntityFrameworkCore;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace GuessingGameTest.Domain.Image.Pipelines
{
    public class ImageCountTests : DbTest
    {
        public ImageCountTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task NoImages_ShouldReturnZero()
        {
            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();

            var request = new ImageCount.Request();
            var handler = new ImageCount.Handler(context);


            var result = await handler.Handle(request, CancellationToken.None);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Images_ShouldReturnCount()
        {
            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();

            var image = new GuessingGame.Core.Domain.Image.Image("source", new List<Tile>(), "label");

            await context.Images.AddAsync(image);

            await context.SaveChangesAsync();

            var request = new ImageCount.Request();
            var handler = new ImageCount.Handler(context);


            var result = await handler.Handle(request, CancellationToken.None);

            Assert.Equal(1, result);
        }
    }
}
