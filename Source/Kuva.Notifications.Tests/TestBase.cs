using Kuva.Notifications.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Tests;

public abstract class TestBase
{
    protected static NotificationsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new NotificationsDbContext(options);
    }
}
