namespace CHOMP_DEMO.Controllers
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        bool Set<T>(string key, T data);
    }
}