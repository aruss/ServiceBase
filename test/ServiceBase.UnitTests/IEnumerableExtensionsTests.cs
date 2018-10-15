namespace ServiceBase.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ServiceBase.Extensions;
    using Xunit;

    [Collection("ServiceBase")]
    public class IEnumerableExtensionsTests
    {
        private class Foo
        {
            public Foo(string name)
            {
                this.Name = name;
            }

            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                var item = obj as Foo;

                if (item == null)
                {
                    return false;
                }

                return this.Name.Equals(item.Name);
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }
        }

        private void AssertResults<TSource>(
            IEnumerable<TSource> removedExpected,
            IEnumerable<TSource> addedExpected,
            IEnumerable<TSource> updatedExpected,
            IEnumerable<TSource> removedActual,
            IEnumerable<TSource> addedActual,
            IEnumerable<TSource> updatedActual
        )
        {
            Assert.Equal(removedExpected.Count(), removedActual.Count());
            Assert.Equal(addedExpected.Count(), addedActual.Count());
            Assert.Equal(updatedExpected.Count(), updatedActual.Count());

            foreach (var item in removedExpected)
            {
                Assert.NotEqual(
                    default(TSource),
                    removedActual.FirstOrDefault(c => c.Equals(item))
                );
            }

            foreach (var item in addedExpected)
            {
                Assert.NotEqual(
                    default(TSource),
                    addedActual.FirstOrDefault(c => c.Equals(item))
                );
            }

            foreach (var item in updatedExpected)
            {
                Assert.NotEqual(
                    default(TSource),
                    updatedActual.FirstOrDefault(c => c.Equals(item))
                );
            }
        }

        [Fact]
        public void AddRemoveAddPrimitive()
        {
            var listMaster = new string[] { "foo", "bar" };
            var listUpdated = new string[] { "bar", "baz" };
            var (removed, added, updated) = listMaster.Diff(listUpdated);

            this.AssertResults(
                new string[] { "foo" },
                new string[] { "baz" },
                new string[] { "bar" },
                removed,
                added,
                updated);
        }

        [Fact]
        public void RemoveAllPrimitive()
        {
            var listMaster = new string[] { "foo", "bar" };
            var listUpdated = new string[] { };
            var (removed, added, updated) = listMaster.Diff(listUpdated);

            this.AssertResults(
                new string[] { "foo", "bar" },
                new string[] { },
                new string[] { },
                removed,
                added,
                updated);
        }

        [Fact]
        public void UpdateAllPrimitive()
        {
            var listMaster = new string[] { "foo", "bar" };
            var listUpdated = new string[] { "foo", "bar" };
            var (removed, added, updated) = listMaster.Diff(listUpdated);

            this.AssertResults(
                new string[] { },
                new string[] { },
                new string[] { "foo", "bar" },
                removed,
                added,
                updated);
        }

        [Fact]
        public void AddAllPrimitive()
        {
            var listMaster = new string[] { };
            var listUpdated = new string[] { "foo", "bar" };
            var (removed, added, updated) = listMaster.Diff(listUpdated);

            this.AssertResults(
                new string[] { },
                new string[] { "foo", "bar" },
                new string[] { },
                removed,
                added,
                updated);
        }


        [Fact]
        public void AddRemoveAddObject()
        {
            var listMaster = new Foo[] {
                new Foo("foo"),
                new Foo("bar")
            };

            var listUpdated = new Foo[] {
                new Foo("bar"),
                new Foo("baz")
            };

            var (removed, added, updated) = listMaster.Diff(
                listUpdated
            );

            this.AssertResults(
                new Foo[] { new Foo("foo") },
                new Foo[] { new Foo("baz") },
                new Foo[] { new Foo("bar") },
                removed,
                added,
                updated);
        }
    }
}
