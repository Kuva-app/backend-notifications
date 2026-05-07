using Kuva.Email.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Email.Tests;

public abstract class TestBase
{
    protected static EmailDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EmailDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new EmailDbContext(options);
    }
}
