// From http://flee.codeplex.com/

using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

#pragma warning disable 0649, 0169, 0414

namespace Expressions.Test.VisualBasicLanguage.BulkTests
{
    public struct Mouse
    {
        public string S;
        public int I;
        public DateTime DT;

        public static DateTime SharedDT;
        public Mouse(string s, int i)
        {
            this.S = s;
            this.I = i;
            DT = new DateTime(2007, 1, 1);
        }

        public int GetI()
        {
            return I;
        }

        public int GetYear(DateTime dt)
        {
            return dt.Year;
        }

        public DateTime this[int i]
        {
            get { return this.DT; }
        }

        public DateTime this[int i, string s]
        {
            get { return this.DT; }
        }

        public int this[string s, int i]
        {
            get { return i * 2; }
        }
    }

    public class Monitor
    {
        public int I;
        public string S;
        public DateTime DT;

        public static string SharedString = "string";
        public Monitor()
        {
            I = 900;
            S = "monitor";
            DT = new DateTime(2007, 1, 1);
        }

        public int GetI()
        {
            return I;
        }

        public static implicit operator double(Monitor value)
        {
            return 1.0;
        }

        public DateTime this[int i]
        {
            get { return this.DT; }
        }

        public DateTime this[double d, string s]
        {
            get { return this.DT; }
        }

        public int this[string s, params int[] args]
        {
            get { return -100; }
        }
    }

    public struct Keyboard
    {
        public Mouse StructA;
        public Monitor ClassA;
    }

    public class ExpressionOwner
    {
        private double DoubleA;
        private float SingleA;
        private Int32 Int32A;
        private string StringA;
        private bool BoolA;
        private Type TypeA;
        private byte ByteA;
        private byte ByteB;
        private sbyte SByteA;
        private Int16 Int16A;
        private UInt16 UInt16A;
        private int[] IntArr = {
		100,
		200,
		300
	};
        private string[] StringArr = {
		"a",
		"b",
		"c"
	};
        private double[] DoubleArr = {
		1.1,
		2.2,
		3.3
	};
        private bool[] BoolArr = {
		true,
		false,
		true
	};
        private char[] CharArr = { '.' };
        private DateTime[] DateTimeArr = { new DateTime(2007, 7, 1) };
        private IList List;
        private System.Collections.Specialized.StringDictionary StringDict;
        private Guid GuidA;
        private DateTime DateTimeA;
        private ICloneable ICloneableA;
        private ICollection ICollectionA;
        private System.Version VersionA;
        private TestStruct StructA;
        private IComparable IComparableA;
        private object ObjectIntA;
        private object ObjectStringA;
        private ValueType ValueTypeStructA;
        private Exception ExceptionA;
        private Exception ExceptionNull;
        private IComparable IComparableString;
        private IComparable IComparableNull;
        private ICloneable ICloneableArray;
        private System.Delegate DelegateANull;
        private System.Array ArrayA;
        private AppDomainInitializer DelegateA;
        private System.Text.ASCIIEncoding[] AsciiEncodingArr = {
		
	};
        private System.Text.Encoding EncodingA;
        private Keyboard KeyboardA;
        private decimal DecimalA;
        private decimal DecimalB;
        private object NullField;
        private object InstanceA;
        private ArrayList InstanceB;
        private Hashtable Dict;
        private Dictionary<string, int> GenericDict;

        private DataRow Row;
        public ExpressionOwner()
	{
		this.InstanceB = new ArrayList();
		this.InstanceA = this.InstanceB;
		this.NullField = null;
		this.DecimalA = 100;
		this.DecimalB = 0.25m;
		this.KeyboardA = new Keyboard();
		this.KeyboardA.StructA = new Mouse("mouse", 123);
		this.KeyboardA.ClassA = new Monitor();
		this.EncodingA = System.Text.Encoding.ASCII;
		this.DelegateA = DoAction;
		this.ICloneableArray = new string[0];
		this.ArrayA = new string[0];
		this.DelegateANull = null;
		this.IComparableNull = null;
		this.IComparableString = "string";
		this.ExceptionA = new ArgumentException();
		this.ExceptionNull = null;
		this.ValueTypeStructA = new TestStruct();
		this.ObjectStringA = "string";
		this.ObjectIntA = 100;
		this.IComparableA = 100.25;
		this.StructA = new TestStruct();
		this.VersionA = new System.Version(1, 1, 1, 1);
		this.ICloneableA = "abc";
		this.GuidA = Guid.NewGuid();
		this.List = new ArrayList();
		this.List.Add("a");
		this.List.Add(100);
		this.StringDict = new System.Collections.Specialized.StringDictionary();
		this.StringDict.Add("key", "value");
		this.DoubleA = 100.25;
		this.SingleA = 100.25f;
		this.Int32A = 100000;
		this.StringA = "string";
		this.BoolA = true;
		this.TypeA = typeof(string);
		this.ByteA = 50;
		this.ByteB = 2;
		this.SByteA = -10;
		this.Int16A = -10;
		this.UInt16A = 100;
		this.DateTimeA = new DateTime(2007, 7, 1);
		this.GenericDict = new Dictionary<string, int>();
		this.GenericDict.Add("a", 100);
		this.GenericDict.Add("b", 100);

		this.Dict = new Hashtable();
		this.Dict.Add(100, null);
		this.Dict.Add("abc", null);

		DataTable dt = new DataTable();
		dt.Columns.Add("ColumnA", typeof(int));

		dt.Rows.Add(100);

		this.Row = dt.Rows[0];
	}


        private void DoAction(string[] args)
        {
        }


        public void DoStuff()
        {
        }

        public int DoubleIt(int i)
        {
            return i * 2;
        }

        public string FuncString()
        {
            return "abc";
        }

        public static int SharedFuncInt()
        {
            return 100;
        }

        private string PrivateFuncString()
        {
            return "abc";
        }

        public static int PrivateSharedFuncInt()
        {
            return 100;
        }

        public DateTime GetDateTime()
        {
            return this.DateTimeA;
        }

        public int ThrowException()
        {
            throw new InvalidOperationException("Should not be thrown!");
        }

        public ArrayList Func1(ArrayList al)
        {
            return al;
        }

        public string ReturnNullString()
        {
            return null;
        }

        public int Sum(int i)
        {
            return 1;
        }

        public int Sum(int i1, int i2)
        {
            return 2;
        }

        public int Sum(int i1, double i2)
        {
            return 3;
        }

        public int Sum(params int[] args)
        {
            return 4;
        }

        public int Sum2(int i1, double i2)
        {
            return 3;
        }

        public int Sum2(params int[] args)
        {
            return 4;
        }

        public int Sum4(params int[] args)
        {
            int sum = 0;

            foreach (int i in args)
            {
                sum += i;
            }

            return sum;
        }

        public int ParamArray1(string a, params object[] args)
        {
            return 1;
        }

        public int ParamArray2(params DateTime[] args)
        {
            return 1;
        }

        public int ParamArray3(params DateTime[] args)
        {
            return 1;
        }

        public int ParamArray3()
        {
            return 2;
        }

        public int ParamArray4(params int[] args)
        {
            return 1;
        }

        public int ParamArray4(params object[] args)
        {
            return 2;
        }

        public double DoubleAProp
        {
            get { return this.DoubleA; }
        }

        private Int32 Int32AProp
        {
            get { return this.Int32A; }
        }

        static internal string SharedPropA
        {
            get { return "sharedprop"; }
        }
    }

    internal class AccessTestExpressionOwner
    {
        private int PrivateField1;
        //[Ciloci.Flee.ExpressionOwnerMemberAccess(true)]
        private int PrivateField2;
        //[Ciloci.Flee.ExpressionOwnerMemberAccess(false)]
        private int PrivateField3;
        public int PublicField1;
    }

    internal class OverloadTestExpressionOwner
    {

        public System.IO.MemoryStream A;

        public object B;
        public int ValueType1(int arg)
        {
            return 1;
        }

        public int ValueType1(float arg)
        {
            return 2;
        }

        public int ValueType1(double arg)
        {
            return 3;
        }

        public int ValueType1(decimal arg)
        {
            return 4;
        }

        public int ValueType2(float arg)
        {
            return 1;
        }

        public int ValueType2(double arg)
        {
            return 2;
        }

        public int ValueType3(double arg)
        {
            return 1;
        }

        public int ValueType3(decimal arg)
        {
            return 2;
        }

        public int ReferenceType1(object arg)
        {
            return 1;
        }

        public int ReferenceType1(string arg)
        {
            return 2;
        }

        public int ReferenceType2(object arg)
        {
            return 1;
        }

        public int ReferenceType2(System.IO.MemoryStream arg)
        {
            return 2;
        }

        public int ReferenceType3(object arg)
        {
            return 1;
        }

        public int ReferenceType3(IComparable arg)
        {
            return 2;
        }

        public int ReferenceType4(IFormattable arg)
        {
            return 1;
        }

        public int ReferenceType4(IComparable arg)
        {
            return 2;
        }

        public int Value_ReferenceType1(int arg)
        {
            return 1;
        }

        public int Value_ReferenceType1(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType2(ValueType arg)
        {
            return 1;
        }

        public int Value_ReferenceType2(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType3(IComparable arg)
        {
            return 1;
        }

        public int Value_ReferenceType3(object arg)
        {
            return 2;
        }

        public int Value_ReferenceType4(IComparable arg)
        {
            return 1;
        }

        public int Value_ReferenceType4(IFormattable arg)
        {
            return 2;
        }

        public int Access1(object arg)
        {
            return 1;
        }

        //[ExpressionOwnerMemberAccess(false)]
        public int Access1(string arg)
        {
            return 2;
        }

        //[ExpressionOwnerMemberAccess(false)]
        public int Access2(object arg)
        {
            return 1;
        }

        //[ExpressionOwnerMemberAccess(false)]
        public int Access2(string arg)
        {
            return 2;
        }

        public int Multiple1(float arg1, double arg2)
        {
            return 1;
        }

        public int Multiple1(int arg1, double arg2)
        {
            return 2;
        }
    }

    internal class TestImport
    {
        public static int DoStuff()
        {
            return 100;
        }
    }

    internal struct TestStruct : IComparable
    {


        private int MyA;
        public TestStruct(int a)
        {
            MyA = a;
        }

        public int DoStuff()
        {
            return 100;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }
    }

    /// <summary> 
    /// This is our custom provider. It simply provides a custom type descriptor 
    /// and delegates all its other tasks to its parent 
    /// </summary> 
    internal sealed class UselessTypeDescriptionProvider : TypeDescriptionProvider
    {
        /// <summary> 
        /// Constructor 
        /// </summary> 
        internal UselessTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        /// <summary> 
        /// Create and return our custom type descriptor and chain it with the original 
        /// custom type descriptor 
        /// </summary> 
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new UselessCustomTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
        }
    }

    /// <summary> 
    /// This is our custom type descriptor. It creates a new property and returns it along 
    /// with the original list 
    /// </summary> 
    internal sealed class UselessCustomTypeDescriptor : CustomTypeDescriptor
    {
        /// <summary> 
        /// Constructor 
        /// </summary> 
        internal UselessCustomTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
        }

        /// <summary> 
        /// This method add a new property to the original collection 
        /// </summary> 
        public override PropertyDescriptorCollection GetProperties()
        {
            // Enumerate the original set of properties and create our new set with it 
            PropertyDescriptorCollection originalProperties = base.GetProperties();
            List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in originalProperties)
            {
                newProperties.Add(pd);
            }

            // Create a new property and add it to the collection 
            newProperties.Add(new CustomPropertyDescriptor());

            // Finally return the list 
            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
        }
    }

    internal class CustomPropertyDescriptor : PropertyDescriptor
    {

        public CustomPropertyDescriptor()
            : base("Name", null)
        {
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override System.Type ComponentType
        {
            get { return typeof(int); }
        }

        public override object GetValue(object component)
        {
            return "prop!";
        }

        public override bool IsReadOnly
        {

            get { return false; }
        }

        public override System.Type PropertyType
        {
            get { return typeof(string); }
        }


        public override void ResetValue(object component)
        {
        }


        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

    public class NestedA
    {

        public class NestedPublicB
        {

            public static int DoStuff()
            {
                return 100;
            }
        }

        internal class NestedInternalB
        {

            public static int DoStuff()
            {
                return 100;
            }
        }
    }

}
