namespace Infrastructure.PostgreSQL;

public class AdapterApplicationContext : IAdapterApplicationContext
{
    // Cache a single ApplicationContext instance per adapter lifetime
    private readonly ApplicationContext _ctx = new ApplicationContext();

    public ApplicationContext getContext() => _ctx;
}
