using System.Collections;

namespace LeanSharp.Tests
{
    public class EqualityComparerStub : IEqualityComparer
    {
        public new bool Equals(object ob1, object ob2) => ob1.Equals(ob2);

        public int GetHashCode(object obj) => obj.GetHashCode();
    }
}
