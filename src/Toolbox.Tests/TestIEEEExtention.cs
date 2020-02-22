using System;
using FluentAssertions;
using Xunit;

namespace Toolbox.IEEE.Test
{
    public class TestToolboxExtentionsIEEE
    {

        [Theory]
        [InlineData(float.PositiveInfinity, "7F800000")]
        [InlineData(float.NegativeInfinity, "FF800000")]
        [InlineData(float.MaxValue,         "7F7FFFFF")]
        [InlineData(float.MinValue,         "FF7FFFFF")]
        [InlineData(float.Epsilon,          "00000001")]
        [InlineData(-float.Epsilon,         "80000001")]
        [InlineData(0,                      "00000000")]

        public void IEEE_Float_To_4Byte_String_BigEndian(float inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString();

            //Assert
            actualString.Should().Be(expectedString);
        }

        [Theory]
        [InlineData(float.PositiveInfinity, "0000807F")]
        [InlineData(float.NegativeInfinity, "000080FF")]
        [InlineData(float.MaxValue,         "FFFF7F7F")]
        [InlineData(float.MinValue,         "FFFF7FFF")]
        [InlineData(float.Epsilon,          "01000000")]
        [InlineData(-float.Epsilon,         "01000080")]
        [InlineData(0,                      "00000000")]

        public void IEEE_Float_To_4Byte_String_LittleEndian(float inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString(true);

            //Assert
            actualString.Should().Be(expectedString);
        }

        [Fact]
        public void IEEE_NaNFloat_To_4Byte_String_BigEndian()
        {
            //Arrange 
            string NanString = "FFC00000";
            string actualString;

            //Act
            actualString = float.NaN.ToIEEEString();

            //Assert
            actualString.Should().Be(NanString);
        }

        [Fact]
        public void IEEE_NaNFloat_To_4Byte_String_LittleEndian()
        {
            //Arrange 
            string NanString = "0000C0FF";
            string actualString;

            //Act
            actualString = float.NaN.ToIEEEString(true);

            //Assert
            actualString.Should().Be(NanString);
        }

        [Theory]
        [InlineData("7F800000", float.PositiveInfinity)]
        [InlineData("FF800000", float.NegativeInfinity)]
        [InlineData("7F7FFFFF", float.MaxValue)]
        [InlineData("FF7FFFFF", float.MinValue)]
        [InlineData("00000001", float.Epsilon)]
        [InlineData("80000001", -float.Epsilon)]
        [InlineData("00000000", 0)]
        [InlineData("80000000", 0)]
        public void IEEE_4Bytes_String_To_Float_BigEndian(string inputString, float expectedFloat)
        {
            //Arrange
            float actualFloat;

            //Act
            actualFloat = inputString.ToFloat();

            //Assert
            actualFloat.Should().Be(expectedFloat);
        }

        [Theory]
        [InlineData("0000807F", float.PositiveInfinity)]
        [InlineData("000080FF", float.NegativeInfinity)]
        [InlineData("FFFF7F7F", float.MaxValue)]
        [InlineData("FFFF7FFF", float.MinValue)]
        [InlineData("01000000", float.Epsilon)]
        [InlineData("01000080", -float.Epsilon)]
        [InlineData("00000000", 0)]
        [InlineData("00000080", 0)]
        public void IEEE_4Bytes_String_To_Float_LittleEndian(string inputArray, float expectedFloat)
        {
            //Arrange
            float actualFloat;

            //Act
            actualFloat = inputArray.ToFloat(true);

            //Assert
            actualFloat.Should().Be(expectedFloat);
        }

        [Fact]
        public void IEEE_NaNString_To_Float_BigEndian()
        {
            //Arrange 
            string NanString = "FFC00000";
            float actualValue;

            //Act
            actualValue = NanString.ToFloat();

            //Assert
            Single.IsNaN(actualValue).Should().BeTrue();
        }

        [Fact]
        public void IEEE_NaNString_To_Float_LittleEndian()
        {
            //Arrange 
            string NanString = "0000C0FF";
            float actualValue;

            //Act
            actualValue = NanString.ToFloat(true);

            //Assert
            Single.IsNaN(actualValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(double.PositiveInfinity, "7FF0000000000000")]
        [InlineData(double.NegativeInfinity, "FFF0000000000000")]
        [InlineData(double.MaxValue, "7FEFFFFFFFFFFFFF")]
        [InlineData(double.MinValue, "FFEFFFFFFFFFFFFF")]
        [InlineData(double.Epsilon, "0000000000000001")]
        [InlineData(-double.Epsilon, "8000000000000001")]
        [InlineData(0, "0000000000000000")]
        public void IEEE_Float_To_8Byte_String_BigEndian(double inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString();

            //Assert
            actualString.Should().Be(expectedString);
        }

        [Theory]
        [InlineData(double.PositiveInfinity, "000000000000F07F")]
        [InlineData(double.NegativeInfinity, "000000000000F0FF")]
        [InlineData(double.MaxValue, "FFFFFFFFFFFFEF7F")]
        [InlineData(double.MinValue, "FFFFFFFFFFFFEFFF")]
        [InlineData(double.Epsilon, "0100000000000000")]
        [InlineData(-double.Epsilon, "0100000000000080")]
        [InlineData(0, "0000000000000000")]

        public void IEEE_Float_To_8Byte_String_LittleEndian(double inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString(true);

            //Assert
            actualString.Should().Be(expectedString);
        }

        [Fact]
        public void IEEE_NaNString_To_Double_BigEndian()
        {
            //Arrange 
            string NanString = "7FF8000000000000";
            double actualValue;

            //Act
            actualValue = NanString.ToDouble();

            //Assert
            double.IsNaN(actualValue).Should().BeTrue();
        }

        [Fact]
        public void IEEE_NaNString_To_Double_LittleEndian()
        {
            //Arrange 
            string NanString = "000000000000F87F";
            double actualValue;

            //Act
            actualValue = NanString.ToDouble(true);

            //Assert
            double.IsNaN(actualValue).Should().BeTrue();
        }

        [Theory]
        [InlineData("7FF0000000000000", double.PositiveInfinity)]
        [InlineData("FFF0000000000000", double.NegativeInfinity)]
        [InlineData("7FEFFFFFFFFFFFFF", double.MaxValue)]
        [InlineData("FFEFFFFFFFFFFFFF", double.MinValue)]
        [InlineData("0000000000000001", double.Epsilon)]
        [InlineData("8000000000000001", -double.Epsilon)]
        public void IEEE_8Byte_String_To_Float_BigEndian(string inputString, double expectedDouble)
        {
            //Arrange
            double actualDouble;

            //Act
            actualDouble = inputString.ToDouble();

            //Assert
            actualDouble.Should().Be(expectedDouble);
        }

        [Theory]
        [InlineData("000000000000F07F", double.PositiveInfinity)]
        [InlineData("000000000000F0FF", double.NegativeInfinity)]
        [InlineData("FFFFFFFFFFFFEF7F", double.MaxValue)]
        [InlineData("FFFFFFFFFFFFEFFF", double.MinValue)]
        [InlineData("0100000000000000", double.Epsilon)]
        [InlineData("0100000000000080", -double.Epsilon)]
        [InlineData("0000000000000000", 0)]
        [InlineData("0000000000000080", 0)]
        public void IEEE_8Byte_String_To_Float_LittleEndian(string inputString, double expectedDouble)
        {
            //Arrange
            double actualDouble;

            //Act
            actualDouble = inputString.ToDouble(true);

            //Assert
            actualDouble.Should().Be(expectedDouble);
        }

        [Fact]
        public void IEEE_NaNDouble_To_8Byte_String_BigEndian()
        {
            //Arrange 
            string NanString = "FFF8000000000000";
            string actualString;

            //Act
            actualString = double.NaN.ToIEEEString();

            //Assert
            actualString.Should().Be(NanString);
        }

        [Fact]
        public void IEEE_NaNDouble_To_8Byte_String_LittleEndian()
        {
            //Arrange 
            string NanString = "000000000000F8FF";
            string actualString;

            //Act
            actualString = double.NaN.ToIEEEString(true);

            //Assert
            actualString.Should().Be(NanString);
        }
    }
}
