using PX.Data;
using System;

namespace BadDataAccessClass
{
	[Serializable]
	public class GoodClassBase : IBqlTable { }

	/// <summary>
	/// This class should produce no failures in the test project.
	/// </summary>
	public class GoodClass
	{
		public abstract class recordID : IBqlField { }

		[PXDBInt(IsKey = true)]
		public virtual int? RecordID
		{
			get;
			set;
		}

		public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID
		{
			get;
			set;
		}

		public abstract class createdByID : IBqlField { }

		[PXDBCreatedByID]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}

		public abstract class createdByScreenID : IBqlField { }

		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID
		{
			get;
			set;
		}

		public abstract class lastModifiedByID : IBqlField { }

		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}

		public abstract class lastModifiedByScreenID : IBqlField { }

		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID
		{
			get;
			set;
		}

		public abstract class createdDateTime : IBqlField { }

		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}

		public abstract class lastModifiedDateTime : IBqlField { }

		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}

		public abstract class tStamp : IBqlField { }

		[PXDBTimestamp]
		public virtual byte[] Tstamp
		{
			get;
			set;
		}
	}
}
