using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BoxApi.Common.Tests
{
    public class StringEnumTests
    {
        #region Passing Case
        [Fact]
        public void IsValid_Should_return_true_When_the_string_tested_is_part_of_the_enum()
        {
            #region Arrange & Act
            bool isValid = StringEnumTestClass.IsValid("A");
            #endregion

            #region Assert
            Assert.True(isValid);
            #endregion
        }

        [Fact]
        public void AllIsValid_Should_return_true_When_the_list_tested_contains_only_elements_in_the_enum()
        {
            #region Arrange
            List<string> completeEnumList = new List<string> { "A", "B", "C" };
            List<string> partialEnumList = new List<string> { "A", "C" };
            #endregion

            #region Act
            bool isCompleteEnumListValid = StringEnumTestClass.AllIsValid(completeEnumList);
            bool isPartialEnumListValid = StringEnumTestClass.AllIsValid(partialEnumList);
            #endregion

            #region Assert
            Assert.True(isCompleteEnumListValid);
            Assert.True(isPartialEnumListValid);
            #endregion
        }

        [Fact]
        public void RetrieveAll_Should_return_a_List_with_all_the_values_possible_in_the_enum()
        {
            #region Arrange
            List<string> enumList = new List<string> { "A", "B", "C" };
            #endregion

            #region Act
            IReadOnlyCollection<string> returnedList = StringEnumTestClass.RetrieveAll();
            #endregion

            #region Assert
            Assert.Equal(enumList.Count, returnedList.Count);
            Assert.True(enumList.SequenceEqual(returnedList));
            #endregion
        }
        #endregion

        #region Non Passing Case
        [Fact]
        public void IsValid_Should_return_false_When_the_string_tested_is_not_part_of_the_enum()
        {
            #region Arrange & Act
            bool isValid = StringEnumTestClass.IsValid("D");
            #endregion

            #region Assert
            Assert.False(isValid);
            #endregion
        }

        [Fact]
        public void AllIsValid_Should_return_false_When_the_list_tested_contains_a_value_not_in_the_enum()
        {
            #region Arrange
            List<string> nonValidEnumList = new List<string> { "A", "D" };
            #endregion

            #region Act
            bool allIsValid = StringEnumTestClass.AllIsValid(nonValidEnumList);
            #endregion

            #region Assert
            Assert.False(allIsValid);
            #endregion
        }
        #endregion
    }

    public class StringEnumTestClass
    {
        private static readonly StringEnum.StringEnum _allMembers = new StringEnum.StringEnum(typeof(StringEnumTestClass));

        public const string propertyA = "A";
        public const string propertyB = "B";
        public const string propertyC = "C";

        public static bool IsValid(string stringToValidate)
            => _allMembers.IsValid(stringToValidate);

        public static bool AllIsValid(ICollection<string> listToValidate)
            => _allMembers.AllIsValid(listToValidate);

        public static IReadOnlyCollection<string> RetrieveAll()
            => _allMembers.RetrieveAll();
    }
}
