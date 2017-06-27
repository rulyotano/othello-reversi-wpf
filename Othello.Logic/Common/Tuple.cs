using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.Logic.Common
{
    public class MyTuple<T, U>:ICloneable
    {
        public MyTuple()
        { }

        public MyTuple(T value1, U value2)
            : this()
        {
            Item1 = value1;
            Item2 = value2;
        }

        #region Item1

        private T _item1;


        public T Item1
        {
            get { return _item1; }
            set { _item1 = value; }
        }

        #endregion

        #region Item2

        private U _item2;


        public U Item2
        {
            get { return _item2; }
            set { _item2 = value; }
        }

        #endregion

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyTuple<T, U>)obj);
        }

        protected bool Equals(MyTuple<T, U> other)
        {
            return EqualityComparer<T>.Default.Equals(_item1, other._item1) &&
                   EqualityComparer<U>.Default.Equals(_item2, other._item2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_item1) * 397) ^
                       EqualityComparer<U>.Default.GetHashCode(_item2);
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new MyTuple<T, U>(Item1, Item2);
        }

        #endregion

        public static bool operator ==(MyTuple<T, U> tuple1, MyTuple<T, U> tuple2)
        {
            if (ReferenceEquals(tuple1, tuple2)) return true;
            return !ReferenceEquals(tuple1, null) && tuple1.Equals((object)tuple2);
        }

        public static bool operator !=(MyTuple<T, U> tuple1, MyTuple<T, U> tuple2)
        {
            return !(tuple1 == tuple2);
        }
    }
}
