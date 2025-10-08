using System.Collections;

namespace lab2
{
    public class Node<T>(T value)
    {
        public T Value { get; set; } = value;
        public Node<T>? Left { get; set; } = null;
        public Node<T>? Right { get; set; } = null;
    }

    public class BinaryTree<T> : IEnumerable<T> where T : class, IComparable<T>
    {
        public Node<T> Root { get; }

        public BinaryTree(T value) => this.Root = new Node<T>(value);

        public BinaryTree(ICollection<T> values)
        {
            this.Root = new Node<T>(values.First());
            for (int i = 1; i < values.Count; i++)
            {
                this.Insert(values.ElementAt(i));
            }
        }

        public void Insert(T value) => this.Insert(value, this.Root);

        private Node<T> Insert(T value, Node<T>? parent)
        {
            if (parent == null) return new Node<T>(value);

            int compared = value.CompareTo(parent.Value);
            if (compared < 0)
            {
                parent.Left = this.Insert(value, parent.Left);
            }
            else if (compared > 0)
            {
                parent.Right = this.Insert(value, parent.Right);
            }

            return parent;
        }

        public IEnumerator<T> GetEnumerator() => TraverseInOrder(this.Root).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private static IEnumerable<T> TraverseInOrder(Node<T> parent)
        {
            if (parent.Left != null)
            {
                foreach (var i in TraverseInOrder(parent.Left))
                    yield return i;
            }

            yield return parent.Value;

            if (parent.Right != null)
            {
                foreach (var i in TraverseInOrder(parent.Right))
                    yield return i;
            }
        }
    }
}