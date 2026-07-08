using System;

namespace Warzone.Content
{
    public static class ContentValidator
    {
        public static void NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}


