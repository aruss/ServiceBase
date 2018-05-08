namespace ServiceBase.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a node in a tree.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the encapsuled data.
    /// </typeparam>
    public class Node<T> : IEnumerable<Node<T>>
    {
        #region Private Fields

        /// <summary>
        /// Saves a weak reference to the tree.
        /// </summary>
        /// <remarks>
        /// Since the tree already references the node a strong reference
        /// would be cyclic and result in a memory leak.
        /// </remarks>
        private WeakReference<Tree<T>> _tree;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new node with the passed data with the passed
        /// parent.
        /// </summary>
        /// <param name="data">
        /// The data which is referenced by this node.
        /// </param>
        /// <param name="parent">
        /// The parent node of the current node.
        /// </param>
        /// <param name="tree">
        /// The tree where the current node should belong to.
        /// </param>
        public Node(
            T data,
            Node<T> parent,
            Tree<T> tree)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (tree == null)
            {
                throw new ArgumentNullException("tree");
            }

            this.Data = data;
            this.Parent = parent;
            this.Children = Array.AsReadOnly(new Node<T>[0]);
            this._tree = new WeakReference<Tree<T>>(tree);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the referenced data.
        /// </summary>
        public T Data
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public Node<T> Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the tree the current node belongs to.
        /// </summary>
        public Tree<T> Tree
        {
            get
            {
                Tree<T> tree;
                this._tree.TryGetTarget(out tree);

                return tree;
            }
        }

        /// <summary>
        /// Gets or sets the collection of child nodes.
        /// </summary>
        public ReadOnlyCollection<Node<T>> Children
        {
            get;
            internal set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the enumerator instance.
        /// </summary>
        /// <returns>
        /// Enumerator over the children of the current node.
        /// </returns>
        public IEnumerator<Node<T>> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator instance.
        /// </summary>
        /// <returns>
        /// Enumerator over the children of the current node.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        #endregion
    }
}
