using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Toolbox.IEEE.Test
{
    [TestClass]
    public class TestEagleExtentionsIEEE
    {

        [DataTestMethod]
        [DataRow(float.PositiveInfinity, "7F800000", DisplayName = "Positive Infinity ")]
        [DataRow(float.NegativeInfinity, "FF800000", DisplayName = "Negative Infinity")]
        [DataRow(float.MaxValue,         "7F7FFFFF", DisplayName = "Maximum Positive Value")]
        [DataRow(float.MinValue,         "FF7FFFFF", DisplayName = "Maximum Negative Value")]
        [DataRow(float.Epsilon,          "00000001", DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow(-float.Epsilon,         "80000001", DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow(0,                      "00000000", DisplayName = "Zero")]

        public void IEEE_Float_To_4Byte_String_BigEndian(float inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString();

            //Assert
            Assert.AreEqual(expectedString, actualString);
            
        }


        [DataTestMethod]
        [DataRow(float.PositiveInfinity, "0000807F", DisplayName = "Positive Infinity ")]
        [DataRow(float.NegativeInfinity, "000080FF", DisplayName = "Negative Infinity")]
        [DataRow(float.MaxValue,         "FFFF7F7F", DisplayName = "Maximum Positive Value")]
        [DataRow(float.MinValue,         "FFFF7FFF", DisplayName = "Maximum Negative Value")]
        [DataRow(float.Epsilon,          "01000000", DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow(-float.Epsilon,         "01000080", DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow(0,                      "00000000", DisplayName = "Zero")]

        public void IEEE_Float_To_4Byte_String_LittleEndian(float inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString(true);

            //Assert
            Assert.AreEqual(expectedString, actualString);

        }

        [TestMethod]
        public void IEEE_NaNFloat_To_4Byte_String_BigEndian()
        {
            //Arrange 
            string NanString = "FFC00000";
            string actualString;

            //Act
            actualString = float.NaN.ToIEEEString();

            //Assert
            Assert.AreEqual(NanString, actualString);

        }

        [TestMethod]
        public void IEEE_NaNFloat_To_4Byte_String_LittleEndian()
        {
            //Arrange 
            string NanString = "0000C0FF";
            string actualString;

            //Act
            actualString = float.NaN.ToIEEEString(true);

            //Assert
            Assert.AreEqual(NanString, actualString);

        }

        [DataTestMethod]
        [DataRow("7F800000", float.PositiveInfinity, DisplayName = "Positive Infinity ")]
        [DataRow("FF800000", float.NegativeInfinity, DisplayName = "Negative Infinity")]
        [DataRow("7F7FFFFF", float.MaxValue, DisplayName = "Maximum Positive Value")]
        [DataRow("FF7FFFFF", float.MinValue, DisplayName = "Maximum Negative Value")]
        [DataRow("00000001", float.Epsilon, DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow("80000001", -float.Epsilon, DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow("00000000", 0, DisplayName = "Positive Zero")]
        [DataRow("80000000", 0, DisplayName = "Negative Zero")]
        public void IEEE_4Bytes_String_To_Float_BigEndian(string inputString, float expectedFloat)
        {
            //Arrange
            float actualFloat;

            //Act
            actualFloat = inputString.ToFloat();

            //Assert
            Assert.AreEqual(expectedFloat, actualFloat);
        }

        [DataTestMethod]
        [DataRow("0000807F", float.PositiveInfinity, DisplayName = "Positive Infinity ")]
        [DataRow("000080FF", float.NegativeInfinity, DisplayName = "Negative Infinity")]
        [DataRow("FFFF7F7F", float.MaxValue, DisplayName = "Maximum Positive Value")]
        [DataRow("FFFF7FFF", float.MinValue, DisplayName = "Maximum Negative Value")]
        [DataRow("01000000", float.Epsilon, DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow("01000080", -float.Epsilon, DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow("00000000", 0, DisplayName = "Positive Zero")]
        [DataRow("00000080", 0, DisplayName = "Negative Zero")]
        public void IEEE_4Bytes_String_To_Float_LittleEndian(string inputArray, float expectedFloat)
        {
            //Arrange
            float actualFloat;

            //Act
            actualFloat = inputArray.ToFloat(true);

            //Assert
            Assert.AreEqual(expectedFloat, actualFloat);

        }
        [TestMethod]
        public void IEEE_NaNString_To_Float_BigEndian()
        {
            //Arrange 
            string NanString = "FFC00000";
            float actualValue;

            //Act
            actualValue = NanString.ToFloat();

            //Assert
            Assert.IsTrue(Single.IsNaN(actualValue));

        }

        [TestMethod]
        public void IEEE_NaNString_To_Float_LittleEndian()
        {
            //Arrange 
            string NanString = "0000C0FF";
            float actualValue;

            //Act
            actualValue = NanString.ToFloat(true);

            //Assert
            Assert.IsTrue(Single.IsNaN(actualValue));

        }

        [DataTestMethod]
        [DataRow(double.PositiveInfinity, "7FF0000000000000", DisplayName = "Positive Infinity ")]
        [DataRow(double.NegativeInfinity, "FFF0000000000000", DisplayName = "Negative Infinity")]
        [DataRow(double.MaxValue, "7FEFFFFFFFFFFFFF", DisplayName = "Maximum Positive Value")]
        [DataRow(double.MinValue, "FFEFFFFFFFFFFFFF", DisplayName = "Maximum Negative Value")]
        [DataRow(double.Epsilon, "0000000000000001", DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow(-double.Epsilon, "8000000000000001", DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow(0, "0000000000000000", DisplayName = "Zero")]

        public void IEEE_Float_To_8Byte_String_BigEndian(double inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString();

            //Assert
            Assert.AreEqual(expectedString, actualString);

        }

        [DataTestMethod]
        [DataRow(double.PositiveInfinity, "000000000000F07F", DisplayName = "Positive Infinity ")]
        [DataRow(double.NegativeInfinity, "000000000000F0FF", DisplayName = "Negative Infinity")]
        [DataRow(double.MaxValue, "FFFFFFFFFFFFEF7F", DisplayName = "Maximum Positive Value")]
        [DataRow(double.MinValue, "FFFFFFFFFFFFEFFF", DisplayName = "Maximum Negative Value")]
        [DataRow(double.Epsilon, "0100000000000000", DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow(-double.Epsilon, "0100000000000080", DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow(0, "0000000000000000", DisplayName = "Zero")]

        public void IEEE_Float_To_8Byte_String_LittleEndian(double inputValue, string expectedString)
        {
            //Arrange
            string actualString;

            //Act
            actualString = inputValue.ToIEEEString(true);

            //Assert
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void IEEE_NaNString_To_Double_BigEndian()
        {
            //Arrange 
            string NanString = "7FF8000000000000";
            double actualValue;

            //Act
            actualValue = NanString.ToDouble();

            //Assert
            Assert.IsTrue(double.IsNaN(actualValue));

        }

        [TestMethod]
        public void IEEE_NaNString_To_Double_LittleEndian()
        {
            //Arrange 
            string NanString = "000000000000F87F";
            double actualValue;

            //Act
            actualValue = NanString.ToDouble(true);

            //Assert
            Assert.IsTrue(double.IsNaN(actualValue));

        }
        [DataTestMethod]
        [DataRow("7FF0000000000000", double.PositiveInfinity, DisplayName = "Positive Infinity ")]
        [DataRow("FFF0000000000000", double.NegativeInfinity, DisplayName = "Negative Infinity")]
        [DataRow("7FEFFFFFFFFFFFFF", double.MaxValue, DisplayName = "Maximum Positive Value")]
        [DataRow("FFEFFFFFFFFFFFFF", double.MinValue, DisplayName = "Maximum Negative Value")]
        [DataRow("0000000000000001", double.Epsilon, DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow("8000000000000001", -double.Epsilon, DisplayName = "Minimum Negative Subnormal Value")]

        public void IEEE_8Byte_String_To_Float_BigEndian(string inputString, double expectedDouble)
        {
            //Arrange
            double actualDouble;

            //Act
            actualDouble = inputString.ToDouble();

            //Assert
            Assert.AreEqual(expectedDouble, actualDouble);

        }

        [DataTestMethod]
        [DataRow("000000000000F07F", double.PositiveInfinity, DisplayName = "Positive Infinity ")]
        [DataRow("000000000000F0FF", double.NegativeInfinity, DisplayName = "Negative Infinity")]
        [DataRow("FFFFFFFFFFFFEF7F", double.MaxValue, DisplayName = "Maximum Positive Value")]
        [DataRow("FFFFFFFFFFFFEFFF", double.MinValue, DisplayName = "Maximum Negative Value")]
        [DataRow("0100000000000000", double.Epsilon, DisplayName = "Minimum Positive Subnormal Value")]
        [DataRow("0100000000000080", -double.Epsilon, DisplayName = "Minimum Negative Subnormal Value")]
        [DataRow("0000000000000000", 0, DisplayName = "Positive Zero")]
        [DataRow("0000000000000080", 0, DisplayName = "Negative Zero")]

        public void IEEE_8Byte_String_To_Float_LittleEndian(string inputString, double expectedDouble)
        {
            //Arrange
            double actualDouble;

            //Act
            actualDouble = inputString.ToDouble(true);

            //Assert
            Assert.AreEqual(expectedDouble, actualDouble);

        }

        [TestMethod]
        public void IEEE_NaNDouble_To_8Byte_String_BigEndian()
        {
            //Arrange 
            string NanString = "FFF8000000000000";
            string actualString;

            //Act
            actualString = double.NaN.ToIEEEString();

            //Assert
            Assert.AreEqual(NanString, actualString);

        }

        [TestMethod]
        public void IEEE_NaNDouble_To_8Byte_String_LittleEndian()
        {
            //Arrange 
            string NanString = "000000000000F8FF";
            string actualString;

            //Act
            actualString = double.NaN.ToIEEEString(true);

            //Assert
            Assert.AreEqual(NanString, actualString);

        }
    }
}
