namespace CHOMP_DEMO.Managers
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        bool Set<T>(string key, T data);
        bool Del<T>(string key);
    }
}