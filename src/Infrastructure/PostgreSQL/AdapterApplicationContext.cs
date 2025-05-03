namespace Infrastructure.PostgreSQL;

public class AdapterApplicationContext : IAdapterApplicationContext
{
    public ApplicationContext getContext() => new ApplicationContext();
}