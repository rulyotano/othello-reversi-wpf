using System.Threading;
using System.Windows.Threading;

namespace Othello.Utils
{
    public class ThreadUtils
    {
        public static void Sleep(int milisecond)
        {
            using (Dispatcher.CurrentDispatcher.DisableProcessing())
            {
                Thread.Sleep(milisecond);
            }
        }
    }
}
