using AutoFixture;
using AutoFixture.AutoMoq;
using System;

namespace DotnetIgnoreCliToolTests
{
    public abstract class UnitTestBase : IDisposable
    {
        public IFixture Fixture { get; }

        protected UnitTestBase()
        {
            Fixture = new Fixture()
                .Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }

        public virtual void Dispose()
        {
        }
    }
}