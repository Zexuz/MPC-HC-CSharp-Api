using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MPC_HC.Domain.Interfaces;
using MPC_HC.Domain.Services;
using Xunit;


namespace MPC_HC.Test
{
    public class CommandServiceTest : IDisposable
    {
        private readonly string _path = "/controls.html";
        private readonly Uri _baseUri = new Uri("http://localhost:13579");
        private readonly Process mediaProcess;
        private readonly IRequestService _requestService;
        private readonly Info _firstInfo;

        public CommandServiceTest()
        {
            mediaProcess = Process.Start("D:\\Program Files (x86)\\MPC-HC\\mpc-hc.exe");
            AsyncHelpers.RunSync(() => Task.Delay(1000));

            _requestService = new RequestService(new HttpClient(), _baseUri, new LogService());

            AsyncHelpers.RunSync(() =>
                new CommandService(_requestService).OpenFile(
                    "D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01E01.Tourist.Trapped.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv"));
            _firstInfo = AsyncHelpers.RunSync(() => new CommandService(_requestService).GetInfo());
        }

        [Fact]
        public void SetSoundThrowsArgumentOutOfRangeException()
        {
            var commandService = new CommandService(_requestService);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-1));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-10));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-54));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(1000));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(120));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(101));
        }

        [Fact]
        public async void SetSoundSuccess()
        {
            var commandService = new CommandService(_requestService);
            await commandService.SetSoundLevel(10);

            var soundLevel = await commandService.GetSoundLevel();
            Assert.Equal(10, soundLevel);
        }


        public void Dispose()
        {
            mediaProcess.Kill();
        }
    }

    public static class AsyncHelpers
    {
        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}