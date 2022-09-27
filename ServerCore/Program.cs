internal class Program
{
    private static int _num = 0;
    private static SpinLock _lock = new SpinLock();

    private static void Thread1()
    {
        for (int i = 0; i < 1000000; i++)
        {
            _lock.Acquire();
            _num++;
            _lock.Release();
        }
    }

    private static void Thread2()
    {
        for (int i = 0; i < 1000000; i++)
        {
            _lock.Acquire();
            _num--;
            _lock.Release();
        }
    }

    private static void Main(string[] args)
    {
        Task thread1 = new Task(Thread1);
        Task thread2 = new Task(Thread2);
        thread1.Start();
        thread2.Start();

        Task.WaitAll(thread1, thread2);

        Console.WriteLine(_num);
    }
}

class SpinLock
{
    private const int expectedLockValue = 0;
    private const int desiredLockValue = 1;
    private volatile int _locked = 0;

    public void Acquire()
    {
        while (true)
        {
            if (Interlocked.CompareExchange(ref _locked, desiredLockValue, expectedLockValue).Equals(expectedLockValue))
            {
                break;
            }
        }
    }

    public void Release()
    {
        _locked = 0;
    }
}