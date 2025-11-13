using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Tests;

public static class TestHelpers
{
    public static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"tasksdb_{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}
