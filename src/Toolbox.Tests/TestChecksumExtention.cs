using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Toolbox.Checksum.Test
{
    [TestClass]
    public class TestEagleExtentionChecksums
    {

        [TestMethod]
        public void TestChecksumLRC()
        {
            byte[] rxMessage = { };

            byte[] rx1 = { 6, 2, 48, 48, 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 6, 2, 48, 48 };
            byte[] rx2 = { 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, };
            byte[] rx3 = { 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 114, 114, 32, 86, 111, 108, 117, 109 };
            byte[] rx4 = { 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32, 3, 7 };
            
            int rxOriginalLenght = rxMessage.Length;
            Array.Resize(ref rxMessage, rxOriginalLenght + rx1.Length);
            Array.Copy(rx1, 0, rxMessage, rxOriginalLenght, rx1.Length);

            rxOriginalLenght = rxMessage.Length;
            Array.Resize(ref rxMessage, rxOriginalLenght + rx2.Length);
            Array.Copy(rx2, 0, rxMessage, rxOriginalLenght, rx2.Length);

            rxOriginalLenght = rxMessage.Length;
            Array.Resize(ref rxMessage, rxOriginalLenght + rx3.Length);
            Array.Copy(rx3, 0, rxMessage, rxOriginalLenght, rx3.Length);

            rxOriginalLenght = rxMessage.Length;
            Array.Resize(ref rxMessage, rxOriginalLenght + rx4.Length);
            Array.Copy(rx4, 0, rxMessage, rxOriginalLenght, rx4.Length);

            int LastSTX = Array.LastIndexOf<byte>(rxMessage, 2) + 1;
            int LastETX = Array.LastIndexOf<byte>(rxMessage, 3);
            int mlength = LastETX - LastSTX;

            byte[] message = new byte[mlength];
            Array.Copy(rxMessage, LastSTX, message, 0, mlength);

            // Decode 

                       
            TestLRC(message, 63);

            //int index = Array.IndexOf<byte>(rx, 2);
            //int lastIndex = Array.LastIndexOf<byte>(rx, 2);

            TestLRC("000000000000000000000000", 0);
            TestLRC("000003010001010840401063", 14);
            TestLRC("0000000100010100", 1);
            TestLRC("000003002A000000", Convert.ToByte('p'));
            TestLRC("0000000021000800", 11);

            string messageString = "0001AB002100FF20Corr Volume        00000000     ";
            TestLRC(messageString, 7);

            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);

            //byte[] message = { 48, 8, 8, 9, 5, 6, 8, 8, 0, 9, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32 };
            
            TestLRC(messageBytes, 7);
            
        }

        private void TestLRC(string messageString, byte pExpectedResult)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);            

            // Check String to Byte
            Assert.AreEqual(messageString.ChecksumToByte(ChecksumTypes.LRC), pExpectedResult);
            Assert.AreEqual(messageBytes.ChecksumToByte(ChecksumTypes.LRC), pExpectedResult);
            Assert.AreEqual(messageString.ToByte(), pExpectedResult);
            
        }

        private void TestLRC(byte[] messageBytes, byte pExpectedResult)
        {
            string messageString  = Encoding.ASCII.GetString(messageBytes);

            // Check String to Byte
            Assert.AreEqual(messageString.ChecksumToByte(ChecksumTypes.LRC), pExpectedResult);
            Assert.AreEqual(messageBytes.ChecksumToByte(ChecksumTypes.LRC), pExpectedResult);
            Assert.AreEqual(messageString.ToByte(), pExpectedResult);

        }

    }
}
