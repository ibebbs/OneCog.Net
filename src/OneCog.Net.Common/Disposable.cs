﻿using System;

namespace OneCog.Net
{
    public class Disposable : IDisposable
    {
        private Action _dispose;

        public Disposable(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (_dispose != null)
            {
                _dispose();
                _dispose = null;
            }
        }
    }
}
