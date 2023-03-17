using CefSharp;
using CefSharp.Handler;
using CefSharp.SchemeHandler;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace PainelPress.Classes
{
    public class WpfBrowserProcessHandler : BrowserProcessHandler
    {

        private Timer timer;
        private Dispatcher dispatcher;

        public WpfBrowserProcessHandler(Dispatcher dispatcher)
        {
            timer = new Timer { Interval = 30, AutoReset = true };
            timer.Start();
            timer.Elapsed += TimerTick;

            this.dispatcher = dispatcher;
            this.dispatcher.ShutdownStarted += DispatcherShutdownStarted;
        }

        private void DispatcherShutdownStarted(object sender, EventArgs e)
        {
            //If the dispatcher is shutting down then we stop the timer
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            //Basically execute Cef.DoMessageLoopWork 30 times per second, on the UI Thread
            dispatcher.BeginInvoke((Action)(() => Cef.DoMessageLoopWork()), DispatcherPriority.Render);
        }

        protected override void OnScheduleMessagePumpWork(long delay)
        {
            //If the delay is greater than the Maximum then use ThirtyTimesPerSecond
            //instead - we do this to achieve a minimum number of FPS
            if (delay > 30)
            {
                delay = 30;
            }

            //When delay <= 0 we'll execute Cef.DoMessageLoopWork immediately
            // if it's greater than we'll just let the Timer which fires 30 times per second
            // care of the call
            if (delay <= 0)
            {
                dispatcher.BeginInvoke((Action)(() => Cef.DoMessageLoopWork()), DispatcherPriority.Normal);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dispatcher != null)
                {
                    dispatcher.ShutdownStarted -= DispatcherShutdownStarted;
                    dispatcher = null;
                }

                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}