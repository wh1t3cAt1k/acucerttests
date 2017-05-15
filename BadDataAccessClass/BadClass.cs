using PX.Data;

namespace BadDataAccessClass
{
	/// <summary>
	/// This class should produce all the failures in the test project.
	/// </summary>
	// Should fail: the class is not serializable.
	// -
	public class BadClass : IBqlTable
	{
		public abstract class recordID : IBqlField { }

		// Should fail: there is no key field in the class.
		// -
		[PXDBInt]
		public int? RecordID
		{
			get;
			set;
		}

		// Should fail: no note id field.
		// -

		// Should fail: no created by id field.
		// -

		// Should fail: no created by screen id field.
		// -

		// Should fail: no last modified by id field.
		// -

		// Should fail: no created datetime field.
		// -

		// Should fail: no last modified datetime field.
		// -

		// Should fail: no timestamp field.
		// -

		// Should fail: the property field has no setter.
		// -
		public abstract class boundFieldWithoutSetter : IBqlField { }

		[PXDBString]
		public string BoundFieldWithoutSetter
		{
			get;
		}

		// Should fail: the property field has no setter.
		// -
		public abstract class boundFieldWithoutGetter : IBqlField { }

		[PXDBString]
		public string BoundFieldWithoutGetter
		{
			set { }
		}

		// Should fail: the property field has non-nullable type.
		// -
		public abstract class boundFieldWithNonNullableType : IBqlField { }

		[PXDBInt]
		public int BoundFieldWithNonNullableType
		{
			get;
			set;
		}
	}
}
