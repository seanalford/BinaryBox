using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolbox.Checksum.Test
{
    [TestClass]
    public class TestChecksumCrcExtentions
    {
        [DataTestMethod]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 91, 11 }, DisplayName = "Test 1")]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0, 8, 4, 0, 4, 0, 1, 0, 6, 3 }, new byte[] { 235, 115 }, DisplayName = "Test 2")]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 }, new byte[] { 43, 193 }, DisplayName = "Test 3")]
        [DataRow(new byte[] { 48, 48, 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32 }, new byte[] { 63, 149 }, DisplayName = "Test 4")]
        public void TestCrc16Checksum(byte[] data, byte[] pExpectedResult)
        {
            // Arrange
            byte[] result = new byte[2];

            // Act
            result = data.Crc16();

            // Assert
            CollectionAssert.AreEqual(result, pExpectedResult);
        }

    }
}
