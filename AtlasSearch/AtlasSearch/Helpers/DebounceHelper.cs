internal static class DebounceHelper
{
    /// <summary>
    /// Debounce a request to ensure we don't execute it on every property change.
    /// </summary>
    public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
    {
        var last = 0;
        return arg =>
        {
            var current = Interlocked.Increment(ref last);
            Task.Delay(milliseconds).ContinueWith(task =>
            {
                if (current == last)
                {
                    func(arg);
                }

                task.Dispose();
            });
        };
    }
}