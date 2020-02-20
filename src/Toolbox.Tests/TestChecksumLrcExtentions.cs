using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Toolbox.Checksum.Test
{
    [TestClass]
    public class TestChecksumLrcExtentions
    {
        [DataTestMethod]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, (byte)0, DisplayName = "Test byte[] Lrc - Test 1 All zeros")]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0, 8, 4, 0, 4, 0, 1, 0, 6, 3 }, (byte)14, DisplayName = "Test byte[] Lrc - Test 2")]
        [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 }, (byte)1, DisplayName = "Test byte[] Lrc - Test 3")]
        [DataRow(new byte[] { 48, 48, 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32 }, (byte)63, DisplayName = "Test byte[] Lrc - Test 4")]
        public void TestLrcChecksumBytesToByte(byte[] data, byte pExpectedResult)
        {
            Assert.AreEqual(data.Lrc(), pExpectedResult);
        }
    }
}
