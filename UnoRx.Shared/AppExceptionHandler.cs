using System;
using System.Diagnostics;
using System.Reactive.Concurrency;

using ReactiveUI;

using Splat;

namespace UnoRx {

    /// <summary>
    /// Custom exception handler
    /// </summary>
    public class AppExceptionHandler : IEnableLogger, IObserver<Exception> {

        public void OnNext(Exception value) {
            if (Debugger.IsAttached) Debugger.Break();

            this.Log().Error(value);

            RxApp.MainThreadScheduler.Schedule(() => { throw value; });
        }

        public void OnError(Exception error) {
            if (Debugger.IsAttached) Debugger.Break();

            this.Log().Error(error);

            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnCompleted() {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { throw new NotImplementedException(); });
        }

    }
}
