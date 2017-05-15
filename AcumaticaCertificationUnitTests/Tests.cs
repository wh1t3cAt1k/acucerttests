using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using PX.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcumaticaCertificationUnitTests
{
	[TestClass]
	public class BqlTablesTests
	{
		public const string TESTED_ASSEMBLY_NAME = "BadDataAccessClass";

		/// <summary>
		/// Collects the data access classes from the assembly.
		/// </summary>
		private static IEnumerable<Type> CollectDataAccessClasses()
			=> Assembly
				.Load(TESTED_ASSEMBLY_NAME)
				.GetTypes()
				.Where(type => typeof(IBqlTable).IsAssignableFrom(type));

		/// <summary>
		/// Returns a value indicating whether the data access class
		/// contains any database-bound fields.
		/// </summary>
		private static bool IsDatabaseBound(Type dac)
		{
			PXGraph graph = new PXGraph();
			PXCache cache = graph.Caches[dac];

			return cache.Fields.Any(fieldName => cache
				.GetAttributesReadonly(fieldName)
				.OfType<PXDBFieldAttribute>()
				.Any());
		}

		private static IEnumerable<Type> CollectDatabaseBoundClasses()
			=> CollectDataAccessClasses().Where(dac => IsDatabaseBound(dac));

		private static IEnumerable<PropertyInfo> CollectDatabaseBoundPropertyFields()
			=> CollectDatabaseBoundClasses()
				.SelectMany(dac => dac.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				.Where(property =>
				{
					PXGraph graph = new PXGraph();
					PXCache cache = graph.Caches[property.DeclaringType];

					return cache
						.Fields
						.Where(fieldName => cache
							.GetAttributesReadonly(fieldName)
							.OfType<PXDBFieldAttribute>()
							.Any())
						.Contains(property.Name, StringComparer.OrdinalIgnoreCase);
				});

		private static bool IsFieldWithAttributeExists<T>(Type dac)
		{
			PXGraph graph = new PXGraph();
			PXCache cache = graph.Caches[dac];

			return cache.Fields.Any(fieldName => cache
				.GetAttributesReadonly(fieldName, true)
				.OfType<T>()
				.Any());
		}

		private static string Describe(IEnumerable<Type> dacs)
			=> string.Join(", ", dacs.Select(dac => dac.FullName));

		private static string Describe(IEnumerable<PropertyInfo> properties)
			=> string.Join(", ", properties.Select(property => $"{property.DeclaringType.Name}.{property.Name}"));

		[TestMethod]
		public void Test_Assembly_HasDataAccessClasses()
		{
			Assert.IsTrue(
				CollectDataAccessClasses().Any(),
				$"There are no data access classes in assembly {TESTED_ASSEMBLY_NAME}.");
		}

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveAtLeastOneKeyField()
		{
			IEnumerable<Type> boundClassesWithoutKeyFields =
				CollectDatabaseBoundClasses().Where(dac =>
				{
					PXGraph graph = new PXGraph();
					PXCache cache = graph.Caches[dac];

					return !cache.Fields.Any(fieldName => cache
						.GetAttributesReadonly(fieldName, true)
						.OfType<PXDBFieldAttribute>()
						.Any(attribute => attribute.IsKey == true));
				});

			Assert.IsTrue(
				!boundClassesWithoutKeyFields.Any(),
				$"The following classes are DB-bound but have no fields marked as key: {Describe(boundClassesWithoutKeyFields)}.");
		}

		public static void VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<TAttribute>()
		{
			IEnumerable<Type> boundClassesWithoutFieldsMarkedWithAttribute =
				CollectDatabaseBoundClasses().Where(
					dac => !IsFieldWithAttributeExists<TAttribute>(dac));

			Assert.IsTrue(
				!boundClassesWithoutFieldsMarkedWithAttribute.Any(),
				$"The following classes are DB-bound but have no field marked with {typeof(TAttribute).Name}: {Describe(boundClassesWithoutFieldsMarkedWithAttribute)}.");
		}

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveNoteIdField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXNoteAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveCreatedByIdField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBCreatedByIDAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveCreatedByScreenIDField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBCreatedByScreenIDAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveLastModifiedByIDField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBLastModifiedByIDAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveLastModifiedByScreenIDField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBLastModifiedByScreenIDAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveCreatedDateTimeField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBCreatedDateTimeAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveLastModifiedDateTimeField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBLastModifiedDateTimeAttribute>();

		[TestMethod]
		public void Test_AllBoundDataAccessClasses_HaveTimeStampField()
			=> VerifyDatabaseBoundClassesWithoutFieldsMarkedWith<PXDBTimestampAttribute>();

		[TestMethod]
		public void Test_AllBoundFieldsOfDataAccessClasses_HaveBothGetterAndSetter()
		{
			IEnumerable<PropertyInfo> boundPropertyFieldsWithoutBothGetterAndSetter =
				CollectDatabaseBoundPropertyFields()
					.Where(property => !property.CanRead || !property.CanWrite);

			Assert.IsTrue(
				!boundPropertyFieldsWithoutBothGetterAndSetter.Any(),
				$"The following DB-bound property fields either have no getter or no setter: {Describe(boundPropertyFieldsWithoutBothGetterAndSetter)}.");
		}

		[TestMethod]
		public void Test_AllBoundFieldsOfDataAccessClasses_HaveNullableType()
		{
			IEnumerable<PropertyInfo> boundPropertyFieldsWithNonNullableType =
				CollectDatabaseBoundPropertyFields()
					.Where(property => 
						property.PropertyType.IsValueType 
						&& Nullable.GetUnderlyingType(property.PropertyType) == null);

			Assert.IsTrue(
				!boundPropertyFieldsWithNonNullableType.Any(),
				$"The following DB-bound property fields have a non-nullable type: {Describe(boundPropertyFieldsWithNonNullableType)}.");
		}

		[TestMethod]
		public void Test_AllDataAccessClasses_AreSerializable()
		{
			IEnumerable<Type> nonSerializableDataAccessClasses =
				CollectDataAccessClasses().Where(dac => !dac.IsSerializable);

			Assert.IsTrue(
				!nonSerializableDataAccessClasses.Any(),
				$"The following data access classes are not serializable: {Describe(nonSerializableDataAccessClasses)}.");
		}
	}
}
