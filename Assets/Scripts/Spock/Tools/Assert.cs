#define DEBUG

using System; 
using System.Diagnostics;

namespace Spock {

	public class Assert { 

	    [Conditional("DEBUG")] 
	    public static void True(string message, bool condition) { 
	        if (!condition) 
	            throw new Exception("Assert.True failed: " + message); 
	    }
		
		[Conditional("DEBUG")] 
		public static void False(string message, bool condition) { 
			if (condition) 
				throw new Exception("Assert.False failed: " + message); 
		}

	    [Conditional("DEBUG")]
	    public static void Null(string message, object obj) { 
	        if (obj != null) 
	            throw new Exception("Assert.Null failed: " + message);  
	    }

	    [Conditional("DEBUG")]
	    public static void NotNull(string message, object obj) { 
	        if (obj == null) 
	            throw new Exception("Assert.NotNull failed: " + message); 
	    }
		
		[Conditional("DEBUG")]
		public static void Is<T>(string message, object obj) { 
			if (obj == null || typeof(T) != obj.GetType()) 
				throw new Exception("Assert.Is<" + typeof(T).Name + "> failed: " + message + ((obj == null) ? " (Object is null)" : "")); 
		}
		
		[Conditional("DEBUG")]
		public static void Never(string message) { 
			throw new Exception("Assert.Never: " + message); 
		}
		
		[Conditional("DEBUG")]
		public static void Same(string message, System.Object obj1, System.Object obj2) { 
			if (!obj1.Equals(obj2)) 
				throw new Exception("Assert.Same: " + message); 
		}
		
		[Conditional("DEBUG")]
		public static void Different(string message, System.Object obj1, System.Object obj2) { 
			if (obj1.Equals(obj2)) 
				throw new Exception("Assert.Different: " + message); 
		}

        [Conditional("DEBUG")]
        public static void WithinBounds(string message, int lowerBound, int value, int upperBound) {
            if (value < lowerBound || value > upperBound)
                throw new Exception("Assert.WithinBounds: " + message + " (value = " + value + ")");
        }

        [Conditional("DEBUG")]
        public static void WithinBounds(string message, float lowerBound, float value, float upperBound) {
            if (value < lowerBound || value > upperBound)
                throw new Exception("Assert.WithinBounds: " + message + " (value = " + value + ")");
        }


    }

}