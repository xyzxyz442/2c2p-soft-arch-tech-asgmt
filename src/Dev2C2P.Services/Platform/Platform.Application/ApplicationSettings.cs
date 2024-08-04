namespace Dev2C2P.Services.Platform.Application;

public class ApplicationSettings
{
    #region Singleton Pattern
    private static readonly Lazy<ApplicationSettings> instance
        = new Lazy<ApplicationSettings>(() => Activator.CreateInstance<ApplicationSettings>());

    public static ApplicationSettings I
    {
        get
        {
            return instance.Value;
        }
    }
    #endregion

    public bool IsSeedDatabase { get; set; } = false;
    public DatabaseSetting Database { get; set; } = new DatabaseSetting();
}

public class DatabaseSetting()
{
    public string ConnectionString { get; set; } = string.Empty;
}
