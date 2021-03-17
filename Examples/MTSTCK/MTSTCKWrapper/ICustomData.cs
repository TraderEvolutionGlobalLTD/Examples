
namespace MTSTCKWrapper
{
    public interface ICustomData<T1, T2>
    {
        string Name { get; }

        int Count { get; }

        T2 GetValueByIndex(int index);
        T2 GetValue(T1 value);  
    }
}
