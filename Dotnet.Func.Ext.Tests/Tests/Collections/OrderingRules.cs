namespace Dotnet.Func.Ext.Tests.Tests.Collections
{
    using Dotnet.Func.Ext.Collections;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class TOrderingRules
    {
        private enum Columns { Id, Name }
        private struct Entity { public int Id; public string Name; }
        
        [Test]
        public void OrderEnumerableByTwoFields()
        {
            var rules = new OrderingRules<Columns, Entity>
            {
                { Columns.Id, entity => entity.Id },
                { Columns.Name, entity => entity.Name },
            };
            var entities = new[]
            {
                new Entity { Id = 1, Name = "a" },
                new Entity { Id = 2, Name = "a" },
                new Entity { Id = 1, Name = "b" },
            };

            var actual = entities.AsEnumerable().OrderBy(rules, Columns.Id, false).ThenBy(rules, Columns.Name, true);

            var expected = new[]
            {
                new Entity { Id = 1, Name = "b" },
                new Entity { Id = 1, Name = "a" },
                new Entity { Id = 2, Name = "a" },
            };

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void OrderQueryableByTwoFields()
        {
            var rules = new OrderingRules<Columns, Entity>
            {
                { Columns.Id, entity => entity.Id },
                { Columns.Name, entity => entity.Name },
            };
            var entities = new[]
            {
                new Entity { Id = 1, Name = "a" },
                new Entity { Id = 2, Name = "a" },
                new Entity { Id = 1, Name = "b" },
            };

            var actual = entities.AsQueryable().OrderBy(rules, Columns.Id, false).ThenBy(rules, Columns.Name, true);

            var expected = new[]
            {
                new Entity { Id = 1, Name = "b" },
                new Entity { Id = 1, Name = "a" },
                new Entity { Id = 2, Name = "a" },
            };

            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
