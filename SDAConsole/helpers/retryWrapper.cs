namespace SDAConsole.helpers
{
    /**
    * @brief Provides helper methods for executing operations with retry logic.
    */
    public static class RetryHelper
    {
        /**
        * @brief Executes an asynchronous operation with retry logic.
        *
        * @tparam T The type of the result returned by the operation.
        * @param action The asynchronous operation to execute.
        * @param maxRetries The maximum number of retry attempts. Default is 3.
        * @param delayMs The delay in milliseconds between retry attempts. Default is 2000ms.
        * @return The result of the operation if it succeeds.
        * @throws SDAException Thrown if the operation fails after the specified number of retry attempts.
        */
        public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxRetries = 3, int delayMs = 2000)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        throw new SDAException($"Operation failed after {maxRetries} attempts. Error: {ex}", ex);
                    }
                    await Task.Delay(delayMs);
                }
            }
            return default;
        }
    }

}
