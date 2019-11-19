using AutoFixture;
using System;

namespace DotnetIgnoreCliToolTests
{
    public abstract class UnitTestBase : IDisposable
    {
        private IFixture Fixture { get; }

        protected UnitTestBase()
        {
            Fixture = new Fixture();
        }

        public virtual void Dispose()
        {
        }
    }
}