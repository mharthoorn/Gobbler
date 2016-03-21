namespace System
{ 
    public static class ObjectExtensions
    {
        public static bool Cast<T>(this object x, out T casted)
        {
            if (x is T)
            {
                casted = (T)x;
                return true;
            }
            else
            {
                casted = default(T);
                return false;
            }
        }
        public static T Casted<T>(this object x)
        {
            if (x is T)
            {
                T casted = (T)x;
                return casted;
            }
            else throw new InvalidCastException("Object could not be casted");
        }
    }
}
