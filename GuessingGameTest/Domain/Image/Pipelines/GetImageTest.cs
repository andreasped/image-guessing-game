using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetImageTests : DbTest
    {
        public GetImageTests(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public async Task ImageDoesExist_ShouldReturnImage()
        {
            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();

            var image = new GuessingGame.Core.Domain.Image.Image("source", new List<Tile>(), "label");

            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();
            var request = new GetImage.Request(image.Id);

            var handler = new GetImage.Handler(context);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(image, result);
        }
    }
}
