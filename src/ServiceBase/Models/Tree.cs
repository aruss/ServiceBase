namespace ServiceBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a tree structure.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the encapsuled data.
    /// </typeparam>
    public class Tree<T>
    {
        #region Private Fields

        /// <summary>
        /// Saves the instance of indexes and their matches.
        /// </summary>
        private Dictionary<object, List<Node<T>>> _indexes
            = new Dictionary<object, List<Node<T>>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}" /> class
        /// using the passed dictionary which maps parents to children.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// None or multiple root nodes where found.
        /// </exception>
        /// <param name="dictionary">
        /// The dictionary which maps parents to children. The root element
        /// should be placed under the null key. Multiple keys aren't
        /// supported.
        /// </param>
        public Tree(NullableDictionary<T, List<T>> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            List<T> roots;

            if (!dictionary.TryGetValue(default(T), out roots) ||
                roots.Count != 1)
            {
                throw new ArgumentException(
                    "The dictionary has to contain exactly one element without parent (the root node, key must be null)");
            }

            this.InitByDictionaryAndRoot(dictionary, roots.First());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}" /> class
        /// using the passed dictionary which maps parents to children.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary which maps parents to children.
        /// </param>
        /// <param name="root">
        /// The root element of the tree.
        /// </param>
        public Tree(Dictionary<T, List<T>> dictionary, T root)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            this.InitByDictionaryAndRoot(dictionary, root);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}" /> class.
        /// </summary>
        protected Tree(T rootObject)
        {
            if (rootObject == null)
            {
                throw new ArgumentNullException("rootObject");
            }

            this.Root = new Node<T>(rootObject, null, this);
        }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="Tree{T}" />
        /// class.
        /// </summary>
        private Tree()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Saves the root node of the current tree.
        /// </summary>
        public Node<T> Root
        {
            get;
            private set;
        }

        /// <summary>
        /// Retrieves a list of nodes from the index.
        /// </summary>
        /// <param name="key">
        /// The key used to identify the nodes.
        /// </param>
        /// <returns>
        /// A list of nodes for the passed key or null if none were indexed.
        /// </returns>
        public List<Node<T>> this[object key]
        {
            get
            {
                List<Node<T>> nodes;

                this._indexes.TryGetValue(key, out nodes);

                return nodes ?? new List<Node<T>>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indexes all nodes using the passed expression.
        /// </summary>
        /// <param name="exp">
        /// A expression selecting a Property which should be used as index
        /// key.
        /// </param>
        public void IndexByProperty(Expression<Func<T, dynamic>> exp)
        {
            this.Index(exp.Compile(), this.Root);
        }

        /// <summary>
        /// Indexes all nodes using the passed function.
        /// </summary>
        /// <param name="exp">
        /// A function returning a value which should be used as index key.
        /// </param>
        public void IndexByFunc(Func<T, dynamic> func)
        {
            this.Index(func, this.Root);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the tree using the passed dictionary which maps parent
        /// to childs.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary containing all elements and their parent to child
        /// relationships.
        /// </param>
        /// <param name="root">
        /// The root element of the tree.
        /// </param>
        private void InitByDictionaryAndRoot(
            IDictionary<T, List<T>> dictionary, T root)
        {
            this.Root = new Node<T>(root, null, this);

            Action<Node<T>> addChildrenForNode = null;

            addChildrenForNode = (node) =>
            {
                List<T> children;

                if (!dictionary.TryGetValue(node.Data, out children))
                {
                    return;
                }

                List<Node<T>> nodes = new List<Node<T>>();

                foreach (T element in children)
                {
                    Node<T> subNode = new Node<T>(element, node, this);
                    nodes.Add(subNode);

                    addChildrenForNode(subNode);
                }

                node.Children = nodes.AsReadOnly();
            };

            addChildrenForNode(this.Root);
        }

        /// <summary>
        /// Indexes the passed node using the specified function.
        /// </summary>
        /// <param name="getter">
        /// The getter function selecting the key value.
        /// </param>
        /// <param name="node">
        /// The node which should be indexed.
        /// </param>
        private void Index(
            Func<T, dynamic> getter,
            Node<T> node)
        {
            object value = (object)getter(node.Data);

            if (!this._indexes.ContainsKey(value))
            {
                this._indexes[value] = new List<Node<T>>();
            }

            this._indexes[value].Add(node);

            foreach (Node<T> child in node.Children)
            {
                this.Index(getter, child);
            }
        }

        #endregion
    }
}
